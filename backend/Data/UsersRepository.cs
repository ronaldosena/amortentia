using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using backend.Dtos;
using backend.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using backend.Helpers;

namespace backend.Data
{
  public class UsersRepository : IUsersRepository
  {
    private readonly IMapper _mapper;
    private readonly DataContext _context;
    public UsersRepository(IOptions<Settings> settings, IMapper mapper)
    {
      _mapper = mapper;
      _context = new DataContext(settings);
    }

    public async Task<bool> AddPhoto(string username, Photo photo)
    {
      var filter = Builders<User>.Filter.Eq(x => x.Username, username);
      var update = Builders<User>.Update.Push<Photo>(x => x.Photos, photo);
      var result = await _context.Users.FindOneAndUpdateAsync(filter, update);

      if (result != null)
        return true;
      return false;
    }

    public async Task<bool> AnyMainPhoto(string username)
    {
      // var filter = Builders<User>.Filter.Eq(x => x.Username, username) & 
      //              Builders<User>.Filter.AnyEq("Photos", new BsonDocument { { "IsMain", true } });
      // var result = await _context.Users.Find(filter).CountDocumentsAsync();

      var builder = Builders<User>.Filter;
      // var filter = Builders<User>.Filter.Eq(x => x.Username, username) & 
      //              Builders<User>.Filter.AnyEq("Photos", new BsonDocument { { "IsMain", true } });
      var filter = builder.And(builder.Eq("Username", username), builder.Eq("Photos.IsMain", true));
      var result = await _context.Users.Find(filter).AnyAsync();
      return result;
    }

    public async Task<bool> DeletePhoto(string username, string photoId)
    {
      var update = Builders<User>.Update.PullFilter(x => x.Photos, p => p.PublicId == photoId);
      // var result = await _context.Users.FindOneAndUpdateAsync(x => x.Username == username, update).Result;
      var result = await _context.Users.UpdateOneAsync(x => x.Username == username, update);

      return result.ModifiedCount > 0;
    }

    public async Task<Photo> GetPhoto(string username, string photoId)
    {
      var builder = Builders<User>.Filter;
      var filter = builder.And(builder.Eq("Username", username), builder.Eq("Photos.PublicId", photoId));

      // TODO: Use aggregate in the future to return only the photo
      var result = await _context.Users.Find(filter).FirstOrDefaultAsync();

      if (result != null)
      {
        return result.Photos.FirstOrDefault(x => x.PublicId == photoId);
      }
      return null;
    }

    public async Task<User> GetUser(string username)
    {
      var user = await _context.Users.Find(x => x.Username == username).FirstOrDefaultAsync();
      return user;
    }

    public async Task<PagedList<User>> GetUsers(UserParams userParams)
    {
      var builder = Builders<User>.Filter;
      var filter = builder.Where(x => x.Username != userParams.Username);
      var sort = Builders<User>.Sort.Ascending("Username");

      if (userParams.Gender == "Witch" || userParams.Gender == "Wizard")
        filter &= builder.Where(x => x.Gender == userParams.Gender);

      if (userParams.MaxAge != default(int))
      {
        var maxDoB = DateTime.Today.AddYears(-userParams.MaxAge - 1);
        filter &= builder.Gte(x => x.DateOfBirth, maxDoB);
      }

      if (userParams.MinAge != default(int))
      {
        var minDoB = DateTime.Today.AddYears(-userParams.MinAge - 1);
        filter &= builder.Lte(x => x.DateOfBirth, minDoB);
      }

      if (!string.IsNullOrEmpty(userParams.OrderBy))
      {
        switch (userParams.OrderBy)
        {
          case "Created":
            sort = Builders<User>.Sort.Descending("Created");
            break;
          case "DateOfBirth":
            sort = Builders<User>.Sort.Descending("DateOfBirth");
            break;
          default:
            sort = Builders<User>.Sort.Descending("LastActive");
            break;
        }
      }

      var count = await _context.Users.Find(filter).CountDocumentsAsync();
      var users = await _context.Users.Find(filter).Sort(sort).Skip((userParams.PageNumber - 1) * userParams.PageSize).Limit(userParams.PageSize).ToListAsync();

      var result = new PagedList<User>(users, count, userParams.PageNumber, userParams.PageSize);
      return result;
    }

    public async Task<bool> IsMainPhoto(string username, string photoId)
    {
      var builder = Builders<User>.Filter;
      var filter = builder.And(builder.Eq("Username", username),
                   builder.ElemMatch(p => p.Photos, i => i.PublicId == photoId & i.IsMain == true));
      var result = await _context.Users.Find(filter).AnyAsync();
      return result;
    }

    public async Task<bool> SetMainPhoto(string username, string photoId)
    {
      // Remove any possible IsMain = true from user document
      await _context.Users.FindOneAndUpdateAsync(
        Builders<User>.Filter.Eq("Username", username),
        Builders<User>.Update.Set("Photos.$[].IsMain", false));

      // Set the especified photo as main
      var filter = Builders<User>.Filter.Eq("Username", username);
      // Now thats the trick part. Check the stackoverflow question below for more details:
      // https://stackoverflow.com/a/51622030/10112160
      var update = Builders<User>.Update.Set("Photos.$[option].IsMain", true);
      var arrayFilters = new List<ArrayFilterDefinition>();
      ArrayFilterDefinition<BsonDocument> optionsFilter = new BsonDocument("option.PublicId", new BsonDocument("$eq", photoId));
      arrayFilters.Add(optionsFilter);
      var updateOptions = new UpdateOptions { ArrayFilters = arrayFilters };

      var result = await _context.Users.UpdateOneAsync(filter, update, updateOptions);

      return result.ModifiedCount > 0;
    }

    public async Task<bool> UpdateLastActive(string username)
    {
      var filter = Builders<User>.Filter.Eq("Username", username);
      var update = Builders<User>.Update.Set("LastActive", DateTime.Now);
      var result = await _context.Users.UpdateOneAsync(filter, update);

      return result.ModifiedCount > 0;
    }

    public async Task<bool> UpdateUser(string username, UserForUpdateDto userForUpdateDto)
    {
      var filter = Builders<User>.Filter.Where(x => x.Username == username);
      var user = await _context.Users.Find(filter).FirstOrDefaultAsync();
      _mapper.Map(userForUpdateDto, user);
      var result = await _context.Users.ReplaceOneAsync(filter, user);
      return result.IsAcknowledged;
    }
  }
}