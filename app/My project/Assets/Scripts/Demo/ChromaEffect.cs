using System.Collections;
using UnityEngine;

/// <summary>
/// For rainbow color shift effect
/// </summary>
public class ChromaEffect : MonoBehaviour
{
    private Material material;
    private Color32[] colors;
    // Start is called before the first frame update
    void Start()
    {
        material = transform.GetComponent<MeshRenderer>().material;
        colors = new Color32[]
        {
            new Color32(255, 0, 0, 255), // Red
            new Color32(255, 165, 0, 255), //orange
            new Color32(255, 255, 0, 255), //yellow
            new Color32(0, 255, 0, 255), //green
            new Color32(0, 0, 255, 255), //blue
            new Color32(75, 0, 130, 255), //indigo
            new Color32(238, 130, 238, 255), //violet
        };
        StartCoroutine(ColorCycle());
    }

    private IEnumerator ColorCycle()
    {
        int i = 0;
        while (true)
        {
            for (float j = 0f; j < 1f; j += 0.001f)
            {
                material.color = Color.Lerp(colors[i%7], colors[(i + 1) % 7], j);
                yield return null;
            }
            i++;
        }
    }
}
