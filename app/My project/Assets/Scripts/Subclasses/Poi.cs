using Niantic.Lightship.Maps.Core.Coordinates;
using UnityEngine;
using System;

/// <summary>
/// DTO for POI information
/// </summary>
public class Poi
{
    public string user { get; set; }
    public string title { get; set; }
    public string description { get; set; } 
    public int clipId { get; set; }
    public LatLng latlng { get; set; }
    public DateTime date { get; set; }
    public Vector3 position { get; set; }

    enum clipOrder {
        KERERU,
        KOKAKO,
        PUKEKO,
        HUMMINGBIRD,
        BLOB
        }

    public Poi()
    {

    }

    public Poi(LatLng latlng)
    {
        this.latlng = latlng;
    }

    public Poi(string title, string description, int clipId, LatLng latlng, string user)
    {
        this.title = title;
        this.description = description;
        this.clipId = clipId;
        this.latlng = latlng;
        date = DateTime.Now;
        this.user = user;
    }

    public Poi(string title, string description, int clipId, LatLng latlng, string user, DateTime date, Vector3 position)
    {
        this.title = title;
        this.description = description;
        this.clipId = clipId;
        this.latlng = latlng;
        this.date = date;
        this.user = user;
        this.position = position;
    }
}
