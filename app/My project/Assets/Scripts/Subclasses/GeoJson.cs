using UnityEditor;
using UnityEngine;


public class GeoJson
{
    public string type { get; set; }
    public Geometry geometry { get; set; }
    public Properties properties { get; set; }
} 

public class Geometry
{

    public string type { get; set; }
    public double[] coordinates { get; set; }

    public Geometry(string newType, double[] newCoordinates)
    {
        type = newType;
        coordinates = newCoordinates;
    }

}

public class Properties
{
    public string name { get; set; }

    public Properties (string newName)
    {
        name = newName;
    }
}

