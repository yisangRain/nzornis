using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Niantic.Lightship.Maps.MapLayers.Components;
using Niantic.Lightship.Maps.Core.Coordinates;
using Niantic.Lightship.Maps;


public class ExplorerController : MonoBehaviour
{
    [SerializeField]
    private LayerGameObjectPlacement _CubeGOP;

    public Button testButton;

    // Start is called before the first frame update
    void Start()
    {
        testButton.onClick.AddListener(OnClickPlaceObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void PlaceObject(LatLng pos, string objName)
    {
        _CubeGOP.PlaceInstance(pos, objName);
    }

    public void OnClickPlaceObject()
    {
        var gpsData = Input.location.lastData;
        PlaceObject(new LatLng(gpsData.latitude, gpsData.longitude), "test location");
    }

}
