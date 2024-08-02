using UnityEngine;

public interface IPlayer
{
    string GetId();

}

public class Player : MonoBehaviour, IPlayer
{

    public static Player instance { get; private set; }

    private static readonly object _lock = new object();

    private bool loggedIn = false;
    private string playerId;
    public int outputNum = 0;

    //Dev variables
    private string testId = "100";

    public void Start()
    {
        Initiate();
        DontDestroyOnLoad(gameObject);

    }

    private void Initiate()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }

 
    /// <summary>
    /// DRAFT: a simple (unsecure) log-in method as a placeholder
    /// Treat as blackbox method
    /// </summary>
    /// <returns>String message of outcome</returns>
    public string TestLogIn()
    {
    
        loggedIn = true;
        playerId = testId;
        return "Test Account: Log in successful";

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

    public bool PlayerLoginUpdate(string newUserId)
    {
        if (newUserId != "Fail")
        {
            playerId = newUserId;
            loggedIn = true;
        }

        return loggedIn;
    }

    public Player GetPlayer()
    {
        Initiate();
        return instance;
    }

}
