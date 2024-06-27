using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;
using UnityEngine;

public interface IPlayer
{
    string GetId();

}

public class Player : MonoBehaviour, IPlayer
{
    private Player() {}

    private static Player _instance;

    private static readonly object _lock = new object();

    private bool loggedIn = false;
    private string playerId;
    private string savePath;

    //Dev variables
    private string testId = "100";
    private string testPassword = "test_password";

    public static Player GetInstance()
    {
        if (_instance == null)
        {
            lock(_lock)
            {
                if (_instance == null)
                {
                    GameObject backUpPlayer = new GameObject();
                    _instance = backUpPlayer.AddComponent<Player>();

                }
            }
        }
        return _instance;
    }

    // Start is called before the first frame update
    void Start()
    {
        GetInstance();
        savePath = Application.persistentDataPath;
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

    public string LogOut()
    {
        if (loggedIn == true)
        {
            loggedIn = false;
            playerId = null;
            return "Logged out.";
        }

        return "Cannot log out. Not logged in.";
    }

    public string GetId()
    {
        return playerId;
    }

    public string GetSavePath()
    {
        return savePath;
    }
}
