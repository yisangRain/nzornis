using UnityEngine;
using System.Net.Http;
using System;
using Newtonsoft.Json;
using UnityEngine.Android;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using Niantic.Lightship.Maps.Core.Coordinates;

public interface IFilming
{

}


enum UploadType
{
    RAW,
    AR,
    NULL
}


/// <summary>
/// Script to control the Filming scene
/// </summary>
public class FilmingManager : MonoBehaviour
{
    public Player player;

    public TMP_Text testText;
    public GameObject uploadPanel;
    public GameObject formPanel;
    public TMP_InputField title;
    public TMP_InputField desc;

    private Client client = new Client();
    private string targetPath;
    private UploadType uploadType = UploadType.NULL;

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
            // open and pick media from gallery
            PickVideo();

            // process selection steps
            uploadPanel.SetActive(true);

        } catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    /// <summary>
    /// Opens device gallery and returns user's selected video path
    /// </summary>
    /// <returns>String path of the selected video. Throws exception if the path is null</returns>
    public void PickVideo()
    {
        NativeGallery.Permission permission = NativeGallery.GetVideoFromGallery((path) =>
        {
            Debug.Log($"Video path: {path}");
            targetPath = path;
            testText.text = targetPath;

        }, "Please select a video to convert to AR");

        Debug.Log($"Permision result: {permission}"); 

        if (targetPath == null)
        {
            throw new ApplicationException("Null video path");
        }

    }

    

    /// <summary>
    /// Queries the device's GPS for the current latitude and longitude
    /// </summary>
    /// <returns>string[] {latitude, longitude}</returns>
    public LatLng GetCurrentLocation()
    {
        // check for location service enabled or not
        if (!Input.location.isEnabledByUser)
        {
            Debug.Log("Location permission not enabled");
            throw null;
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
            throw null;
        }

        var loc = Input.location.lastData;
        LatLng latlng = new LatLng(loc.latitude, loc.longitude);
        Input.location.Stop();

        return latlng;

    }

    public void HandleRawButton()
    {
        formPanel.SetActive(true);
        uploadType = UploadType.RAW;
        uploadPanel.SetActive(false);
    }


    public void HandleConversionButton()
    {
        formPanel.SetActive(true);
        uploadType = UploadType.AR;
        uploadPanel.SetActive(false);
    }

    public UploadObject UploadFormMaker()
    {
        UploadObject uploadObject = new UploadObject();
        uploadObject.title = title.text;
        uploadObject.desc = desc.text;
        uploadObject.latLon = GetCurrentLocation();
        uploadObject.time = Convert.ToInt32(DateTimeOffset.Now.ToUnixTimeSeconds());
        return uploadObject;
    }

    public void HandleUpload()
    {
        UploadObject data = UploadFormMaker();
        client.SetClient(new HttpClient());

        if (uploadType == UploadType.RAW)
        {

        } else if (uploadType == UploadType.AR)
        {

        }

    }

}
