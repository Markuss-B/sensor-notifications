using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace SensorNotifications.Models;

public class SensorMeasurements
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } // Unique identifier for each measurement record

    [BsonElement("timestamp")]
    public DateTime Timestamp { get; set; } // Time the measurement was recorded

    [BsonElement("sensorId")]
    public string SensorId { get; set; } // Sensor Id given by the sensor

    [BsonElement("measurements")]
    public Dictionary<string, object> Measurements { get; set; } // Dictionary for varied measurement fields
}

//[BsonIgnoreExtraElements]
//public class SensorMeasurementsFixed
//{
//    [BsonId]
//    [BsonRepresentation(BsonType.ObjectId)]
//    public string Id { get; set; } // Unique identifier for each measurement record
//    [BsonElement("timestamp")]
//    public DateTime Timestamp { get; set; } // Time the measurement was recorded
//    [BsonElement("sensorId")]
//    public string SensorId { get; set; } // Sensor Id given by the sensor
//    [BsonElement("measurements")]
//    public Measurements Measurements { get; set; } // Measurements from the sensor
//}