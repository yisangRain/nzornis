from http.server import BaseHTTPRequestHandler, HTTPServer
import threading
from urllib.parse import urlparse, parse_qs
import socket
from sqlite3 import Error
import cgi
import time
import json
import converter
import database
import os
import magic
import database_init

#note: use flask?

############################################################################
# Global variables 

# DATABASE = "pythonsqlite.db"
DATABASE = "test.db"


############################################################################

###############################################################################
# HTTP request handler
###############################################################################

class NZOrnisHTTPHandler(BaseHTTPRequestHandler):
    """ HTTP request handler for NZ Ornis server.
    do_GET(): GET request handler
    do_POST(): POST request handler
    do_PATCH(): PATCH request handler

    """

    def do_GET(self):
        # Break down the request URL
        parsed_url = urlparse(self.path)
        path = parsed_url.path
        params = parse_qs(parsed_url.query)
        
        # Initiate connection to the DB
        conn = database.Connection().create_connection(DATABASE)

        # GET media conversion status
        if path == '/getConversionStatus':
            # query the database
            cell = database.Cell()
            status = database.ConversionStatus(cell.check_status(conn, params['sighting_id'][0])).name

            conn.close()

            # send response back
            json_data = json.dumps({'sighting_id': params['sighting_id'][0], 'status': status})
            self.send_response(200)
            self.send_header('Content-type', 'application/json')
            self.end_headers()
            self.wfile.write(json.dumps(json_data).encode())
          
        # GET media by media id
        elif path == '/getMedia':
            #query file path
            s = database.Sighting()
            result = database.Sighting().get_by_id(conn, database.Entity.SIGHTING, params['sighting_id'][0])
            filename = result[0][-1]
            c = database.Cell()
            status = c.check_status(conn, params['sighting_id'][0])
            filepath = None
            if status == database.ConversionStatus.READY:
                filepath = f"server/converted/{filename}"
            else:
                filepath = f"server/received/{filename}"

            mime = None

            if os.path.exists(filepath): # determine mime type
                if filename.lower().endswith('.png'):
                    mime = 'image/png'
                elif filename.lower().endswith('.jpg') or filename.lower().endswith('.jpeg'):
                    mime = 'image/jpeg'
                elif filename.lower().endswith('.mp3'):
                    mime = 'audio/mpeg'
                else:
                    mime = 'application/octet-stream'
                    
                # set up response
                self.send_response(200)
                self.send_header('Content-Type', mime)
                self.send_header('Content-Disposition', f'attachment; filename="{os.path.basename(filepath)}"')
                self.end_headers()
                
                # Send the file content
                with open(filepath, 'rb') as file:
                    self.wfile.write(file.read())
            else:
                self.send_response(404)
                self.send_header('Content-Type', 'application/json')
                self.end_headers()
                response = {"status": "File not found"}
                self.wfile.write(bytes(json.dumps(response), 'utf-8'))

        # Get user's media
        elif path == "/gallery":
            result = database.Sighting.get_by_id(conn, database.Entity.USER, params['user_id'][0])
            json_data = database.Sighting.toJson(result)

            self.send_response(200)
            self.send_header('Content-Type', 'application/json')
            self.end_headers()
            self.wfile.write(json_data)

        
        # default 
        else:
            self.send_response(400) # nothing to send
            self.send_header('Content-Type', 'application/json')
            self.end_headers()
            response = {"status": "Bad Request", "message": "Invalid request"}
            self.wfile.write(bytes(json.dumps(response), 'utf-8'))

        conn.close()


    def do_POST(self):
        # Break down the request URL
        parsed_url = urlparse(self.path)
        path = parsed_url.path
        params = parse_qs(parsed_url.query)
    
        content_type, pdict = cgi.parse_header(self.headers['Content-Type'])

        # handle media upload
        # client: param = (user: userId(integer)) body = (file: videofile(mp4/H.264), 
        # json: {title, desc, time, lon, lat})
        if (path == '/upload') and (content_type.startswith('multipart/form-data')):

            pdict['boundary'] = bytes(pdict['boundary'], "utf-8")
            pdict['CONTENT-LENGTH'] = int(self.headers['Content-Length'])

            fields = cgi.parse_multipart(self.rfile, pdict)

            json_data = json.loads(fields.get('json')[0])
            media_data = fields['file'][0]
            mime = magic.Magic(mime=True).from_buffer(media_data)

            filename = filename_generator(params['user'][0], mime)
            
            try:
                # Save the video into the server
             
                with open(f'server/received/{filename}', 'wb') as f:
                    f.write(media_data)
                    print(f"Saved media file {filename}")

                # Insert other information into the DB
                entry_info = database.New_Sighting(json_data.get('title'), json_data.get('desc'), params['user'][0],
                                                   json_data.get('time'), json_data.get('lon'), json_data.get('lat'), filename)
                
                conn = database.Connection().create_connection(dbname=DATABASE)
                entry = database.Sighting(entry_info)
                entry_id = entry.new(conn) # row id number of the uploaded data
                conn.close()

                self.send_response(201, "Video successfully uploaded")
                self.send_header('Content-type', 'application/json')
                self.end_headers()
                # send the entry id and filename back
                self.wfile.write(json.dumps({"id": entry_id, "filename": filename}).encode())

                conn.close()
          
            except Error as e:
                print(e)
                self.send_response(500, "Could not receive the file.")

        # default 
        else:
            self.send_response(404) # nothing to send
        
    

    def do_PATCH(self):
        # Break down the request URL
        parsed_url = urlparse(self.path)
        path = parsed_url.path
        params = parse_qs(parsed_url.query)

        # PATCH AR position
        if path == '/initiateConversion':
            s_id = params['sighting_id'][0]

            conn = database.Connection().create_connection(DATABASE)
            result = database.Cell().check_status(conn, s_id)

            if result == database.ConversionStatus.READY:
                self.send_response(200, 'Ready')
            
            elif result == database.ConversionStatus.CONVERTING:
                self.send_response(200, 'Converting now')
            
            elif result == database.ConversionStatus.RAW_IMAGE:
                converter.img_to_AR(conn, s_id)
                self.send_response(200, "Processing")

            elif result == database.ConversionStatus.RAW_VIDEO:
                converter.vid_to_AR(conn, s_id)
                self.send_response(200, "Processing")
            
            else:
                # return 503 error or similar
                self.send_response(500, "Internal server error")
      
             # Send a simple response back
            response_data = {f'status": "{result}", "message": "PATCH request processed'}
            self.send_header('Content-Type', 'application/json')
            self.end_headers()
            self.wfile.write(bytes(json.dumps(response_data), 'utf-8'))
        
            conn.close()

        # default 
        else:
            self.send_response(404) # nothing to send


########################################################################
# Helper functions
#######################################################################

def filename_generator(user_id, mime):
    now = int(time.time())

    if mime == 'image/png':
        return f'{user_id}_{now}.png'
    elif mime == 'image/jpeg':
        return f'{user_id}_{now}.jpeg'
    elif mime == 'audio/mpeg':
        return f'{user_id}_{now}.mp3'
    elif mime == 'application/octet-stream' or mime == 'video/mp4':
        return f'{user_id}_{now}.mp4'
    else:
        raise NotImplementedError('Received file type not configured.')



######################################################################
# Main
######################################################################


def run_server():
    # Initiates and maintains server until termination
    global http_server 

    # Initialise DB if it's not setup 
    database_init.initialise_database(DATABASE)

    # set port
    server_port = 8000

    # Find and use the machine's IP address
    server_address = (socket.gethostbyname(socket.gethostname()), server_port)
    http_server = HTTPServer(server_address, NZOrnisHTTPHandler)
    
    # For debugging purposes
    print(f"Server running on {server_address[0]} on port {server_address[1]}")

    # Maintain server forever
    http_server.serve_forever()


def stop_server():
    global http_server
    if http_server:
        http_server.shutdown()
        print("Server stopped")


# Run the server in a separate thread
server_thread = threading.Thread(target=run_server)
server_thread.start()
