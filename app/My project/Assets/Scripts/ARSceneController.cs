using UnityEngine;
using UnityEngine.Android;

public class ARSceneController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // Request location permission

        Permission.RequestUserPermission(Permission.Camera);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
