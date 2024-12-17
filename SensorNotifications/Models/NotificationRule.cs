using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SensorNotifications.Models;

public class NotificationRule
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    [BsonElement("name")]
    public string? Name { get; set; }
    [BsonElement("sensorId")]
    public string? SensorId { get; set; }
    [BsonElement("measurement")]
    public string Measurement { get; set; }
    [BsonElement("operator")]
    public string Operator { get; set; }
    [BsonElement("value")]
    public double Value { get; set; }
    [BsonIgnore]
    [BsonElement("notifications")]
    public List<Notification> Notifications { get; set; }
}
