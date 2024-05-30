from http.server import BaseHTTPRequestHandler, HTTPServer
import threading
from urllib.parse import urlparse, parse_qs
import socket
import sqlite3
from sqlite3 import Error
import cgi
import time
import json
import converter



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
        query_params = parse_qs(parsed_url.query)

        # GET processed AR back 
        if path == '/getProcessed':
            connection = create_connection(DATABASE)
            query = f"SELECT filename, status FROM AR WHERE id = '{query_params['id']}"

            result = query_database(connection, query)

            if len(result) == 0:
                self.send_error(404, "Could not find the file.")
            
            elif result[0][1] == 'Raw':
                self.send_error(503, "still converting.")
            
            elif result[0][1] == 'File Error':
                self.send_error(503, "File Error, please upload the video again")
            
            elif result[0][1] == 'Converted':
                try:
                    with open('processed/processed_output.mp4', 'rb') as f:
                        # Set the response headers
                        self.send_response(200)
                        self.send_header('Content-type', 'video/mp4')
                        self.end_headers()
                        # Read and send the video file data in chunks
                        chunk_size = 4096
                        while True:
                            chunk = f.read(chunk_size)
                            if not chunk:
                                break
                            self.wfile.write(chunk)
                except IOError:
                    self.send_error(404, 'File not found')

        # default 
        else:
            self.send_response(404) # nothing to send


    def do_POST(self):
        # Break down the request URL
        parsed_url = urlparse(self.path)
        path = parsed_url.path
        params = parsed_url.params

        id = None

        # POST raw video to be processed
        if path == '/process':

            # Determine output file name
            output_name = f"{params['user']}_{int(time.time())}.mp4"

            if params['type'] == 'video':
                form_data = cgi.FieldStorage(fp=self.rfile, headers=self.headers, environ={'REQUEST_METHOD': 'POST'})
                file_field = form_data['file']

                if file_field.filename:
                    file_data = file_field.file.read()
                    proceed = True

                    # Write the received file into the server to be processed later
                    with open("received/" + output_name, 'wb') as f:
                        f.write(file_data)

                    try:
                        # save the user and filename into the database (intitial update)
                        query = f"INSERT into AR (user, filename, status) VALUES ('{params['user']}', '{output_name}, 'raw');"
                        connection = create_connection(DATABASE)
                        id = insert_database(connection, query)
                        print(f"Received video saved into the DB: user - {params['user']}, file = {output_name}")

                        self.send_response(200, "Video received successfully.")
                        self.send_header('Content-type', 'application/json')
                        self.end_headers()
                        self.wfile.write(json.dumps({"id": id, "filename": output_name}).encode())
                    
                    except Error as e:
                        print(e)
                        proceed = False

                    if proceed == True:
                        # update the database based on the conversion result
                        converter.to_AR(output_name, create_connection(DATABASE), id)
                    
                    connection.close()
            
            else:
                self.send_error(503)

        # default 
        else:
            self.send_response(404) # nothing to send
    

    def do_PATCH(self):
        # Break down the request URL
        parsed_url = urlparse(self.path)
        path = parsed_url.path
        query_params = parse_qs(parsed_url.query)

        # PATCH AR position
        if path == '/addPosition':
            pass

        # default 
        else:
            self.send_response(404) # nothing to send


#####################################################################
# Database 
#####################################################################

DATABASE = "pythonsqlite.db"

def create_connection(db_file):
    # Creates connection to the SQLite database
    conn = None

    try:
        conn = sqlite3.connect(db_file)
        return conn
    
    except Error as e:
        print(e)


def query_database(connection, query):
    """
    Query the database using the given connection and query string
    """
    cursor = connection.cursor()

    cursor.execute(query)

    return cursor.fetchall()


def insert_database(connection, query):
    """
    Insert into the database using the given connection and query string
    """
    cursor = connection.cursor()

    cursor.execute(query)
    connection.commit()

    return cursor.lastrowid


def setup_database(connection):
    # function to setup database if it's not ready

    cursor = connection.cursor()

    test_query = "SELECT * FROM AR LIMIT 1;"

    try:
        query_database(connection, test_query)
        print("Using existing database.")
    
    except Error as e:

        setup_query = "CREATE TABLE AR ( \
            AR_ID integer PRIMARY KEY AUTOINCREMENT, \
            user integer, \
            ar_name text, \
            position text, \
            filename text, \
            longitude real, \
            latitude real, \
            region integer, \
            geojson text, \
            description text, \
            status text \
            );"
        
        cursor.execute(setup_query)
        connection.commit()

        print("Database setup successful.")

        



######################################################################
# Main
######################################################################


def run_server():
    # Initiates and maintains server until termination
    global http_server 

    # Initialise DB if it's not setup 
    connection = create_connection(DATABASE)
    setup_database(connection)
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
