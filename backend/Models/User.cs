using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace backend.Models
{
  public class User
  {
    [BsonId]
    public ObjectId _id { get; set; }
    public string Username { get; set; }
    public byte[] PasswordHash { get; set; }
    public byte[] PasswordSalt { get; set; }
    public string Gender { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string FullName { get; set; }
    public string KnownAs { get; set; }
    public string School { get; set; }
    public string House { get; set; }
    public string Patronus { get; set; }
    public string Introduction { get; set; }
    public string LookingFor { get; set; }
    public string Interests { get; set; }
    public string Country { get; set; }
    public ICollection<Photo> Photos { get; set; }
    public DateTime Created { get; set; }
    public DateTime LastActive { get; set; }
    [BsonExtraElements]
    public BsonDocument CatchAll { get; set; }

    public User()
    {
      Photos = Array.Empty<Photo>();
    }
  }

  public class Photo
  {
    public string Url { get; set; }
    public string Description { get; set; }
    public DateTime DateAdded { get; set; }
    public bool IsMain { get; set; }
    public string PublicId { get; set; }
  }
}
