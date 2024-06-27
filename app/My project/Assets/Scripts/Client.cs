using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System.Web;
using System.IO;

public interface IHttpClient
{
    Task PostUpload(string filePath, string jsonData);
    Task<object> GetStatus(int arId);
    Task<object> GetUserVideo(int arId);
    Task PatchInitConversion();
}

public class Client : IHttpClient
{
    private string devUrl = "http://localhost:8000";
    private Player player = Player.GetInstance();
    HttpClient myClient = new HttpClient();

    public Task PostUpload(string filePath, string jsonData)
    {
        
        throw new NotImplementedException();
    }

    public async Task<object> GetStatus(int arId)
    {
        var parameters = HttpUtility.ParseQueryString(string.Empty);
        parameters["user"] = player.GetId();
        parameters["ar_id"] = arId.ToString();

        string apiUrl = $"{devUrl}/getStatus?{parameters}";

        // Request
        var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);

        HttpResponseMessage response = await myClient.SendAsync(request);

        if (response.IsSuccessStatusCode)
        {
            string responseContent = await response.Content.ReadAsStringAsync();

            //Process response data
            return JsonConvert.DeserializeObject<object>(responseContent);
        }

        return "Error: Status not received.";
    }

    public async Task<object> GetUserVideo(int arId)
    {
        var parameters = HttpUtility.ParseQueryString(string.Empty);
        parameters["user"] = player.GetId();
        parameters["ar_id"] = arId.ToString();

        string apiUrl = $"{devUrl}/getUserVideo?{parameters}";

        // Request
        var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
        HttpResponseMessage response = await myClient.SendAsync(request);

        if (response.IsSuccessStatusCode)
        {
            string responseContent = await response.Content.ReadAsStringAsync();

            return responseContent;
        }

        return "Error: File not received.";

    }

    

    public Task PatchInitConversion()
    {
        throw new NotImplementedException();
    }
}




