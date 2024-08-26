using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DemoGalleryController : MonoBehaviour
{
    [SerializeField]
    ScrollRect gallery;

    DemoGameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        try
        {
            gameManager = GameObject.Find("GameManager").GetComponent<DemoGameManager>();
        } catch
        {
            Debug.Log("[DemoGalleryController] No Game Manager detected.");
        }
    }


}
