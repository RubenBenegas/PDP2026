using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Movies.Api.Models
{
    public class Movie
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string Title { get; set; } = null!;
        public string Director { get; set; } = null!;
        public int Year { get; set; }
    }
}