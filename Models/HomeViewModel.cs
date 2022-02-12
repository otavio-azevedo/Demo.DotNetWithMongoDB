using System.ComponentModel.DataAnnotations;

namespace Demo.DotNetWithMongoDB.Models;

public enum AirportType
{
    All,
    Municipal,
    International
}

public class HomeViewModel
{
    [Required]
    public string Address { get; set; }
    public int Distance { get; set; }
    public AirportType Type { get; set; }

    public override string ToString()
    {
        return $"Address: {Address}, Distance: {Distance}, Type: {Type}.";
    }
}