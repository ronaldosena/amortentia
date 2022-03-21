using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace backend.Dtos
{
    public class UserForListDto
    {
        [BsonId]
        public ObjectId _id { get; set; }
        public string Username { get; set; }
        public string Gender { get; set; }
        public int Age { get; set; }
        public string KnownAs { get; set; }
        public string School { get; set; }
        public string House { get; set; }
        public string PhotoUrl { get; set; }
    }
}