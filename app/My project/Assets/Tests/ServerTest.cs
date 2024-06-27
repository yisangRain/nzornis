using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
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
    private int testArId = 1;
    private string testUser = "100";
    private string testPassword = "test_password";
    Player player = Player.GetInstance();

    // Test video
    byte[] testVideoBytes = File.ReadAllBytes("Assets/TestAssets/blob.mp4");

    [SetUp]
    public void SetUp()
    {
        Assert.IsTrue(player.LogIn(testUser, testPassword) == "Log in successful");
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
            Content = new StringContent(@"{""ar_id"": " + testArId.ToString() + @", ""status"": ""processing"" }")
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
        var testResponse = await myClient.GetStatus(testArId);

        Assert.AreEqual("processing", testResponse);

        mockHttpHandler.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(req =>
                req.Method == HttpMethod.Get
                && req.RequestUri == new Uri("http://localhost:8000/getStatus?user=100&ar_id=1")),
            ItExpr.IsAny<CancellationToken>()
            );
    }


    [Test]
    public async void TestGetUserVideo_Vanilla()
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
        string testResponse = await myClient.GetUserVideo(testArId);
        Debug.Log(testResponse);

        Assert.IsTrue($"{player.GetSavePath()}/ar_{testArId}.mp4" == testResponse);

        Assert.IsTrue(File.Exists(testResponse));
        File.Delete(testResponse);

        mockHttpHandler.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(req =>
                req.Method == HttpMethod.Get
                && req.RequestUri == new Uri("http://localhost:8000/getUserVideo?user=100&ar_id=1")),
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
        var testResponse = await myClient.PatchInitConversion(testArId);

        Assert.AreEqual("Initiating", testResponse);

        mockHttpHandler.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(req =>
                req.RequestUri == new Uri("http://localhost:8000/initCon?user=100&ar_id=1")),
            ItExpr.IsAny<CancellationToken>()
            );
    }




}
