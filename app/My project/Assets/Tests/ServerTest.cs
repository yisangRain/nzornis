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
    int testArId = 1;
    string testUser = "100";

    // Test video
    byte[] testVideoBytes = File.ReadAllBytes("Assets/TestAssets/blob.mp4");




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
            Content = new StringContent(@"{""ar_id"": " + testArId.ToString() + @", ""status"": ""processing"" ")
        };

        mockPlayer.Setup(s => s.GetId()).Returns(testUser);

        mockHttpHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response)
            .Verifiable();

        var testResponse = await myClient.GetStatus(testArId);

        Assert.AreEqual("processing", testResponse);

        mockHttpHandler.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(req =>
                req.Method == HttpMethod.Get
                && req.RequestUri == new Uri("localhost:8000/getStatus?user=100&ar_id=1")
        ));
    }


    [Test]
    public async void TestGetUserVideo_Vanilla()
    {
        var mockHttpHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        var mockPlayer = new Mock<IPlayer>();

        var response = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new ByteArrayContent(testVideoBytes)
        };

        mockPlayer.Setup(s => s.GetId()).Returns(testUser);

        mockHttpHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response)
            .Verifiable();

        var testResponse = await myClient.GetUserVideo(testArId);

        Assert.IsTrue(testResponse == testVideoBytes);

        mockHttpHandler.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(req =>
                req.Method == HttpMethod.Get
                && req.RequestUri == new Uri("localhost:8000/getUserVideo?user=100&ar_id=1")
        ));
    }







}
