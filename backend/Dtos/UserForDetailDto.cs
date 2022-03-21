using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace backend.Dtos
{
    public class UserForDetailDto
    {
        [BsonId]
        public ObjectId _id { get; set; }
        public string Username { get; set; }
        public string Gender { get; set; }
        public int Age { get; set; }
        public string FullName { get; set; }
        public string KnownAs { get; set; }
        public string School { get; set; }
        public string House { get; set; }
        public string Patronus { get; set; }
        public string Introduction { get; set; }
        public string LookingFor { get; set; }
        public string Interests { get; set; }
        public string Country { get; set; }
        public string PhotoUrl { get; set; }
        public ICollection<PhotoForDetailDto> Photos { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastActive { get; set; }
        [BsonExtraElements]
        public BsonDocument CatchAll { get; set; }
    }
}