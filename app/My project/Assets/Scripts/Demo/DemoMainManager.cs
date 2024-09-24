using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DemoMainManager : MonoBehaviour
{
    [SerializeField]
    TMP_Text demoInfo;

    [SerializeField]
    TMP_Text newText;

    DemoGameManager gameManager;
    DemoGameManager.demo demoLocation;

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

            if (gameManager.newEntry)
            {
                newText.text = "New!";
            } else
            {
                newText.text = "";
            }

        } catch {
            demoInfo.text = "Game Manager is null";
        }
    }

    private void Update()
    {
        if (gameManager != null)
        {

        }
    }

}
