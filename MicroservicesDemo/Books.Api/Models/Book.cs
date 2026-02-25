using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Books.Api.Models
{
    public class Book
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string Title { get; set; } = null!;
        public string Author { get; set; } = null!;
    }
}
