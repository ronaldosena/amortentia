using backend.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace backend.Data
{
  public class DataContext
  {
    private readonly IMongoDatabase _database = null;

    public DataContext(IOptions<Settings> settings)
    {
      var client = new MongoClient(settings.Value.ConnectionString);
      if (client != null)
        _database = client.GetDatabase(settings.Value.Database);
    }

    public IMongoCollection<User> Users
    {
      get
      {
        return _database.GetCollection<User>("Users");
      }
    }
  }
}
