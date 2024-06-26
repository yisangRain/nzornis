using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Moq;

public class ServerTest
{

    private Client client;

    [SetUp]
    public void SetUp()
    {
        // set up mock requests...or intercepts
    }

    [UnityTest]
    public IEnumerator ClientTestPostRequest()
    {
        // check post request sent out
        yield return null;
    }



    
    // A Test behaves as an ordinary method
    [Test]
    public void ServerTestSimplePasses()
    {
        // Use the Assert class to test conditions
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator ServerTestWithEnumeratorPasses()
    {
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        yield return null;
    }
}
