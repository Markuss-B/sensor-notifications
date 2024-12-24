using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Globalization;

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

    public bool Triggers(double value)
    {
        return Operator switch
        {
            "<" => value < Value,
            ">" => value > Value,
            _ => throw new ArgumentException($"Unknown operator: {Operator}"),
        };
    }

    public string GetMessage(SensorMeasurements measurements)
    {
        return $"{Name}.Sensors {measurements.SensorId}: {Measurement} {Operator} {Value}";
    }
}
