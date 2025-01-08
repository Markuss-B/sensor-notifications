using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SensorNotifications.Models;

public class NotificationRule
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    /// <summary>
    /// Name of the rule which will appear in the message
    /// </summary>
    [BsonElement("name")]
    public string? Name { get; set; }
    /// <summary>
    /// Sensor ID that the rule is associated with. If null, the rule applies to all sensors.
    /// </summary>
    [BsonElement("sensorId")]
    public string? SensorId { get; set; }
    /// <summary>
    /// Measurement to compare ex. temperature, co2
    /// </summary>
    [BsonElement("measurement")]
    public string Measurement { get; set; }
    /// <summary>
    /// Operator to compare the value with the measurement. ">", "<"
    /// </summary>
    [BsonElement("operator")]
    public string Operator { get; set; }
    /// <summary>
    /// Value to compare with the measurement. If the operator is ">" and the value is 30, the rule will trigger if the measurement is over 30.
    /// </summary>
    [BsonElement("value")]
    public double Value { get; set; }

    /// <summary>
    /// Check if the value triggers the rule.
    /// </summary>
    public bool Triggers(double value)
    {
        return Operator switch
        {
            "<" => value < Value,
            ">" => value > Value,
            _ => throw new ArgumentException($"Unknown operator: {Operator}"),
        };
    }

    /// <summary>
    /// Creates the message for the notification and includes the trigger sensor ID.
    /// </summary>
    /// <param name="measurements"><see cref="SensorMeasurements"/> which triggered a rule</param>
    public string GetMessage(SensorMeasurements measurements)
    {
        return $"{Name}. Sensors {measurements.SensorId}: {Measurement} {Operator} {Value}";
    }
}