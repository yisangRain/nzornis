using TMPro;
using UnityEngine;

public class DemoMainManager : MonoBehaviour
{
    [SerializeField]
    TMP_Text demoInfo;

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
        } catch {
            demoInfo.text = "Game Manager is null";
        }
    }

}
