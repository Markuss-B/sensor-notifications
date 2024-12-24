using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SensorNotifications.Models;

public class Notification
{
    [BsonId]
    public ObjectId Id { get; set; }
    [BsonElement("sensorId")]
    public string? SensorId { get; set; }
    [BsonElement("message")]
    public string Message { get; set; }
    [BsonElement("startTimestamp")]
    public DateTime StartTimestamp { get; set; }
    [BsonElement("endTimestamp")]
    [BsonDefaultValue(null)]
    public DateTime? EndTimestamp { get; set; }
    [BsonElement("ruleId")]
    public string RuleId { get; set; }
}
