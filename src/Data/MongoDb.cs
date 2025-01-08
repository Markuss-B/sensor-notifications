using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using SensorNotifications.Models;

namespace SensorNotifications.Data;

public class MongoDb
{
    public MongoDb(IOptions<MongoDbSettings> options)
    {
        MongoDbSettings settings = options.Value;
        MongoClient client = new MongoClient(settings.ConnectionString);
        IMongoDatabase database = client.GetDatabase(settings.DatabaseName);

        SensorMeasurements = database.GetCollection<SensorMeasurements>("sensorMeasurements");
        Notifications = database.GetCollection<Notification>("notifications");
        NotificationRules = database.GetCollection<NotificationRule>("notificationRules");

        var conventionPack = new ConventionPack
        {
            new CamelCaseElementNameConvention(),
            new EnumRepresentationConvention(BsonType.String),
        };
        ConventionRegistry.Register("camelCase", conventionPack, t => true);
    }

    public IMongoCollection<SensorMeasurements> SensorMeasurements { get; set; }
    public IMongoCollection<Notification> Notifications { get; set; }
    public IMongoCollection<NotificationRule> NotificationRules { get; set; }
}