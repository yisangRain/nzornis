using UnityEngine;
using UnityEngine.SceneManagement;

public class DemoPositionController : MonoBehaviour
{
    [SerializeField]
    private GameObject exampleModel;

    private DemoGameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        try
        {
            gameManager = GameObject.Find("GameManager").GetComponent<DemoGameManager>();
        } catch
        {
            Debug.Log("[DemoPosition] No game manager found.");
        }
    }

    public void MoveModel(string direction)
    {
        Vector3 delta;
        switch (direction)
        {
            case "up":
                delta = new Vector3(0, 0.1f, 0);
                break;
            case "down":
                delta = new Vector3(0, -0.1f, 0);
                break;
            case "left":
                delta = new Vector3(-0.1f, 0, 0);
                break;
            case "right":
                delta = new Vector3(0.1f, 0, 0);
                break;
            case "forward":
                delta = new Vector3(0, 0, 0.1f);
                break;
            case "backward":
                delta = new Vector3(0, 0, -0.1f);
                break;
            default:
                delta = new Vector3(0, 0, 0);
                break;
        }
        exampleModel.transform.position += delta;
    }

    public void ConfirmPosition()
    {
        if (gameManager != null & gameManager.newPoi != null)
        {
            gameManager.newPoi.position = exampleModel.transform.position;
            gameManager.BackScene();
        } else
        {
            SceneManager.LoadScene("DemoCreate");
        }
    }

    public void ExitScene()
    {
        if (gameManager != null)
        {
            gameManager.BackScene();
        }
        else
        {
            SceneManager.LoadScene("DemoCreate");
        }
    }
}
