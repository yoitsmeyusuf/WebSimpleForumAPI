using Amazon.SecurityToken.Model;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace ForumApi.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; internal set; } = string.Empty;

        [BsonElement("Name")]
        [JsonPropertyName("Name")]
        public string Username { get; set; } = string.Empty;

        [BsonElement("Bio")]
        [JsonPropertyName("Bio")]
        public string Bio { get; set; } = string.Empty;

        [BsonElement("ProductIds")]
        public List<string> SubjectIds { get; set; }

        [BsonElement("CommentIds")]
        public List<string> CommentIds { get; set; }
        public byte[]? PasswordHash { get; set; }
        public byte[]? PasswordSalt { get; set; }

        [BsonElement("Role")]
        public string Roles { get; } = "User";

        [BsonElement("notifcomids")]
        public List<string> notifcomids { get; set; }

        public string mailpass {get; set;} = "";

        public User()
        {
            SubjectIds = new List<string>();
            CommentIds = new List<string>();
            notifcomids = new List<string>();
        }
    }
}
