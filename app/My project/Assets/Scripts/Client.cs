using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;


public class Client : MonoBehaviour
{
    private string serverIP;
    private int serverPort = 8000;
    private Player player = Player.GetInstance();

    /// <summary>
    /// Sets the current machine's IP address as the server IP
    /// For development purpose only
    /// </summary>
    public void Start()
    {
        GetLocalIPAddress();
    }

    public void PostNewVideo(string videoPath)
    {
        string url = serverIP + "/upload";
        StartCoroutine(UploadFile(videoPath, url));
    }

    /// <summary>
    /// Method <c>UploadFile</c> Uploads given file 
    /// </summary>
    /// <param name="filePath">Path to the target file</param>
    /// <param name="uploadUrl">Target url</param>
    /// <returns></returns>
    private IEnumerator UploadFile(string filePath, string uploadUrl)
    {
        // Check if file exists
        if (!File.Exists(filePath))
        {
            Debug.LogError("File not found at " + filePath);
            yield break;
        }

        // Read the file into a byte array
        byte[] fileData = File.ReadAllBytes(filePath);

        // setup params
        string param = "";


        // Create a UnityWebRequest for a PUT request
        UnityWebRequest req = new UnityWebRequest(uploadUrl, UnityWebRequest.kHttpVerbPOST);
        req.uploadHandler = new UploadHandlerRaw(fileData);
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "multipart/form-data");


        // Send request and wait for response
        yield return req.SendWebRequest();

        // Handle response
        if (req.result == UnityWebRequest.Result.ConnectionError || req.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error uploading file: " + req.error);
        }
        else
        {
            Debug.Log("File upload success. Response: " + req.downloadHandler.text);
        }
    }


    public void GetStatus()
    {

    }


    /// <summary>
    /// Method <c>GetLocalIPAddress</c> Queries for the current machine's IP address
    /// </summary>
    /// <returns>string IP address</returns>
    private string GetLocalIPAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }
        throw new Exception("No network adapters with an IPv4 address in the system!");
    }

    public void SetServerIP(string newIp)
    {
        serverIP = newIp;
    }


}



