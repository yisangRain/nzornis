using System;
using System.Collections.Generic;
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
        Segmentation,
        Novotel,
        University
    }

    private demo demoLocation = demo.None; // To store the target setup
    private Exception demoNotSetException = new Exception("Demo location not set.");

    public Poi poi { get; set; }
    public Vector3 arPosition { get; set; }

    // For breadcrumbs
    private string prevSceneName = null;
    private string currentSceneName = null;

    // For carrying target Poi between different scens
    public Poi newPoi;

    // To store user-created PoIs for the immediate application instance. Erases upon application reset or closure.
    public List<Poi> addedPois = new List<Poi>();

    public bool newEntry = false;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);        
    }

    public void SetDemoLocation(demo newDemoLocation)
    {
        demoLocation = newDemoLocation;
    }

    public void Update()
    {
        if (SceneManager.GetActiveScene().name != currentSceneName)
        {
            prevSceneName = currentSceneName;
            currentSceneName = SceneManager.GetActiveScene().name;
            Debug.Log($"[Scene] Prev {prevSceneName}, Curr {currentSceneName}");
        }

        // Simple breadcrumbing. Needs better method if the scene navigation depth increases further than 3
        if (prevSceneName == "DemoCreate" || prevSceneName == "DemoPosition" || prevSceneName == "DemoAr")
        {
            prevSceneName = "DemoMain";
            Debug.Log("[Scene] Previous scene name switched to DemoMain");
        }
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
        SceneManager.LoadScene(sceneName);
    }

    /// <summary>
    /// Go back to the previous scene.
    /// If no previous scene, load Main.
    /// </summary>
    public void BackScene()
    {
        if (prevSceneName != null & currentSceneName != prevSceneName)
        {
            if (prevSceneName == "Landing")
            {
                LoadScene("DemoMain");
            } else
            {
                LoadScene(prevSceneName);
            }
            
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
