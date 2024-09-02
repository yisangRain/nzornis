using System.Collections;
using Niantic.Lightship.Maps.Core.Coordinates;
using UnityEngine;
using UnityEngine.Android;

/// <summary>
/// Class to access the device's GPS and return the current coordinate
/// 
/// </summary>
public class LocationTracker : MonoBehaviour
{
    private LatLng currentCoord = new LatLng();
    private double lastMapUpdateTime;
    private static bool IsLocationServiceInitializing
      => Input.location.status == LocationServiceStatus.Initializing;

    private void Start()
    {
        Debug.Log("[Location Tracker] Initiating");
        StartCoroutine(CurrentLocation());
    }

    private IEnumerator CurrentLocation()
    {
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermission(Permission.FineLocation);
        }

        if (!Input.location.isEnabledByUser)
        {
            Debug.Log("[LocationTracker] Location service is not enabled.");
            yield break;
        }

        Input.location.Start();

        int maxWait = 20;
        while (IsLocationServiceInitializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        if (maxWait < 1)
        {
            Debug.Log("[LocationTracker] GPS initialisation timed out: 20 secodns.");
            yield break;
        }

        if (Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.Log("[LocationTracker] Location service failed to connect.");
            yield break;
        }

        while (isActiveAndEnabled)
        {
            var gpsInfo = Input.location.lastData;
            if(gpsInfo.timestamp > lastMapUpdateTime)
            {
                lastMapUpdateTime = gpsInfo.timestamp;
                currentCoord = new LatLng(gpsInfo.latitude, gpsInfo.longitude);
            }

            yield return null;

        }

        Input.location.Stop();
    }

    public LatLng GetCoord()
    {
        return currentCoord;
    }
}
