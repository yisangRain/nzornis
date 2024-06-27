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

############################################################################
# Global variables 

DATABASE = "pythonsqlite.db"


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
        
        # GET ar conversion status
        if path == '/getStatus':
            # set db connection up
            conn = database.create_connection(DATABASE)
            status, _ = database.check_status(conn, params['user'][0], params['ar_id'][0])
            conn.close()

            # send response back
            json_data = json.dumps({'ar_id': params['ar_id'][0], 'status': status})
            self.send_response(200)
            self.send_header('Content-type', 'application/json')
            self.end_headers()
            self.wfile.write(json.dumps(json_data).encode())
          
        # GET user's video
        elif path == '/getUserVideo':
            # set db up
            conn = database.create_connection(DATABASE)
            [(filename)] = database.get_filename(conn, params['user'][0], params['ar_id'][0])
            
            filepath = f"server/converted/{filename}"
            if os.path.exists(filepath):
                # set up response
                self.send_response(200)
                self.send_header('Content-Type', 'application/octet-stream')
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
        
        # default 
        else:
            self.send_response(400) # nothing to send
            self.send_header('Content-Type', 'application/json')
            self.end_headers()
            response = {"status": "Bad Request", "message": "Invalid request"}
            self.wfile.write(bytes(json.dumps(response), 'utf-8'))



    def do_POST(self):
        # Break down the request URL
        parsed_url = urlparse(self.path)
        path = parsed_url.path
        params = parse_qs(parsed_url.query)
    
        content_type, pdict = cgi.parse_header(self.headers['Content-Type'])

        id = None

        # handle video upload
        # client: param = (user: userId(integer)) body = (file: videofile(mp4/H.264), json: {location: PointGeoJSON
        # movement, startTime, position})
        if (path == '/upload') and (content_type.startswith('multipart/form-data')):
            filename = f"user{params['user'][0]}_{int(time.time())}.mp4"
            pdict['boundary'] = bytes(pdict['boundary'], "utf-8")
            pdict['CONTENT-LENGTH'] = int(self.headers['Content-Length'])

            fields = cgi.parse_multipart(self.rfile, pdict)

            json_data = json.loads(fields.get('json')[0])
            video_data = fields.get('file')[0]
            
            try:
                # Save the video into the server
             
                with open(f'server/received/{filename}', 'wb') as f:
                    f.write(video_data)
                    print(f"Saved video file {filename}")

                # Insert other information into the DB
                conn = database.create_connection(DATABASE)
                query = f"INSERT into AR (user, filename, status, geojson, time, position) \
                    VALUES ('{params['user'][0]}', '{filename}', 'raw', '{json_data.get('location')}', '{json_data.get('startTime')}', '{json_data.get('position')}');"
                # Receive the database row id
                id = database.insert_database(conn, query)

                self.send_response(201, "Video successfully uploaded")
                self.send_header('Content-type', 'application/json')
                self.end_headers()
                # send the entry id and filename back
                self.wfile.write(json.dumps({"id": id, "filename": filename}).encode())
          
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
        if path == '/initCon':
            ar_id = params['ar_id'][0]
            user = params['user'][0]

            conn = database.create_connection(DATABASE)
            result = database.check_status(conn, user, ar_id)

            if result:
                status, filename = result
            
                if status == 'raw':
                    database.change_status(conn, user, ar_id, 'processing')
                    self.send_response(200, "Initiating")
                    # convert
                    converter.to_AR(filename, conn, ar_id)

                elif status == 'processing':
                    # return current status as response
                    self.send_response(201, "Processing")

                elif status == 'converted':
                    # return current status as response
                    self.send_response(201, "Ready")
           
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


######################################################################
# Main
######################################################################


def run_server():
    # Initiates and maintains server until termination
    global http_server 

    # Initialise DB if it's not setup 
    connection = database.create_connection(DATABASE)
    database.setup_database(connection)
    connection.close()

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
