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
        CleanUp();
    
        if ( GameObject.Find("Explorer Controller") != null)
        {
            exp = GameObject.Find("Explorer Controller").GetComponent<DemoExpManager>();
            titleText = exp.titleText;
            LoadAr();

        } else
        {
            titleText.text = "AR not loaded";
            Debug.LogError("[DemoArManager] AR data not passed on from Explorer scene");
        }


    }


    private void CleanUp()
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

    public void LoadAr()
    {
        placeholder.GetComponent<VideoPlayer>().url = exp.filepath;
    }
}
