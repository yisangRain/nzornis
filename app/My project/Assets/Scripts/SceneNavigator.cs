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
        Debug.Log($"Loading scene: {sceneName}");
        SceneManager.LoadScene(sceneName);
    }

}
