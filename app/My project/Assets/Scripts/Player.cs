using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Player() {}

    private static Player _instance;

    private static readonly object _lock = new object();

    private bool loggedIn = false;
    private string playerId;

    //Dev variables
    private string testId = "test_id";
    private string testPassword = "test_password";

    public static Player GetInstance()
    {
        if (_instance == null)
        {
            lock(_lock)
            {
                if (_instance == null)
                {
                    _instance = new Player();
                }
            }
        }
        return _instance;
    }

    // Start is called before the first frame update
    void Start()
    {
        GetInstance();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// DRAFT: a simple (unsecure) log-in method as a placeholder
    /// Treat as blackbox method
    /// </summary>
    /// <param name="id"></param>
    /// <param name="password"></param>
    /// <returns>String message of outcome</returns>
    public string LogIn(string id, string password)
    {
        if (id == testId && password == testPassword)
        {
            loggedIn = true;
            playerId = testId;
            return "Log in successful";
        }
        else
        {
            return "Log in failed";
        }
    }

    public string GetId()
    {
        return playerId;
    }
}
