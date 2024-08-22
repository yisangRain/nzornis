using System;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Game Manager class to manage global operations within the Demo
/// Includes:
/// - Persisting variable storage
/// - Loading Main scene from Landing
/// </summary>
public class DemoGameManager : MonoBehaviour
{
    public enum demo
    {
        None,
        Novotel,
        University
    }

    private demo demoLocation = demo.None; // To store the target set of demo
    private Exception demoNotSetException = new Exception("Demo location not set.");

    public Poi poi { get; set; }
    public Vector3 arPosition { get; set; }

    private string prevSceneName = null;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void SetDemoLocation(demo newDemoLocation)
    {
        demoLocation = newDemoLocation;
    }

    public demo GetDemoLocation()
    {
        if (demoLocation == demo.None)
        {
            Debug.LogError("[GameManager] Demo location is None.");
            throw demoNotSetException;
        } 
        return demoLocation;
    }

    public void LoadScene(string sceneName)
    {
        prevSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(sceneName);
    }

    /// <summary>
    /// Go back to the previous scene.
    /// If no previous scene, load Main.
    /// </summary>
    public void BackScene()
    {
        if (prevSceneName != null & SceneManager.GetActiveScene().name != prevSceneName)
        {
            LoadScene(prevSceneName);
        } else
        {
            LoadScene("DemoMain");
        }
    }

    public void SetDemoAndLoadMainScene(string demoString)
    {
        if (demoString.Equals("novotel"))
        {
            SetDemoLocation(demo.Novotel);
        } else if (demoString.Equals("university"))
        {
            SetDemoLocation(demo.University);
        } else
        {
            throw demoNotSetException;
        }
        LoadScene("DemoMain");
    }
}
