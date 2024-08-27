using System;
using System.Collections;
using Niantic.Lightship.Maps.Core.Coordinates;
using Niantic.Lightship.Maps;
using UnityEngine;
using UnityEngine.Android;

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

    //[SerializeField]
    //private LayerGameObjectPlacement poiGOP;

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

    private const float WalkThreshold = 0.5f;
    private const float TeleportThreshold = 200f;

    private static bool IsLocationServiceInitializing
        => Input.location.status == LocationServiceStatus.Initializing;

    //private Poi[] demos;
    //private bool demoReady = false;

    // Game Manager
    //DemoGameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        mapView.MapOriginChanged += OnMapViewOriginChanged;
        currentPosition = targetPosition = transform.position;

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
        while (IsLocationServiceInitializing && maxWait > 0)
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
}
