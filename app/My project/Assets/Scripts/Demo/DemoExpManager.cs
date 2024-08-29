using Niantic.Lightship.Maps.Core.Coordinates;
using Niantic.Lightship.Maps.MapLayers.Components;
using System;
using System.Collections.Generic;
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

    // Current location
    [SerializeField]
    private DemoPlayerLocCon locCon;
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

    //Spawned pois
    private List<Poi> spawnedPois = new List<Poi>();
    private List<string> spawnedNames = new List<string>();

    // Distance threshold
    public double interactionLimit = 0.0001; //0.0001 is 11.1m

    // Distant check timing
    public DateTime timing = DateTime.Now;


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

            SpawnNew();
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
            if (Application.isEditor)
            {
                demoInfo.text = "Demo: Editor";
            }
        }

        // Deactivate POI panel to hide it.
        poiPanel.SetActive(false);

        SpawnDemo();
        DistanceChecker();
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
        Poi n1 = new Poi("Test title 1", "Lorem ipsum", 0, new LatLng(float.Parse("-43.530406"), float.Parse("172.637542")), "Alice Baker", new DateTime(2024, 1, 1), new Vector3(0,0,0));
        Poi n2 = new Poi("Test title 2", "Lorem ipsum", 1, new LatLng(float.Parse("-43.530411"), float.Parse("172.637813")), "Alice Baker", new DateTime(2024, 2, 21), new Vector3(10,5,10));
        Poi n3 = new Poi("Test title 3", "Lorem ipsum", 2, new LatLng(float.Parse("-43.530440"), float.Parse("172.637654")), "Chris Donovan", new DateTime(2023, 12, 1), new Vector3(5,5,5));

        // uni demos
        Poi u1 = new Poi("Lab 1", "Slightly off from the front door", 0, new LatLng(float.Parse("-43.520448"), float.Parse("172.583186")), "Ethan Fong", new DateTime(2024, 1, 1), new Vector3(0,0,0));
        Poi u2 = new Poi("Lab 2", "I think this is around my desk", 1, new LatLng(float.Parse("-43.520579"), float.Parse("172.583286")), "Gareth Han", new DateTime(2024, 3, 10), new Vector3(3,3,3));
        Poi u3 = new Poi("Lab 3", "Around Stephan's office", 2, new LatLng(float.Parse("-43.520612"), float.Parse("172.583128")), "Ines Capri", new DateTime(2024, 5, 5), new Vector3(10,5, 0));
        Poi u4 = new Poi("Somewhere by the fire station", "This is where firetrucks live.", 3, new LatLng(float.Parse("-43.520216"), float.Parse("172.582620")), "Ines Capri", new DateTime(2024, 8,1), new Vector3(0,0,0));

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

            // add into the tracker lists
            spawnedPois.Add(loc);
            spawnedNames.Add("demo" + i.ToString());

            i++;
        }
        Debug.Log("[SpawnDemo] Finished.");
    }

    /// <summary>
    /// Spawn pois created within this demo instance
    /// </summary>
    public void SpawnNew()
    {
        int i = 0;

        using (IEnumerator<Poi> penum = gameManager.addedPois.GetEnumerator())
        {
            while (penum.MoveNext())
            {
                Poi p = penum.Current;
                poiGOP.PlaceInstance(p.latlng, "new" + i.ToString());
                PoiController temp = GameObject.Find("new" + i.ToString()).GetComponent<PoiController>();
                temp.poi = p;

                Debug.Log($"[SpawnNew] new poi {i} created");

                i++;
            }
            Debug.Log("[SpawnNew] Finished.");
        }
    }


    public void Update()
    {
      
        if (DateTime.Now.Subtract(timing).Seconds > 5)
        {
            timing = DateTime.Now;
            DistanceChecker();
        }

        
    }

    /// <summary>
    /// Check euclidean distance between the user's current location and each pois
    /// </summary>
    public void DistanceChecker()
    {
        LatLng current = locCon.currentCoordinate;

        int i = 0;

        Debug.Log("[DistanceChecker] iterating");
        // iterate through each Pois currently on the scene. 
        using (IEnumerator<Poi> poiEnum = spawnedPois.GetEnumerator())
        {
            while (poiEnum.MoveNext())
            {
                Poi p = poiEnum.Current;
                double d = Math.Sqrt(Math.Pow(p.latlng.Latitude - current.Latitude, 2) + Math.Pow(p.latlng.Longitude - current.Longitude, 2));

                if (d > interactionLimit)
                {
                    GameObject sp = GameObject.Find(spawnedNames[i]);
                    sp.GetComponent<MeshRenderer>()
                        .material
                        .color = Color.gray;
                    sp.GetComponent<PoiController>()
                        .active = false;
                }
                if (d <= interactionLimit)
                {
                    GameObject sp = GameObject.Find(spawnedNames[i]);
                    sp.GetComponent<MeshRenderer>()
                        .material
                        .color = Color.blue;
                    sp.GetComponent<PoiController>()
                        .active = true;
                }
                i++;
            }
        }   
    }
}
