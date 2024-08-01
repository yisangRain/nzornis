using Niantic.Lightship.Maps.Core.Coordinates;


public class UploadObject
{
    public string title { get; set; }
    public string desc { get; set; }
    public int time { get; set; }
    public LatLng latLon { get; set; }
    public string filename { get; set; }

    public void ToLatLn(double lat, double lon)
    {
        latLon = new LatLng(lat, lon);
    }

}
