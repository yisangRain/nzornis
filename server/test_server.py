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
test_db = "test.db"

testVideo =  "server/testAssets/blob.mp4"
testImg = "server/testAssets/bird_1.jpg"

testUser = 0
testSighting = 0
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


    # def test_PATCH_initiateAndCheckConversion(self):
    #     """
    #     Testing initiating conversion and conversion.
    #     Difficult to separate the two due to server and test running on
    #     a single terminal
    #     """
    #     # variables
    #     filename = "blobConversionTest.mp4"

    #     # insert test data entry into the database
    #     conn = database.create_connection(testDatabase)
    #     q = f"INSERT INTO AR (user, filename, status) \
    #         VALUES ('{testUser}', '{filename}', 'raw');"
    #     id = database.insert_database(conn, q)
    #     conn.close()

    #     # Define the URL of the server
    #     url = f'http://{server_ip_address}:{server_port}/initCon'

    #     # send patch request
    #     params = {'user': testUser, 'ar_id': id}
    #     response = requests.patch(url, params=params)

    #     # receive status back (Accepted (202) | could not find file (404))
    #     self.assertEqual(response.status_code, 202)
        
    #     # check and delete db entry
    #     conn = database.create_connection(testDatabase)
    #     [check_conversion] = database.query_database(conn, f"SELECT status FROM AR WHERE ar_id={id};")
    #     self.assertEqual(check_conversion[0], "converted")
    #     delete_result = database.delete_database(conn, id)
    #     conn.close()

    #     # assertion to check deletion
    #     self.assertTrue(delete_result)

    #     # check and delete converted file
    #     self.assertTrue(os.path.exists(f'server/converted/{filename}'))

    #     os.remove(f'server/converted/{filename}')



    # def test_GET_conversionStatus(self):
    #     """
    #     Request to query the conversion status
    #     """
    #     # Insert a new test entry into the db
    #     conn = database.Connection(testDatabase).create_connection()
    #     database.Cell.new(conn, testGrid, testSighting, database.ConversionStatus.READY)

    #     # Define url for test
    #     url = f'http://{server_ip_address}:{server_port}/getConversionStatus'
        
    #     # Make a request and get res back. Code 201
    #     param = {'sighting_id': testSighting }
    #     response = requests.get(url, params=param)

    #     # Check results
    #     self.assertEqual(response.status_code, 200)
    #     result = json.loads(response.json())
    #     self.assertEqual(result['status'], database.ConversionStatus.READY.name)

    #     # Delete db entry
    #     database.Cell.delete(conn, testSighting)
    #     conn.close()



    # def test_GET_convertedVideo(self):
    #     """
    #     Test GET request for user's own video
    #     """
    #     # Create entry into the db
    #     conn = database.create_connection(testDatabase)
    #     q = f"INSERT INTO AR (user, filename, status) \
    #     VALUES ('{testUser}', 'blob.mp4', 'converted');"
    #     id = database.insert_database(conn, q)

    #     # test file path
    #     testPath = "server/converted/blob.mp4"

    #     # receive video
    #     url = f'http://{server_ip_address}:{server_port}/getUserVideo'
    #     param = {'user': testUser, 'ar_id': id}
    #     response = requests.get(url, params=param)

    #     # check status code and video received
    #     self.assertEqual(response.status_code, 200)
    #     self.assertEqual(
    #         response.headers['Content-Disposition'], 
    #         f'attachment; filename="{os.path.basename(testPath)}"')
        
    #     # Delete db entry
    #     database.delete_database(conn, id)
    #     conn.close()
     


    # def test_GET_videoByID(self):
    #     pass




if __name__ == '__main__':
    unittest.main()