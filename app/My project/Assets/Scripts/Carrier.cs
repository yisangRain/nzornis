using UnityEngine;

// For keeping a game object alive between scenes
public class Carrier : MonoBehaviour
{
    public string payload;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
}
