using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Demo.DotNetWithMongoDB.Models;
using Demo.DotNetWithMongoDB.Util;
using System.Net;
using Demo.DotNetWithMongoDB.Infra;
using MongoDB.Driver.GeoJsonObjectModel;
using MongoDB.Driver;

namespace Demo.DotNetWithMongoDB.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public async Task<JsonResult> Search(HomeViewModel model)
    {
        if (model is null)
            throw new ArgumentNullException(nameof(model));

        Debug.WriteLine(model);

        //Captura a posição atual e adiciona a lista de pontos
        CoordinateModel coordLocal = GetCoordinates(model.Address);

        var nearAirports = new List<CoordinateModel>();
        nearAirports.Add(coordLocal);

        //Captura a latitude e longitude locais
        double lat = Convert.ToDouble(coordLocal.Latitude.Replace(".", ","));
        double lon = Convert.ToDouble(coordLocal.Longitude.Replace(".", ","));

        //Testa o tipo de aeroporto que será usado na consulta
        string airportType = "";

        if (model.Type == AirportType.International)
        {
            airportType = "International";
        }
        if (model.Type == AirportType.Municipal)
        {
            airportType = "Municipal";
        }

        //Captura o valor da distancia
        int distance = model.Distance * 1000;

        //Conecta MongoDB  
        var dBConnection = new MongoDBConnection();

        //Configura o ponto atual no mapa   
        var point = new GeoJson2DGeographicCoordinates(lon, lat);
        var localization = new GeoJsonPoint<GeoJson2DGeographicCoordinates>(point);

        // filtro
        var construtor = Builders<AirportModel>.Filter;
        FilterDefinition<AirportModel> filtro_builder;

        if (airportType == "")
        {
            filtro_builder = construtor.NearSphere(x => x.loc, localization, distance);
        }
        else
        {
            filtro_builder = construtor.NearSphere(x => x.loc, localization, distance) & construtor.Eq(x => x.type, airportType);
        };

        // Captura a lista
        var airportList = await dBConnection.Airports.Find(filtro_builder).ToListAsync();
        
        // Escreve os pontos
        foreach (var doc in airportList)
        {
            var aero = new CoordinateModel(doc.name,
                            Convert.ToString(doc.loc.Coordinates.Latitude).Replace(",", "."),
                            Convert.ToString(doc.loc.Coordinates.Longitude).Replace(",", "."));

            nearAirports.Add(aero);
        }

        return Json(nearAirports);
    }

    private CoordinateModel GetCoordinates(string address)
    {
        string url = $"http://maps.google.com/maps/api/geocode/json?address={address}";
        Debug.WriteLine(url);

        var coord = new CoordinateModel("Não Localizado", "-10", "-10");
        var response = new WebClient().DownloadString(url);
        var googleGeocode = Newtonsoft.Json.JsonConvert.DeserializeObject<GoogleGeocodeResponse>(response);


        if (googleGeocode.status == "OK")
        {
            coord.Name = googleGeocode.results[0].formatted_address;
            coord.Latitude = googleGeocode.results[0].geometry.location.lat;
            coord.Longitude = googleGeocode.results[0].geometry.location.lng;
        }

        return coord;
    }
}
