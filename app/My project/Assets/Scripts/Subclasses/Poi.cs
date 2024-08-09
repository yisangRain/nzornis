using Niantic.Lightship.Maps.Core.Coordinates;

/// <summary>
/// DTO for POI information
/// </summary>
public class Poi
{
    public string title { get; set; }
    public string description { get; set; } 
    public string filepath { get; set; }
    public LatLng latlng { get; set; }

    public Poi(string title, string description, string filepath, LatLng latlng)
    {
        this.title = title;
        this.description = description;
        this.filepath = filepath;
        this.latlng = latlng;
    }
}
