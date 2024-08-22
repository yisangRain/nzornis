using Niantic.Lightship.Maps.Core.Coordinates;
using Niantic.Lightship.Maps.MapLayers.Components;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DemoExpManager : MonoBehaviour
{
    // GameManager 
    [SerializeField]
    private TMP_Text demoInfo;
    private DemoGameManager gameManager;
    private DemoGameManager.demo demoLocation;

    // POI subscene
    [SerializeField]
    private GameObject poiPanel;
    [SerializeField]
    private Button closePoiButton;
    [SerializeField]
    private TMP_Text titleText;
    [SerializeField]
    private TMP_Text userText;
    [SerializeField]
    private TMP_Text dateText;
    [SerializeField]
    private TMP_Text descriptionText;
    
    // POI object
    public Poi currentPoi;

    //POI demo
    [SerializeField]
    private LayerGameObjectPlacement poiGOP;
    private Poi[] demos;


    // Start is called before the first frame update
    void Start()
    {
        // Get GameManager
        try
        {
            gameManager = GameObject.Find("GameManager").GetComponent<DemoGameManager>();
            demoLocation = gameManager.GetDemoLocation();
            switch (demoLocation)
            {
                case DemoGameManager.demo.Novotel:
                    demoInfo.text = "Demo: Novotel";
                    break;

                case DemoGameManager.demo.University:
                    demoInfo.text = "Demo: University";
                    break;

                default:
                    break;
            }
        }
        catch (Exception e)
        {
            if (Application.isEditor)
            {
                demoInfo.text = "Demo: Editor";
            }
        }

        // Deactivate POI panel to hide it.
        poiPanel.SetActive(false);

        SpawnDemo();

    }
    
    public void ClosePoiPanel()
    {
        poiPanel.SetActive(false);
    }

    public void SetPoi(Poi newPoi)
    {
        currentPoi = newPoi;
        titleText.text = currentPoi.title;
        userText.text = currentPoi.user;
        dateText.text = currentPoi.date.ToLocalTime().ToString();
        descriptionText.text = currentPoi.description;
        poiPanel.SetActive(true);
    }

    public void LoadAR()
    {
        gameManager.poi = currentPoi;
        SceneManager.LoadScene("DemoAr");
    }

    private void SpawnDemo()
    {
        demos = new Poi[] { };

        // novotel demos
        Poi n1 = new Poi("Test title 1", "Lorem ipsum", 1, new LatLng(float.Parse("-43.530406"), float.Parse("172.637542")), "Alice Baker", new DateTime(2024, 1, 1));
        Poi n2 = new Poi("Test title 2", "Lorem ipsum", 1, new LatLng(float.Parse("-43.530411"), float.Parse("172.637813")), "Alice Baker", new DateTime(2024, 2, 21));
        Poi n3 = new Poi("Test title 3", "Lorem ipsum", 1, new LatLng(float.Parse("-43.530440"), float.Parse("172.637654")), "Chris Donovan", new DateTime(2023, 12, 1));

        // uni demos
        Poi u1 = new Poi("Lab 1", "Slightly off from the front door", 0, new LatLng(float.Parse("-43.520448"), float.Parse("172.583186")), "Ethan Fong", new DateTime(2024, 1, 1));
        Poi u2 = new Poi("Lab 2", "I think this is around my desk", 1, new LatLng(float.Parse("-43.520579"), float.Parse("172.583286")), "Gareth Han", new DateTime(2024, 3, 10));
        Poi u3 = new Poi("Lab 3", "Around Stephan's office", 2, new LatLng(float.Parse("-43.520612"), float.Parse("172.583128")), "Ines Capri", new DateTime(2024, 5, 5));
        Poi u4 = new Poi("Somewhere by the fire station", "This is where firetrucks live.", 3, new LatLng(float.Parse("-43.520216"), float.Parse("172.582620")), "Ines Capri", new DateTime(2024, 8,1));

        try
        {
            if (Application.isEditor)
            {
                Poi[] tempDemoEditor = { u1, u2, u3, u4 };
                demos = tempDemoEditor;
            }
            else
            {
                switch (gameManager.GetDemoLocation())
                {
                    case DemoGameManager.demo.Novotel:
                        Poi[] tempDemo1 = { n1, n2, n3 };
                        demos = tempDemo1;
                        break;

                    default:
                        Poi[] tempDemo2 = { u1, u2, u3, u4 };
                        demos = tempDemo2;
                        break;
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("[SpawnDemo] could not find demo location: " + e.ToString());
            Poi[] editorVar = { u1, u2, u3 };
            demos = editorVar;
        }

        int i = 0;

        foreach (Poi loc in demos)
        {
            poiGOP.PlaceInstance(loc.latlng, "demo" + i.ToString());
            PoiController temp = GameObject.Find("demo" + i.ToString()).GetComponent<PoiController>();
            temp.poi = loc;
            

            Debug.Log($"[SpawnDemo] demo {i} created");

            i++;
        }
        Debug.Log("[SpawnDemo] Finished.");
    }

}
