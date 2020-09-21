using System;
using System.ComponentModel.DataAnnotations;
using GameSessionFeedback.Utils;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace GameSessionFeedback.Models
{
    public class SessionFeedback
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [JsonProperty("id")]
        public string Id { get; set; }
        
        [JsonProperty("user")]
        public string UserId { get; set; }
        
        [JsonProperty("session")]
        public string SessionId { get; set; }
        
        [Required]
        [Range(1, 5)]
        [JsonProperty("rating")]
        public short Rate { get; set; }

        [JsonProperty("comment")]
        public string Comment { get; set; } = string.Empty;
            
        [JsonConverter(typeof(DateFormatConverter), "yyyy-MM-dd hh:mm:ss")]
        [JsonProperty("feedbackDate")]
        public DateTime CreatedOn { get; set; }
    }
}