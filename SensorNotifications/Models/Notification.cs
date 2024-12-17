using MongoDB.Bson.Serialization.Attributes;

namespace SensorNotifications.Models;

public class Notification
{
    [BsonId]
    public string Id { get; set; }
    [BsonElement("sensorId")]
    public string? SensorId { get; set; }
    [BsonElement("message")]
    public string Message { get; set; }
    [BsonElement("timestamp")]
    public DateTime Timestamp { get; set; }
    [BsonElement("ruleId")]
    [BsonDefaultValue(false)]
    public string RuleId { get; set; }
}
