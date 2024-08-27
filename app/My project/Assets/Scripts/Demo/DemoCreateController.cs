using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Niantic.Lightship.Maps.Core.Coordinates;

public class DemoCreateController : MonoBehaviour
{
    [SerializeField]
    Button selectButton;
    [SerializeField]
    Button positionButton;
    [SerializeField]
    Button descButton;
    [SerializeField]
    Button uploadButton;

    [SerializeField]
    TMP_Text instText;

    [SerializeField]
    GameObject selectPanel;

    [SerializeField]
    GameObject descPanel;

    [SerializeField]
    TMP_InputField titleInput;
    [SerializeField]
    TMP_InputField uploaderInput;
    [SerializeField]
    TMP_InputField descInput;


    private DemoGameManager gameManager;
    [SerializeField]
    private LocationTracker locationTracker;

    private Poi poiCreated = new Poi();

    // Start is called before the first frame update
    void Start()
    {
        try
        {
            gameManager = GameObject.Find("GameManager").GetComponent<DemoGameManager>();
            if (gameManager.newPoi != null)
            {
                selectButton.interactable = false;
                positionButton.interactable = false;
                descButton.interactable = true;
                instText.text = "Please add title and description.";
            }

        } catch
        {
            Debug.Log("GameManager not detected.");
        }

        selectPanel.SetActive(false);
        descPanel.SetActive(false);
    }

    public void GoBack()
    {
        if (gameManager != null)
        {
            gameManager.BackScene();
        } else
        {
            Debug.Log("[Create Scene] Game Manager does not exist. Loading Main scene");
            SceneManager.LoadScene("DemoMain");
        }
        
    }

    public void SelectVideoButtonClicked()
    {
        selectPanel.SetActive(true);
    }

    public void SelectDemoVideo(string birdName)
    {
        switch (birdName)
        {
            case "Kereru":
                poiCreated.clipId = 0;
                break;

            case "Kokako":
                poiCreated.clipId = 1;
                break;

            case "Pukeko":
                poiCreated.clipId = 2;
                break;

            case "Hummingbird":
                poiCreated.clipId = 3;
                break;

            default:
                poiCreated.clipId = 4;
                break;
        }
        selectPanel.SetActive(false);
        selectButton.interactable = false;
        positionButton.interactable = true;
        instText.text = $"{birdName} selected. Please position the bird.";
    }

    public void PositionButtonClicked()
    {
        if (gameManager != null)
        {
            gameManager.newPoi = poiCreated;
            gameManager.LoadScene("DemoPosition");
        } else
        {
            Debug.Log("[Create Scene] Game Manager does not exist. Loading DemoPosition scene");
            SceneManager.LoadScene("DemoPosition");
        }  
    }

    public void DescButtonClicked()
    {
        descPanel.SetActive(true);
    }

    public void DescExitButtonClicked()
    {
        descPanel.SetActive(false);
    }

    public void DescCompleteButtonClicked()
    {
        string title = titleInput.text;
        string uploader = uploaderInput.text;
        string desc = descInput.text;

        if (gameManager != null)
        {
            gameManager.newPoi.title = title;
            gameManager.newPoi.user = uploader;
            gameManager.newPoi.description = desc;
        }

        descPanel.SetActive(false);
        descButton.interactable = false;
        uploadButton.interactable = true;
        instText.text = "Press 'Upload' to confirm upload.";
    }

    public void UploadButtonClicked()
    {
        if (gameManager != null)
        {
            LatLng currentLocation = locationTracker.GetCoord();
            DateTime currentTime = DateTime.Now;
            Poi newPoi = gameManager.newPoi;
            newPoi.date = currentTime;
            newPoi.latlng = currentLocation;
            gameManager.addedPois.Add(newPoi);
            gameManager.newPoi = null;
            gameManager.LoadScene("DemoMain");

        } else
        {
            SceneManager.LoadScene("DemoMain");
            Debug.Log("[DemoCreateController] No Game Manager detected. Navigating to Main Scene instead.");
        }
    }


}
