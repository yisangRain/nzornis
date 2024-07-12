using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.IO;
using System.Threading.Tasks;
#if UNITY_ANDROID
using UnityEngine.Android;
#endif

/// <summary>
/// Camera manager script to control and manage device camera input to display and then encoding the collected textures into a video.
/// </summary>
public class CameraManager : MonoBehaviour
{
    private WebCamTexture deviceCamera;
    private Texture defaultBackground;
    private bool camAvailable = false;
    private bool recording = false;
    private int i = 0;
    private int j = 0;

    public RawImage background;
    public Button recordButton;
    public AspectRatioFitter fit;
    public TextMeshProUGUI testText;
    public Player player;

    public int frameRate = 30;


    /// <summary>
    /// Called at start of the scene before the first frame.
    /// Asks for the user permission for storage read/write and camera if it is not given.
    /// </summary>
    void Start()
    {
        OpenCamera();
        recordButton.onClick.AddListener(RecordHandler);

        if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            Permission.RequestUserPermission(Permission.Camera);
        }
        if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite))
        {
            Permission.RequestUserPermission(Permission.ExternalStorageWrite);
        }
        if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead))
        {
            Permission.RequestUserPermission(Permission.ExternalStorageRead);
        }
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


    public async void RecordHandler()
    {

        if (recording == false && camAvailable)
        {
            recordButton.GetComponentInChildren<TextMeshProUGUI>().text = "Stop Recording";
            recording = true;
            if(testText.text.Length < 10)
            {
                testText.text = Application.persistentDataPath;
            }
            Debug.Log("Initiating recording using FFMPEG");

        } else if (recording == true)
        {
            recordButton.GetComponentInChildren<TextMeshProUGUI>().text = "Processing";
            recording = false;
            i = 0;
            recordButton.enabled = false;

            await FfmpegRecord();
            recordButton.GetComponentInChildren<TextMeshProUGUI>().text = "Start recording";
            recordButton.enabled = true;
        }

    }

    public void Record()
    {
        try
        {
            Texture2D tex = new Texture2D(deviceCamera.width, deviceCamera.height, TextureFormat.RGB24, false);
            tex.SetPixels32(deviceCamera.GetPixels32());
            tex.Apply();
            byte[] imgData = tex.EncodeToJPG();
            Destroy(tex);

            if (j < 10)
            {
                File.WriteAllBytes(Path.Combine(Application.persistentDataPath, "00" + j.ToString() + ".jpg"), imgData);
            } else if (j < 100)
            {
                File.WriteAllBytes(Path.Combine(Application.persistentDataPath, "0" + j.ToString() + ".jpg"), imgData);
            } else
            {
                File.WriteAllBytes(Path.Combine(Application.persistentDataPath, j.ToString() + ".jpg"), imgData);
            }
        
            j += 1;
            Debug.Log($"Recorded scene: {j}");

        } catch(Exception e)
        {
            Debug.Log(e);
        }

    }

    public async Task FfmpegRecord()
    {
        var path = Application.persistentDataPath;
        AndroidJavaClass jc = new AndroidJavaClass("com.arthenica.mobileffmpeg.FFmpeg");
        jc.CallStatic<int>("execute", new object[] { $"-framerate 30 -i {path}/%03d.jpg {path}/output_{player.outputNum}.mp4 -y" });
        Debug.Log("Video conversion complete");

        for (int k = 0; k <= j; k++)
        {
            if (k < 10)
            {
                File.Delete($"{ path }/00{k}.jpg");
            }
            else if (k < 100)
            {
                File.Delete($"{ path }/0{k}.jpg");
            }
            else
            {
                File.Delete($"{ path }/{k}.jpg");
            }
        }

        Debug.Log("Deleted all captures");

        jc.Dispose();
        player.outputNum += 1;
        j = 0;

        await Task.Delay(2000);

    }



}
