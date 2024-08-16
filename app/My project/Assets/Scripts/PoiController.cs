using UnityEngine;
using TMPro;
using Niantic.Lightship.Maps.Core.Coordinates;

/// <summary>
/// Script attached to the POI prefab
/// </summary>
public class PoiController : MonoBehaviour
{
    public TMP_Text testText;
    public Poi poi;
    public GameObject panel;

    private DemoExpManager expController;
    private string prev;

    private void Update()
    {
        string current = transform.position.ToString();
        if (current != prev)
        {
            prev = current;
            Debug.Log($"Position of the {name}: {current}");
            testText.text = current;
            Debug.Log($"GPS position: {poi.latlng}");


        }
    }

    public void OnMouseUpAsButton()
    {
        testText.text = "Clicked";
        panel.SetActive(true);
        expController = GameObject.Find("Explorer Controller").GetComponent<DemoExpManager>();
        expController.UpdateVars(poi.title, poi.description, poi.filepath);
    }
}
