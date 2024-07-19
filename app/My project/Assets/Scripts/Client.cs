using UnityEngine;
using System;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System.Web;
using System.IO;
using System.Text;
using Assets.Scripts.Subclasses;

public interface IHttpClient
{
    public void SetClient(HttpClient httpClient);
    Task<string> PostUpload(string filePath, UploadObject jsonData);
    Task<string> GetStatus(int arId);
    Task<string> GetUserVideo(int arId);
    Task<string> PatchInitConversion(int arId);

    public void CloseClient();
}

public class Client : IHttpClient
{
    private string devUrl = "http://localhost:8000";
    private Player player = Player.instance;
    HttpClient myClient;

    public Client()
    {
        myClient = new HttpClient();
    }

    public void SetClient(HttpClient httpClient)
    {
        myClient = httpClient;
    }

    public async Task<string> PostUpload(string filePath, UploadObject jsonData)
    {
        var parameters = HttpUtility.ParseQueryString(string.Empty);
        parameters["user"] = player.GetId();

        string apiUrl = $"{devUrl}/upload?{parameters}";

        var jsonContent = new StringContent(JsonConvert.SerializeObject(jsonData), Encoding.UTF8, "application/json");
        var fileContent = new StreamContent(File.OpenRead(filePath));
        var multiContent = new MultipartFormDataContent();
        multiContent.Add(jsonContent, "json");
        multiContent.Add(fileContent, "file");

        // Request
        HttpResponseMessage response = await myClient.PostAsync(apiUrl, multiContent);

        response.EnsureSuccessStatusCode();

        if (response.IsSuccessStatusCode)
        {
            return response.ReasonPhrase;
        }

        return "Error: Error posting video.";
    }

    public async Task<string> GetStatus(int arId)
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
            return JsonConvert.DeserializeObject<StatusJson>(responseContent).status;
        }

        return "Error: Status not received.";
    }

    public async Task<string> GetUserVideo(int arId)
    {
        var parameters = HttpUtility.ParseQueryString(string.Empty);
        parameters["user"] = player.GetId();
        parameters["ar_id"] = arId.ToString();

        string apiUrl = $"{devUrl}/getUserVideo?{parameters}";

        string savePath = $"{Application.persistentDataPath}/ar_{arId}.mp4";

        // Request
        var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);

        try
        {
            HttpResponseMessage response = await myClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            using (Stream contentStream = await response.Content.ReadAsStreamAsync(),
                fileStream = new FileStream(savePath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true))
            {
                await contentStream.CopyToAsync(fileStream);
            }
            Debug.Log("Download and save successful.");
            return savePath;
        } 
        catch (Exception e)
        {
            Debug.LogError($"An error occurred: {e.Message}");
            return "Error";
        }

    }

    

    public async Task<string> PatchInitConversion(int arId)
    {
        var parameters = HttpUtility.ParseQueryString(string.Empty);
        parameters["user"] = player.GetId();
        parameters["ar_id"] = arId.ToString();

        string apiUrl = $"{devUrl}/initCon?{parameters}";

        // Request
        var request = new HttpRequestMessage(new HttpMethod("PATCH"), apiUrl);

        HttpResponseMessage response = await myClient.SendAsync(request);

        if (response.IsSuccessStatusCode)
        {
            return response.ReasonPhrase;
        }

        return "Error: Error initiating conversion.";
    }

    public async Task<LoginInfo> PostLogin(string username, string password)
    {
        string apiUrl = $"{ devUrl }/login";

        var jsonData = new Cred(username, password);
        var jsonContent = new StringContent(JsonConvert.SerializeObject(jsonData), Encoding.UTF8, "application/json");
        
        HttpResponseMessage response = await myClient.PostAsync(apiUrl, jsonContent);

        if (response.IsSuccessStatusCode)
        {
            string responseContent = await response.Content.ReadAsStringAsync();

            //Process response data
            return JsonConvert.DeserializeObject<LoginInfo>(responseContent);
        }

        else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized){
            return new LoginInfo(LoginInfo.Status.Incorrect);
        }

        return new LoginInfo(LoginInfo.Status.Failed);
    }

    public void CloseClient()
    {
        myClient.Dispose();
    }
}






