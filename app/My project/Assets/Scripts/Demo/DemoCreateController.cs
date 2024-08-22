using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    private DemoGameManager gameManager;

    private int videoSelected;
    private Vector3 positionSelected;
    private Poi poiCreated;

    // Start is called before the first frame update
    void Start()
    {
        try
        {
            gameManager = GameObject.Find("GameManager").GetComponent<DemoGameManager>();

        } catch
        {
            
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GoBack()
    {
        gameManager.BackScene();
    }

    public void SelectVideo()
    {

    }


}
