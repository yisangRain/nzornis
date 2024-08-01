using NUnit.Framework;
using UnityEngine;
using Moq;
using Moq.Protected;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using System.IO;

public class ServerTest
{
    Client myClient = new Client();
    Player player = Player.instance;

    // Test Vars
    private int testSightingId = 1;
    private string testTitle = "Test Title";
    private string testDesc = "Test description";
    private int testTime = Convert.ToInt32(DateTimeOffset.Now.ToUnixTimeSeconds());
    private double testLat = -43.52628;
    private double testLon = 172.58623;
    private string testFilename = "blob.mp4";

    // Test video
    byte[] testVideoBytes = File.ReadAllBytes("Assets/TestAssets/blob.mp4");

    [SetUp]
    public void SetUp()
    {
        Assert.IsTrue(player.TestLogIn() == "Log in successful");
    }

    [TearDown]
    public void TearDown()
    {
        Assert.IsTrue(player.LogOut() == "Logged out.");
    }

    // Vanilla tests below

    /// <summary>
    /// TDD development test to help with the development.
    /// Only tests vanilla (core) functionality
    /// </summary>
    [Test]
    public async void TestGetStatus_Vanilla()
    {
        var mockHttpHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        var mockPlayer = new Mock<IPlayer>();

        var response = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(@"{""sighting_id"": " + testSightingId.ToString() + @", ""status"": ""3"" }")
        };

        mockHttpHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response)
            .Verifiable();

        myClient.SetClient(new HttpClient(mockHttpHandler.Object));
        var testResponse = await myClient.GetStatus(testSightingId);

        Assert.AreEqual("3", testResponse);

        mockHttpHandler.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(req =>
                req.Method == HttpMethod.Get
                && req.RequestUri == new Uri("http://localhost:8000/getStatus?sighting_id=1")),
            ItExpr.IsAny<CancellationToken>()
            );
    }


    [Test]
    public async void TestGetMedia_Vanilla()
    {
        var mockHttpHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);

        var response = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new ByteArrayContent(testVideoBytes)
        };

        mockHttpHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response)
            .Verifiable();

        myClient.SetClient(new HttpClient(mockHttpHandler.Object));
        string testResponse = await myClient.GetMedia(testSightingId);
        Debug.Log(testResponse);

        Assert.IsTrue($"{Application.persistentDataPath}/ar_{testSightingId}.mp4" == testResponse);

        Assert.IsTrue(File.Exists(testResponse));
        File.Delete(testResponse);

        mockHttpHandler.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(req =>
                req.Method == HttpMethod.Get
                && req.RequestUri == new Uri("http://localhost:8000/getUserVideo?sighting_id=1")),
            ItExpr.IsAny<CancellationToken>()
            );
    }


    [Test]
    public async void TestPostUpload_Vanilla()
    {
        var mockHttpHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);

        var response = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.Created,
            ReasonPhrase = "Video successfully uploaded"
        };

        mockHttpHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response)
            .Verifiable();

        myClient.SetClient(new HttpClient(mockHttpHandler.Object));

        string testFilePath = "Assets/TestAssets/blob.mp4";
        UploadObject testJsonData = new UploadObject();
        testJsonData.title = testTitle;
        testJsonData.desc = testDesc;
        testJsonData.time = testTime;
        testJsonData.ToLatLn(testLat, testLon);
        testJsonData.filename = testFilename;

        var testResponse = await myClient.PostUpload(testFilePath, testJsonData);

        Assert.AreEqual("Video successfully uploaded", testResponse);

        mockHttpHandler.Protected().Verify(
          "SendAsync",
          Times.Once(),
          ItExpr.Is<HttpRequestMessage>(req =>
              req.RequestUri == new Uri("http://localhost:8000/upload?user=100")),
          ItExpr.IsAny<CancellationToken>()
          );
    }

    [Test]
    public async void TestPatchInitConversion_Vanilla()
    {
        var mockHttpHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);

        var response = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.Accepted,
            ReasonPhrase = "Initiating"
        };

        mockHttpHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response)
            .Verifiable();

        myClient.SetClient(new HttpClient(mockHttpHandler.Object));
        var testResponse = await myClient.PatchInitConversion(testSightingId);

        Assert.AreEqual("Initiating", testResponse);

        mockHttpHandler.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(req =>
                req.RequestUri == new Uri("http://localhost:8000/initiateConversion?sighting_id=1")),
            ItExpr.IsAny<CancellationToken>()
            );
    }




}

