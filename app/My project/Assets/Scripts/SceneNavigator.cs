//using UnityEngine;
//using UnityEngine.SceneManagement;
//using System;

///// <summary>
///// Script to control scene navigation
///// </summary>
//public class SceneNavigator : MonoBehaviour
//{
//    public Carrier carrier;

//    private string prevScene;

//    public void Start()
//    {
//        if (carrier == null)
//        {
//            try
//            {
//                carrier = GameObject.Find("Carrier").GetComponent<Carrier>();
//            } catch (Exception e)
//            {
//                Debug.LogError("[Scene Navigator] Carrier was not found: " + e.ToString());
//            }
//        }

//        if (prevScene != null)
//        {
//            //UnloadUnusedScene();
//        }
        
//    }

//    /// <summary>
//    /// Loads the home scene (Main)
//    /// </summary>
//    public void LoadHome()
//    {
//        Cleaner();
//        prevScene = SceneManager.GetActiveScene().name;
//        SceneManager.LoadScene("Main");
//        Debug.Log("Home scene loaded");       
//    }

//    /// <summary>
//    /// Load given scene
//    /// </summary>
//    /// <param name="sceneName">Target scene name</param>
//    public void LoadScene(string sceneName)
//    {
//        Cleaner();
//        prevScene = SceneManager.GetActiveScene().name;
//        Debug.Log($"Loading scene: {sceneName}");
//        SceneManager.LoadScene(sceneName);
//    }

//    /// <summary>
//    /// Demo scene navigator
//    /// </summary>
//    /// <param name="demoLoc"></param>
//    /// <param name="sceneName"></param>
//    public void LoadDemoMain(string demoLoc)
//    {
//        Cleaner();
//        carrier.payload = demoLoc;
//        Debug.Log($"[Demo Initiation] Loading demo main scene set to {demoLoc}");
//        prevScene = SceneManager.GetActiveScene().name;
//        SceneManager.LoadScene("DemoMain");
//    }


//    public void UnloadUnusedScene()
//    {
//        SceneManager.SetActiveScene(SceneManager.GetActiveScene());
//        SceneManager.UnloadSceneAsync(prevScene);
//    }

//    /// <summary>
//    /// Destroys empty automatically spawned carry-over gameobjects
//    /// </summary>
//    private void Cleaner()
//    {
//        GameObject[] x = FindObjectsByType<GameObject>(FindObjectsSortMode.InstanceID);
//        foreach (GameObject o in x)
//        {
//            if ((o.name == "Player" || o.name == "Carrier") && o.transform.childCount == 0)
//            {
//                Destroy(o);
//            }
//        }
        
//    }

//}
