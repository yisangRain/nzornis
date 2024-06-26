using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading.Tasks;
using System.Text;
using System.Net.Http;
using Newtonsoft.Json;

public interface IHttpClient
{
    Task PostUpload(string filePath, string jsonData);
    Task<string> GetStatus(int arId);
    Task GetUserVideo();
    Task PatchInitConversion();
}

public class Client : MonoBehaviour, IHttpClient
{
    private string devUrl = "localhost:8000";
    private Player player = Player.GetInstance();
    HttpClient myClient = new HttpClient();

    public Task PostUpload(string filePath, string jsonData)
    {
        
        throw new NotImplementedException();
    }

    public async Task<string> GetStatus(int arId)
    {
        // Params {'user': userid, 'ar_id': id}
        string apiUrl = devUrl + "/getStatus";
        
        //Request
        var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
        request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            {"user", player.GetId()},
            {"ar_id", arId.ToString()}
        });
        HttpResponseMessage response = await myClient.SendAsync(request);

        if (response.IsSuccessStatusCode)
        {
            string responseContent = await response.Content.ReadAsStringAsync();

            //Process response data
            return JsonConvert.DeserializeObject<string>(responseContent);
        }

        return "Error: Status not received.";
    }

    public Task GetUserVideo()
    {
        throw new NotImplementedException();
    }

    public Task PatchInitConversion()
    {
        throw new NotImplementedException();
    }
}




