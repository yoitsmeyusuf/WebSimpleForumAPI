using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace Takasbu.Models
{
    public class Comment
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("SubjectId")]
        [JsonPropertyName("SubjectId")]
        public string SubjectId { get; set; } = string.Empty;

        [BsonElement("Context")]
        [JsonPropertyName("Context")]
        public string context { get; set; } = string.Empty;

        [BsonElement("DateTime")]
        [JsonPropertyName("DateTime")]
        public DateTime DateTime { get; set; }

        [BsonElement("Owner")]
        public string Owner { get; set; } = string.Empty;
    }
}
