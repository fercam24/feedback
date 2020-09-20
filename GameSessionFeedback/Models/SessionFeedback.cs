using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GameSessionFeedback.Models
{
    public class SessionFeedback
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public string SessionId { get; set; }
        
        [Range(1, 5)]
        public short Rate { get; set; }

        public string Comment { get; set; }
    }
}