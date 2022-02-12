using MongoDB.Driver;
using Demo.DotNetWithMongoDB.Models;

namespace Demo.DotNetWithMongoDB.Infra;

public class MongoDBConnection
{
    public const string CONNECTION_STRING = "mongodb://admin:password@localhost:27017";
    public const string DATABASE = "db-geo";
    public const string COLLECTION = "airports";

    private static readonly IMongoClient _client;
    private static readonly IMongoDatabase _database;

    static MongoDBConnection()
    {
        _client = new MongoClient(CONNECTION_STRING);
        _database = _client.GetDatabase(DATABASE);
    }

    public IMongoClient Client
    {
        get { return _client; }
    }

    public IMongoCollection<AirportModel> Airports
    {
        get { return _database.GetCollection<AirportModel>(COLLECTION); }
    }
}