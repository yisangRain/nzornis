using UnityEngine;

/// <summary>
/// Script attached to the POI prefab
/// </summary>
public class PoiController : MonoBehaviour
{
    public Poi poi;

    private DemoExpManager expController;

    public void OnMouseUpAsButton()
    {
        expController = GameObject.Find("Explorer Controller").GetComponent<DemoExpManager>();
        expController.SetPoi(poi);
    }
}
