using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DemoExpManager : MonoBehaviour
{
    public GameObject poiPanel;
    public Button closePoiButton;
    public TMP_Text titleText;
    public TMP_Text descriptionText;
    public string filepath;
    public Vector3 arPosition = new Vector3(0,0,15);

    public string title;
    public string desc;


    // Start is called before the first frame update
    void Start()
    {
        poiPanel.SetActive(false);
        DontDestroyOnLoad(this);
    }
    
    public void ClosePoiPanel()
    {
        poiPanel.SetActive(false);
    }

    public void UpdateVars(string newTitleText, string newDescriptionText, string filepathText)
    {
        titleText.text = newTitleText;
        title = newTitleText;
        descriptionText.text = newDescriptionText;
        desc = newDescriptionText;
        filepath = filepathText;

    }

    
}
