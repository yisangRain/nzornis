using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationTeller : MonoBehaviour
{
    private Vector3 curr;
    public string name;
    // Start is called before the first frame update
    void Start()
    {
        curr = transform.position;
        Debug.Log($"[{name}] Current position: {curr}");
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position != curr)
        {
            curr = transform.position;
            Debug.Log($"[{name}] Current position: {curr}");
        }
    }
}
