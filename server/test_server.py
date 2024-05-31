from main_server import run_server, stop_server
import database
import unittest
import threading
import time
import requests
import os
import json
import socket

"""
Collection of tests for server codebase

based on localhost url
"""

testVideo =  "server/testAssets/blob.mp4"
testGeoJSON = '{ \
  "type": "Feature", \
  "geometry": { \
    "type": "Point", \
    "coordinates": [125.6, 10.1] \
  }, \
  "properties": { \
    "name": "Dinagat Islands" \
  } \
}'
testMovement = '{ something }'
testPosition = '(10, 10, 10)'
testDatabase = "pythonsqlite.db"
testUser = 0

server_ip_address = socket.gethostbyname(socket.gethostname())
server_port = 8000

class TestServer(unittest.TestCase):

    def setUp(self):
        self.server_thread = threading.Thread(target=run_server)
        self.server_thread.start()
        time.sleep(1)


    def tearDown(self):
        stop_server()
        time.sleep(1)


    def test_POST_upload(self):
        """
        Test POST /upload endpoint
        - send video file & json data
        - receive status code 200 (success) back
        - check file is saved within the server machine
        - remove the saved file (clean up)
        """

        # Define the URL of the server
        url = f'http://{server_ip_address}:{server_port}/upload'

        with open(testVideo, 'rb') as f:
            files = {'file': f}
            json_data = json.dumps({"location": testGeoJSON, "movement": testMovement, "startTime": time.time(), "position": testPosition })
            data = {'json': json_data}
            params = {'user': testUser}
            response = requests.post(url, files=files, data=data, params=params)
            filename = response.json().get('filename', None)

            self.assertEqual(response.status_code, 200)

            self.assertTrue(os.path.exists(f'server/received/{filename}'))

            os.remove(f'server/received/{filename}')


    def test_PATCH_initiateConversion(self):
        # variables
        filename = "blob.mp4"

        # insert test data entry into the database
        conn = database.create_connection(testDatabase)
        q = f"INSERT INTO AR (user, filename, status) \
            VALUES ('testUser0', '{filename}', 'raw');"
        id = database.insert_database(conn, q)

        # Define the URL of the server
        url = f'http://{server_ip_address}:{server_port}/initiateConversion'

        # send patch request
        params = {'user': testUser, 'ar_id': id}
        response = requests.patch(url, params=params)

        # receive status back (yes (200) | could not find file (404))
        self.assertEqual(response.status_code, 200)
        
        # check and delete db entry
        q2 = f"DELETE FROM AR WHERE user='{testUser}' AND ar_id='{id}';"
        database.delete_database(conn, q2)



    # def test_GET_conversionStatus(self):
    #     pass



    # def test_GET_convertedVideo(self):
    #     pass



    # def test_PATCH_setPosition(self):
    #     pass


    # def test_GET_videoByID(self):
    #     pass




if __name__ == '__main__':
    unittest.main()