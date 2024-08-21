using Niantic.Lightship.Maps.Core.Coordinates;
using System;

/// <summary>
/// DTO for POI information
/// </summary>
public class Poi
{
    public string user { get; set; }
    public string title { get; set; }
    public string description { get; set; } 
    public string filepath { get; set; }
    public LatLng latlng { get; set; }
    public DateTime date { get; set; }

    public Poi(string title, string description, string filepath, LatLng latlng, string user)
    {
        this.title = title;
        this.description = description;
        this.filepath = filepath;
        this.latlng = latlng;
        date = DateTime.Now;
        this.user = user;
    }

    public Poi(string title, string description, string filepath, LatLng latlng, string user, DateTime date)
    {
        this.title = title;
        this.description = description;
        this.filepath = filepath;
        this.latlng = latlng;
        this.date = date;
        this.user = user;
    }
}
