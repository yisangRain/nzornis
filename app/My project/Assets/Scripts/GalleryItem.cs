using UnityEngine;

public class GalleryItem : MonoBehaviour
{
    public Poi poi;

    public DemoGalleryController galleryController;

    public void OnMouseDown()
    {
        galleryController.SetCurrentPoi(poi);
    }
}
