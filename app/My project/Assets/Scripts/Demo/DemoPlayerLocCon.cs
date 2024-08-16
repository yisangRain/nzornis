using System;
using System.Collections;
using Niantic.Lightship.Maps.Core.Coordinates;
using Niantic.Lightship.Maps.MapLayers.Components;
using Niantic.Lightship.Maps;
using UnityEngine;
using UnityEngine.Android;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// Adapted from 'PlayerLocationController.cs' script from Niantic 
/// namespace Niantic.Lightship.Maps.SampleAssets.Player
/// 
/// Demo-only script
/// To be used for showcase demo. Not for production.
/// </summary>
public class DemoPlayerLocCon : MonoBehaviour
{
    [SerializeField]
    private LightshipMapView mapView;

    [SerializeField]
    private Camera myCamera;

    public GameObject poiPanel;

    // Using a simple polygon shape as a player for now
    [SerializeField]
    public PlayerController playerModel;

    [SerializeField]
    private LayerGameObjectPlacement _CubeGOP;

    [SerializeField]
    private GameObject mapLayer;

    [SerializeField]
    private GameObject map;

    private double lastGpsUpdateTime;
    private Vector3 targetPosition;
    private Vector3 currentPosition;
    private double lastMapUpdateTime;

    public LatLng currentCoordinate = new LatLng();

    /// <summary>
    /// Event to alert any issues with the GPS system within the app
    /// </summary>
    public Action<string> OnGpsError;
    public TMP_Text testText;

    private const float WalkThreshold = 0.5f;
    private const float TeleportThreshold = 200f;

    private static bool IsLocationServiceInitializing
        => Input.location.status == LocationServiceStatus.Initializing;

    private Poi[] demos;
 
    private bool spawned = false;

    public Button testButton;

    // Start is called before the first frame update
    void Start()
    {
        testButton.onClick.AddListener(OnClickPlaceObject);
        mapView.MapOriginChanged += OnMapViewOriginChanged;
        currentPosition = targetPosition = transform.position;

        if (Application.isEditor)
        {
            SpawnDemo();
            spawned = true;
        }

        StartCoroutine(UpdateGpsLocation());

    }

    private void OnMapViewOriginChanged(LatLng center)
    {
        var offset = targetPosition - currentPosition;
        currentPosition = mapView.LatLngToScene(center);
        targetPosition = currentPosition + offset;
    }

    /// <summary>
    /// For Android playback of the app
    /// Asks for permission and uses the device's GPS data to update the player object's current position
    /// </summary>
    /// <returns></returns>
    private IEnumerator UpdateGpsLocation()
    {
        yield return null;

        // Request location permission
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermission(Permission.FineLocation);
            while (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
            {
                // wait until permission acquired
                yield return new WaitForSeconds(1);
            }
        }
        
        // Check if location service is enabled
        if (!Input.location.isEnabledByUser)
        {
            OnGpsError?.Invoke("Location service not enabled.");
            yield break;
        }

        // Start the location service
        Input.location.Start();

        // Wait until location service initialisation
        int maxWait = 20;
        while(IsLocationServiceInitializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        // if timeout, cancel location service use
        if (maxWait < 1)
        {
            OnGpsError?.Invoke("GPS initialisation timed out: 20 seconds");
            yield break;
        }

        // if connection failed, also cancel location service use
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            OnGpsError?.Invoke("Location service failed to connect");
            yield break;
        }

        while (isActiveAndEnabled)
        {
            var gpsInfo = Input.location.lastData;
            if (gpsInfo.timestamp > lastGpsUpdateTime)
            {
                lastGpsUpdateTime = gpsInfo.timestamp;
                currentCoordinate = new LatLng(gpsInfo.latitude, gpsInfo.longitude);
                testText.text = currentCoordinate.ToString();
                UpdatePlayerLocation(currentCoordinate);
            }

            yield return null;
        }

        Input.location.Stop();

    }

    /// <summary>
    /// Update player's current position. Teleports if very far.
    /// </summary>
    /// <param name="location"></param>
    private void UpdatePlayerLocation(in LatLng location)
    {

        targetPosition = mapView.LatLngToScene(location);

    }

    
    // Update is called once per frame
    void Update()
    {
        if (spawned == false && currentCoordinate != (new LatLng()) )
        {
            spawned = true;
            SpawnDemo();
            Debug.Log("Spawned demo with active GPS");

        }

        UpdateMapViewPosition();

        var movementVector = targetPosition - currentPosition;
        var moveDistance = movementVector.magnitude;

        switch (moveDistance)
        {
            case > TeleportThreshold:
                currentPosition = targetPosition;
                break;

            case > WalkThreshold:
                {
                    var forward = movementVector.normalized;
                    var rotation = Quaternion.LookRotation(forward, Vector3.up);
                    transform.rotation = rotation;
                    break;
                }
        }

        currentPosition = Vector3.Lerp(
            currentPosition,
            targetPosition,
            Time.deltaTime);

        transform.position = currentPosition;
        playerModel.UpdatePlayerState(moveDistance);
    }

    public void DestroyPoi()
    {
        int i = 0;
        foreach (Poi p in demos)
        {
            Destroy(GameObject.Find("demos" + i));
            i++;
        }
    }

    private void UpdateMapViewPosition()
    {
        // only update periodically to avoid spamming
        if (Time.time < lastMapUpdateTime + 1.0f)
        {
            return;
        }

        lastMapUpdateTime = Time.time;

        // update map view based on the player location
        mapView.SetMapCenter(transform.position);
    }

    /// <summary>
    /// Demo POI object spawner
    /// </summary>
    private void SpawnDemo()
    {
        demos = new Poi[] { };

        // novotel demos
        Poi n1 = new Poi("Test title 1", "Lorem ipsum", "Assets/TestAssets/blob.mp4", new LatLng(-43.530406, 172.637542));
        Poi n2 = new Poi("Test title 2", "Lorem ipsum", "Assets/TestAssets/blob.mp4", new LatLng(-43.530411, 172.637813));
        Poi n3 = new Poi("Test title 3", "Lorem ipsum", "Assets/TestAssets/blob.mp4", new LatLng(-43.530440, 172.637654));

        // uni demos
        Poi u1 = new Poi("Lab 1", "Slightly off from the front door", "Assets/TestAssets/blob.mp4", new LatLng(-43.523523, 172.585451));
        Poi u2 = new Poi("Lab 2", "I think this is around my desk", "Assets/TestAssets/blob.mp4", new LatLng(-43.523579, 172.585286));
        Poi u3 = new Poi("Lab 3", "Around Stephan's office", "Assets/TestAssets/testPukeko.mp4", new LatLng(-43.523612, 172.585128));

        try
        {
            string payload = GameObject.Find("Carrier").GetComponent<Carrier>().payload;

            if (payload == "novotel")
            {
                Poi[] tempDemo = { n1, n2, n3};
                demos = tempDemo;

            } else if (payload == "uni")
            {
                Poi[] tempDemo = { u1, u2, u3 };
                demos = tempDemo;
            }

        } catch (Exception e)
        {
            Debug.LogError("[SpawnDemo] payload issue: " + e.ToString());
            Poi[] editorVar = { u1, u2, u3 };
            demos = editorVar;
        }


        int i = 0;

        foreach (Poi loc in demos)
        {
            PlaceObject(loc.latlng, "demo" + i.ToString());

            PoiController temp = GameObject.Find("demo" + i.ToString()).GetComponent<PoiController>();
            temp.testText = testText;
            temp.panel = poiPanel;
            temp.poi = loc;

            i++;
        }
        testText.text = "Spawned";
        Debug.Log("[SpawnDemo] Finished.");
    }

    private void PlaceObject(LatLng pos, string objName)
    {
        _CubeGOP.PlaceInstance(pos, objName);
    }

    public void OnClickPlaceObject()
    {
        if (currentCoordinate.Equals(new LatLng()))
        {
            testText.text = "Location service not ready. Please try again later";
            Debug.Log("Location not ready");
        }
        else
        {
            //PlaceObject(currentCoordinate, "test location");
            SpawnDemo();
            Debug.Log("Should have spawned");
        }


    }
}
