using UnityEngine;
using System.Net.Http;
using System;
using Newtonsoft.Json;
using UnityEngine.Android;
using System.Collections;

public interface IFilming
{

}


/// <summary>
/// Script to control the Filming scene
/// </summary>
public class FilmingManager : MonoBehaviour
{
    public Player player;

    private Client client = new Client();

    private static bool IsLocationServiceInitializing
            => Input.location.status == LocationServiceStatus.Initializing;

    /// <summary>
    /// Request for Location permission on awake
    /// </summary>
    public void Awake()
    {
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermission(Permission.FineLocation);
        }
    }

    /// <summary>
    /// Opens device camera app
    /// </summary>
    public void OpenCamera()
    {
        if (NativeCamera.IsCameraBusy())
        {
            Debug.Log("Camera is already open.");
            return;
        }

        RecordVideo();
    }

    /// <summary>
    /// Record video using the device camera application;
    /// </summary>
    public void RecordVideo()
    {
        NativeCamera.Permission permission = NativeCamera.RecordVideo((path) =>
        {
            Debug.Log($"Video path: {path}");
        }, NativeCamera.Quality.High, 60);
        Debug.Log($"Permisson result: {permission}");
    }

    /// <summary>
    /// Opens device gallery for the user to select the target video to convert to an AR instance.
    /// Saves the path into the local variable.
    /// Then calls ConversionHandler to handle the conversion process
    /// </summary>
    public void SelectVideo()
    {
        // Check if the gallery is open
        if (NativeGallery.IsMediaPickerBusy())
        {
            Debug.Log("Device gallery is already open.");
            return;
        }

        try
        {
            // open gallery
            var targetPath = PickVideo();

            // process conversion steps
            ConversionHandler(targetPath);

        } catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    /// <summary>
    /// Opens device gallery and returns user's selected video path
    /// </summary>
    /// <returns>String path of the selected video. Throws exception if the path is null</returns>
    public string PickVideo()
    {
        string targetPath = null;

        NativeGallery.Permission permission = NativeGallery.GetVideoFromGallery((path) =>
        {
            Debug.Log($"Video path: {path}");
            targetPath = path;

        }, "Please select a video to convert to AR");

        Debug.Log($"Permision result: {permission}"); 

        if (targetPath != null)
        {
            return targetPath;
        }

        throw new ApplicationException("Null video path");
        
    }

    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="targetPath"></param>
    public void ConversionHandler(string targetPath)
    {
        // Assign location
        var locationString = GetCurrentLocation();
        if (locationString == null)
        {
            Debug.Log("Issue with location string");
            return;
        }

        // Assign xyz position within the AR space


        // Ask for final confirmation

        // Send HTTP request (Post and Patch)
        client.SetClient(new HttpClient());

        // Inform user that the conversion is in progress
    }

    /// <summary>
    /// Queries the device's GPS for the current latitude and longitude
    /// </summary>
    /// <returns>string[] {latitude, longitude}</returns>
    public string[] GetCurrentLocation()
    {
        // check for location service enabled or not
        if (!Input.location.isEnabledByUser)
        {
            Debug.Log("Location permission not enabled");
            return null;
        }

        Input.location.Start();

        int maxWait = 20;
        while (IsLocationServiceInitializing && maxWait > 0)
        {
            new WaitForSeconds(1);
            maxWait--;
        }

        if (maxWait < 1 || Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.Log("Location service initialization issue: Timeout");
            return null;
        }

        var loc = Input.location.lastData;
        string[] latlng = { loc.latitude.ToString(), loc.latitude.ToString() };

        Input.location.Stop();

        return latlng;

    }

}
