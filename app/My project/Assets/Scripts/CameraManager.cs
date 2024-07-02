using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class CameraManager : MonoBehaviour
{
    private WebCamTexture deviceCamera;
    private Texture defaultBackground;
    private bool camAvailable = false;
    private bool recording = false;
    private int i = 0;

    public RawImage background;
    public Button recordButton;
    public AspectRatioFitter fit;
    public TextMeshProUGUI testText;


    // Start is called before the first frame update
    void Start()
    {
        OpenCamera();
        recordButton.onClick.AddListener(RecordHandler);
    }

    public void OpenCamera()
    {
        defaultBackground = background.texture;
        WebCamDevice[] devices = WebCamTexture.devices;

        if (devices.Length == 0)
        {
            Debug.Log("No camera detected.");
            return;
        }

        for (int i = 0; i < devices.Length; i++)
        {
            if (!devices[i].isFrontFacing)
            {
                deviceCamera = new WebCamTexture(devices[i].name, Screen.width, Screen.height);
                camAvailable = true;
            }
        }

        if (deviceCamera == null)
        {
            Debug.Log("Unable to find back-facing camera.");
            return;
        }

        deviceCamera.Play();
        background.texture = deviceCamera;
    }

    // Update is called once per frame
    void Update()
    {
        if (!camAvailable)
        {
            return;
        }

        float ratio = (float)deviceCamera.width / (float)deviceCamera.height;
        fit.aspectRatio = ratio;

        float scaleY = deviceCamera.videoVerticallyMirrored ? -1f : 1f;
        background.rectTransform.localScale = new Vector3(1f, scaleY, 1f);

        int orientation = -deviceCamera.videoRotationAngle;
        background.rectTransform.localEulerAngles = new Vector3(0, 0, orientation);

        if (recording == true && i < 900)
        {
            Record();
            i += 1;
        }
    }


    public void RecordHandler()
    {
        if (recording == false)
        {
            recording = true;
        }
        if (recording == true)
        {
            recording = false;
            i = 0;
        }
    }
    
    public void Record()
    {
        

        // save the image as a video

        // add the video path to the Player

    } 


}
