using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace Takasbu.Models
{
public class CommentDTO
{

   public  string Context { get; set; } = string.Empty;
 
    public  string Subject { get; set; } = string.Empty;

}


}
