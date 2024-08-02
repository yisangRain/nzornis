using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Script to control scene navigation
/// </summary>
public class SceneNavigator : MonoBehaviour
{

    /// <summary>
    /// Loads the home scene (Main)
    /// </summary>
    public void LoadHome()
    {
        Debug.Log("Loading Home scene");
        SceneManager.LoadScene("Main");
    }

    /// <summary>
    /// Load given scene
    /// </summary>
    /// <param name="sceneName">Target scene name</param>
    public void LoadScene(string sceneName)
    {
        Cleaner();
        Debug.Log($"Loading scene: {sceneName}");
        SceneManager.LoadScene(sceneName);
    }

    /// <summary>
    /// Destroys empty automatically spawned player gameobjects
    /// </summary>
    private void Cleaner()
    {
        GameObject[] x = FindObjectsByType<GameObject>(FindObjectsSortMode.InstanceID);
        foreach (GameObject o in x)
        {
            if (o.name == "Player" && o.transform.childCount == 0)
            {
                Destroy(o);
            }
        }
        
    }

}
