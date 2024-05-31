"""
Test suite for the server
"""
import requests
import socket
import json

test_video = 'server/testAssets/blob.mp4'

def client_POST():
    #get current machine's ip address
    server_ip_address = socket.gethostbyname(socket.gethostname())

    server_port = 8000

    # Define the URL of the server
    url_post = f'http://{server_ip_address}:{server_port}/upload'

    json_data = json.dumps({"location": "text", "movement": "text", "startTime": "text", "position": "text" })

    try:
        with open(test_video, 'rb') as f:
            files = {'file': (test_video, f)}
            data = {'json': json_data}
            params = {'user': 0}

            # Send a POST request to the server
            response = requests.post(url_post, files=files, params=params, data=data)

            # Print the response status code and content
            print(f'Response Status Code: {response.status_code}')
            print(f'Response Content: {response.text}')

    except requests.exceptions.RequestException as e:
        # Handle exceptions
        print(f'An error occurred: {e}')
    

client_POST()


