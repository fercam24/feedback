using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GameSessionFeedback.Models
{
    public class SessionFeedback
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [JsonIgnore]
        public string UserId { get; set; }

        [JsonIgnore]
        public string SessionId { get; set; }
        
        [Range(1, 5)]
        public short Rate { get; set; }

        public string Comment { get; set; }
        
        public DateTime CreatedOn { get; set; } = DateTime.Now;
    }
}