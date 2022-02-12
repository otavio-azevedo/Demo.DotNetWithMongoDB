namespace Demo.DotNetWithMongoDB.Models;

public class CoordinateModel
{
    public string Name { get; set; }
    public string Latitude { get; set; }
    public string Longitude { get; set; }

    public CoordinateModel(string name, string lat, string lng)
    {
        Name = name;
        Latitude = lat;
        Longitude = lng;
    }
}
