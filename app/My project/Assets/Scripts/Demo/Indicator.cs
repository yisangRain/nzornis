using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Indicator : MonoBehaviour
{
    // Start is called before the first frame update
    GameObject parent;
    string grandParent;

    private void Start()
    {
        parent = GameObject.Find("MapLayers");
        grandParent = parent.transform.parent.name;
        Debug.Log("starting position: " + parent.transform.position.ToString());
    }

    // Update is called once per frame
    void Update()
    {

        if (parent.transform.parent.name != grandParent)
        {

            Debug.Log("No grandparent to speak of.");
            GameObject temp = GameObject.Find(grandParent);
            parent.SetActive(true);
            parent.transform.parent = temp.transform;

            Debug.Log("reset position: " + parent.transform.position.ToString());
        }
        
    }
}
