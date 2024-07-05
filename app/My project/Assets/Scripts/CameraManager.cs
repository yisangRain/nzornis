using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using OpenCvSharp;
#if UNITY_ANDROID
using UnityEngine.Android;
#endif

public class CameraManager : MonoBehaviour
{
    private WebCamTexture deviceCamera;
    private Texture defaultBackground;
    private bool camAvailable = false;
    private bool recording = false;
    private int i = 0;
    private VideoWriter videoWriter;

    public RawImage background;
    public Button recordButton;
    public AspectRatioFitter fit;
    public TextMeshProUGUI testText;
    public Player player;

    public int frameRate = 30;

    //AndroidJavaObject paramVal = new AndroidJavaClass("com.arthenica.mobileffmpeg.Signal").GetStatic<AndroidJavaObject>("SIGXCPU");

// Start is called before the first frame update
void Start()
    {
        //paramVal.CallStatic("ignoreSignal", new object[] { paramVal });
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

#if UNITY_EDITOR_WIN

        for (int i = 0; i < devices.Length; i++)
        {
            if (devices[i].isFrontFacing)
            {
                deviceCamera = new WebCamTexture(devices[i].name, Screen.width, Screen.height);
                camAvailable = true;
                return;
            }
        }
#endif
#if UNITY_ANDROID

        for (int i = 0; i < devices.Length; i++)
        {
            if (!devices[i].isFrontFacing)
            {
                deviceCamera = new WebCamTexture(devices[i].name, Screen.width, Screen.height);
                camAvailable = true;
            }
        }
#endif
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
        } else if (i >= 900)
        {
            testText.text = "Frame limit reached. Terminating recording.";
            RecordHandler();
        }
    }


    public void RecordHandler()
    {

        if (recording == false && camAvailable)
        {
            Debug.Log("Initiating recording");
            string currentDate = DateTime.Today.ToString("O");
            string filePath = $"{player.GetSavePath()}_10";
            testText.text = filePath;
            recordButton.GetComponentInChildren<TextMeshProUGUI>().text = "Stop Recording";
            videoWriter = new VideoWriter(filePath + ".mp4", VideoWriter.FourCC('H', '2', '6', '4'), frameRate, new Size(deviceCamera.width, deviceCamera.height), true);

            recording = true;
        }
        else if (recording == true)
        {
            recordButton.GetComponentInChildren<TextMeshProUGUI>().text = "Start Recording";
            videoWriter.Dispose();
            Debug.Log("Writer disposed.");
            recording = false;
            i = 0;
        }
    }

    
    public void Record()
    {
        Texture2D tex = new Texture2D(deviceCamera.width, deviceCamera.height, TextureFormat.RGB24, false);
        tex.SetPixels32(deviceCamera.GetPixels32());
        tex.Apply();
   
        byte[] imgData = tex.EncodeToJPG();

        Mat img = Cv2.ImDecode(imgData, ImreadModes.Unchanged);

        videoWriter.Write(img);
        Resources.UnloadUnusedAssets();
    }




}
