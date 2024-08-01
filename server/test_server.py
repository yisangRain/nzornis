from main_server import run_server, stop_server
import database
import unittest
import threading
import time
import requests
import os
import json
import socket
from database_init import BOUNDARY, insert_test_grid

"""
Collection of tests for server codebase

based on localhost url

NB: The server code lacks access control.
Please implement one in if the application becomes live
"""
test_db = "test.db"

testVideo =  "server/testAssets/blob.mp4"
testImg = "server/testAssets/bird_1.jpg"

testReceivedVid = "blobConversionTest.mp4"

testUser = 0
testLat = -43.52628
testLon =  172.58623

testSighting = database.New_Sighting("test title", "test desc", testUser,
                                     int(time.time()),testLon, testLat, testReceivedVid)

testSightingId = 0
testCell = 0
testGrid = 100

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


    def test_GET_CheckConversionStatus(self):
        """
        Testing initiating conversion and conversion.
        Difficult to separate the two due to server and test running on
        a single terminal
        """

        # insert test data entry into the database
        conn = database.Connection().create_connection(test_db)
        s = database.Sighting(testSighting)
        insert_test_grid(test_db, BOUNDARY)
        s_id = s.new(conn)
        conn.close()

        # Define the URL of the server
        url = f'http://{server_ip_address}:{server_port}/getConversionStatus'

        # send patch request
        params = {'sighting_id': s_id}
        response = requests.get(url, params=params)

        # receive status back OK 200
        self.assertEqual(response.status_code, 200)

        # Check results
        self.assertEqual(response.status_code, 200)
        result = json.loads(response.json())
        self.assertEqual(result['status'], database.ConversionStatus.RAW_VIDEO.name)

        # Delete db entry
        conn = database.Connection().create_connection(test_db)
        database.Cell().delete(conn, s_id)
        conn.close()


    def test_GET_Video(self):
        """
        Test GET request for video
        """
        # Create entry into the db
        conn = database.Connection().create_connection(test_db)
        s = database.Sighting(testSighting)
        insert_test_grid(test_db, BOUNDARY)
        s_id = s.new(conn)
        conn.close()

        # receive video
        url = f'http://{server_ip_address}:{server_port}/getMedia'
        param = {'sighting_id': s_id}
        response = requests.get(url, params=param)

        # check status code and video received
        self.assertEqual(response.status_code, 200)
        self.assertEqual(
            response.headers['Content-Disposition'], 
            f'attachment; filename="{os.path.basename(testReceivedVid)}"')
        
        # Delete db entry
        conn = database.Connection().create_connection(test_db)
        database.Sighting().delete(conn, s_id)
        conn.close()


    def test_POST_videoUpload(self):
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
            json_data = json.dumps({"title": "test title", "desc": "test desc", 
                                    "time": time.time(), "lon": 172.58623, "lat": -43.52628 })
            data = {'json': json_data}
            params = {'user': testUser}
            response = requests.post(url, files=files, data=data, params=params)
            filename = response.json().get('filename', None)

            self.assertEqual(response.status_code, 201)

            self.assertTrue(os.path.exists(f'server/received/{filename}'))

            os.remove(f'server/received/{filename}')


    def test_POST_imageUpload(self):
        """
        Test POST /upload endpoint
        - send image file & json data
        - receive status code 200 (success) back
        - check file is saved within the server machine
        - remove the saved file (clean up)
        """

        # Define the URL of the server
        url = f'http://{server_ip_address}:{server_port}/upload'

        with open(testImg, 'rb') as f:
            files = {'file': f}
            json_data = json.dumps({"title": "test title", "desc": "test desc", 
                                    "time": time.time(), "lon": 172.58623, "lat": -43.52628 })
            data = {'json': json_data}
            params = {'user': testUser}
            response = requests.post(url, files=files, data=data, params=params)
            filename = response.json().get('filename', None)

            self.assertEqual(response.status_code, 201)

            self.assertTrue(os.path.exists(f'server/received/{filename}'))

            os.remove(f'server/received/{filename}')


    def test_PATCH_initiateConversion(self):
        """
        Test triggering conversion
        """
        # Create entry into the db
        conn = database.Connection().create_connection(test_db)
        s = database.Sighting(testSighting)
        insert_test_grid(test_db, BOUNDARY)
        s_id = s.new(conn)
        conn.close()

        # define URL
        url = f"http://{server_ip_address}:{server_port}/initiateConversion"

        # Send req
        param = {'sighting_id': s_id}
        response = requests.patch(url, params=param)

        self.assertEqual(response.status_code, 200)

        # Manually check if conversion successful (take ~5 mins :/)




     






if __name__ == '__main__':
    unittest.main()