import converter
from server.server import run_server, stop_server
import unittest
import threading
import time
import requests
import os

"""
Collection of tests for server codebase
"""

testVideo =  "testAssets/blob.mp4"


class TestServer(unittest.TestCase):

    def setUp(self):
        self.server_thread = threading.Thread(target=run_server)
        self.server_thread.start()
        time.sleep(1)

    def tearDown(self):
        stop_server()
        time.sleep(1)

    def test_post_process(self):

        url = "http://localhost:8000/"

        with open(testVideo, 'rb') as f:
            files = {'file': f}
            params = {'user': 0}
            response = requests.post(url, files=files, params=params)
            filename = response.json().get('filename', None)

        self.assertEqual(response.status_code, 200)

        self.assertTrue(os.path.exists(f'received/{filename}'))

        os.remove(f'received/{filename}')


if __name__ == '__main__':
    unittest.main()