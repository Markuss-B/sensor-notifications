using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SensorNotifications.Models;

public class Notification
{
    [BsonId]
    public ObjectId Id { get; set; }
    /// <summary>
    /// Sensor ID that triggered the notification.
    /// </summary>
    [BsonElement("sensorId")]
    public string? SensorId { get; set; }
    /// <summary>
    /// Notification message.
    /// </summary>
    [BsonElement("message")]
    public string Message { get; set; }
    /// <summary>
    /// Timestamp when the notification was created/ when a sensor measurement triggered the rule.
    /// </summary>
    [BsonElement("startTimestamp")]
    public DateTime StartTimestamp { get; set; }
    /// <summary>
    /// Timestamp when the sensor measurement stopped triggering the rule.
    /// </summary>
    [BsonElement("endTimestamp")]
    [BsonDefaultValue(null)]
    public DateTime? EndTimestamp { get; set; }
    /// <summary>
    /// Related rule ID. <see cref="NotificationRule"/>
    /// </summary>
    [BsonElement("ruleId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string RuleId { get; set; }
}
