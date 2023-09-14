using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace ForumApi.Models
{
    public class Subject
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("Name")]
        [JsonPropertyName("Name")]
        public string Name { get; set; } = string.Empty;

        [BsonElement("Description")]
        [JsonPropertyName("Description")]
        public string Description { get; set; } = string.Empty;

        [BsonElement("DateTime")]
        [JsonPropertyName("DateTime")]
        public DateTime DateTime { get; set; }

        [BsonElement("Owner")]
        public string Owner { get; set; } = string.Empty;

        [BsonElement("Comments")]
        public List<string> Comments { get; set; }

        public Subject()
        {
            Comments = new List<string>();
        }
    }
}
