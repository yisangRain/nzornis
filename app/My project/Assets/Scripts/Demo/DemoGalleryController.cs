using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Niantic.Lightship.Maps.Core.Coordinates;
using System.Collections.Generic;

public class DemoGalleryController : MonoBehaviour
{
    [SerializeField]
    ScrollRect gallery;

    [SerializeField]
    GameObject itemPanel;

    [SerializeField]
    TMP_Text titleText;
    [SerializeField]
    TMP_Text dateText;
    [SerializeField]
    TMP_Text descText;
    [SerializeField]
    RectTransform contentRect;

    [SerializeField]
    Button galleryItem; //button prefab

    private DemoGameManager gameManager;
    private Vector3 position = new Vector3(0, -200, 0);

    private Poi currentPoi;


    // Start is called before the first frame update
    void Start()
    {
        Poi u3 = new Poi("Lab 3", "Around Stephan's office", 2, new LatLng(float.Parse("-43.520612"), float.Parse("172.583128")), "Ines Capri", new DateTime(2024, 5, 5), new Vector3(10, 5, 0));
        Poi u4 = new Poi("Somewhere by the fire station", "This is where firetrucks live.", 3, new LatLng(float.Parse("-43.520216"), float.Parse("172.582620")), "Ines Capri", new DateTime(2024, 8, 1), new Vector3(0, 0, 0));

        try
        {
            gameManager = GameObject.Find("GameManager").GetComponent<DemoGameManager>();
            gameManager.addedPois.Add(u3);
            gameManager.addedPois.Add(u4);
            Debug.Log("[GalleryController] Spawning gallery items.");

            int i = 0;

            using (IEnumerator<Poi> penum = gameManager.addedPois.GetEnumerator()) 
            {
                while (penum.MoveNext())
                {
                    Poi p = penum.Current;
                    Instantiate(galleryItem);
                    Button g = GameObject.Find("GalleryItem(Clone)").GetComponent<Button>();
                    g.name = $"galleryItem_{i}";
                    g.transform.SetParent(gallery.content);
                    g.GetComponent<RectTransform>().position = position;
                    g.GetComponentInChildren<TMP_Text>().text = p.title;

                    // attach listener
                    g.onClick.AddListener(delegate { SetCurrentPoi(p); });
                    
                    contentRect.sizeDelta += new Vector2(0, 150);
                    position += new Vector3(0, -200, 0);
                    Debug.Log($"[GalleryController] Spawned {p.title} as galleryItem_{i}");
                    i++;
                }
            } 

        } catch (Exception e)
        {
            Debug.Log($"[DemoGalleryController] Issues {e}");
        }

        itemPanel.SetActive(false);
    }

    public void PanelExitButtonClicked()
    {
        itemPanel.SetActive(false);
    }

    public void GoBackButtonClicked()
    {
        if (gameManager != null)
        {
            gameManager.BackScene();
        } else
        {
            Debug.Log("[GalleryController] No GameManager detected. Loading Main Scene instead.");
            SceneManager.LoadScene("DemoMain");
        }
    }

    public void PlayButtonClicked()
    {
        if (gameManager != null)
        {
            gameManager.poi = currentPoi;
        } else
        {
            Debug.Log("[GalleryController] No Game Manager detected. Loading AR scene without setting AR target.");
        }

        SceneManager.LoadScene("DemoAr");
    }

    public void SetCurrentPoi(Poi targetPoi)
    {
        currentPoi = targetPoi;
        titleText.text = currentPoi.title;
        dateText.text = currentPoi.date.ToLocalTime().ToLongDateString();
        descText.text = currentPoi.description;
        OpenPanel();
    }

    public void OpenPanel()
    {
        itemPanel.SetActive(true);
    }

}
