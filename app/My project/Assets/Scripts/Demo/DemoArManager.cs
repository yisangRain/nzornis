using UnityEngine;
using TMPro;
using UnityEngine.Video;

/// <summary>
/// AR scene manager for Demo
/// 
/// </summary>
public class DemoArManager : MonoBehaviour
{
    public TMP_Text titleText;
    private DemoExpManager exp;
    public GameObject placeholder;

    // Start is called before the first frame update
    void Start()
    {
        CleanUpExp();
    
        if ( GameObject.Find("Explorer Controller") != null)
        {
            exp = GameObject.Find("Explorer Controller").GetComponent<DemoExpManager>();
            titleText.text = exp.title;
            placeholder.GetComponent<VideoPlayer>().url = exp.filepath;

        } else
        {
            titleText.text = "AR not loaded";
            Debug.LogError("[DemoArManager] AR data not passed on from Explorer scene");
            placeholder.GetComponent<VideoPlayer>().url = "Assets/TestAssets/blob.mp4";
        }


    }


    private void CleanUpExp()
    {
        GameObject[] x = FindObjectsByType<GameObject>(FindObjectsSortMode.InstanceID);
        foreach (GameObject o in x)
        {
            if (o.name == "Explorer Controller" && o.transform.childCount == 0)
            {
                Destroy(o);
            }
        }

    }

}
