using UnityEngine;
using TMPro;

/// <summary>
/// Script attached to the POI prefab
/// </summary>
public class PoiController : MonoBehaviour
{
    public TMP_Text testText;
    public Poi poi;
    public GameObject panel;

    private DemoExpManager expController;

    public void OnMouseUpAsButton()
    {
        testText.text = "Clicked";
        panel.SetActive(true);
        expController = GameObject.Find("Explorer Controller").GetComponent<DemoExpManager>();
        expController.UpdateVars(poi.title, poi.description, poi.filepath);
    }
}
