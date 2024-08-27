using UnityEngine;
using TMPro;
using UnityEngine.Video;
using Niantic.Lightship.AR.LocationAR;

/// <summary>
/// AR scene manager for Demo
/// 
/// </summary>
public class DemoArManager : MonoBehaviour
{
    [SerializeField]
    private TMP_Text titleText;

    [SerializeField]
    private GameObject placeholderObject;

    DemoGameManager gameManager;

    [SerializeField]
    private VideoClip demo0;
    [SerializeField]
    private VideoClip demo1;
    [SerializeField]
    private VideoClip demo2;
    [SerializeField]
    private VideoClip demo3;
    [SerializeField]
    private VideoClip demo4;

    // Start is called before the first frame update
    void Start()
    {
        if (Application.isEditor)
        {
            titleText.text = "Editor: This scene only.";
            placeholderObject.GetComponent<VideoPlayer>().clip = demo1;
        } else
        {
            gameManager = GameObject.Find("GameManager").GetComponent<DemoGameManager>();

            titleText.text = gameManager.poi.title;
            switch (gameManager.poi.clipId)
            {
                case 0:
                    placeholderObject.GetComponent<VideoPlayer>().clip = demo0;
                    break;
                case 1:
                    placeholderObject.GetComponent<VideoPlayer>().clip = demo1;
                    break;
                case 2:
                    placeholderObject.GetComponent<VideoPlayer>().clip = demo2;
                    break;
                case 3:
                    placeholderObject.GetComponent<VideoPlayer>().clip = demo3;
                    break;

                default:
                    placeholderObject.GetComponent<VideoPlayer>().clip = demo4;
                    break;
            }

            placeholderObject.transform.position = gameManager.poi.position;

        }
    }
}
