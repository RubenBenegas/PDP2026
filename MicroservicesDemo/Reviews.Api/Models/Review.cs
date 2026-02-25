using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Reviews.Api.Models
{
    public class Review
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("MovieId")]
        public string MovieId { get; set; }

        [BsonElement("Reviewer")]
        public string Reviewer { get; set; }

        [BsonElement("Rating")]
        public int Rating { get; set; }

        [BsonElement("Comment")]
        public string Comment { get; set; }
    }
}
