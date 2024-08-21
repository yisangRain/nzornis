
using UnityEngine;

public class Centerer : MonoBehaviour
{
    private Vector3 origin = new Vector3(0, 0, 0);
    // Start is called before the first frame updat

    // Update is called once per frame
    void Update()
    {
        if (transform.position != origin)
        {
            transform.position = origin;
        }
    }
}
