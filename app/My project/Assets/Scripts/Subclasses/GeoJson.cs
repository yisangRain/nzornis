public class GeoJson
{
    public string type { get; set; }
    public Geometry geometry { get; set; }
    public Properties properties { get; set; }

    public GeoJson(string newType, Geometry geo, Properties prop)
    {
        type = newType;
        geometry = geo;
        properties = prop;
    }

    public GeoJson(Geometry geo, Properties prop)
    {
        geometry = geo;
        properties = prop;
    }

    public GeoJson() { }
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

