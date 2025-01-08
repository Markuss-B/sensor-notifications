using MongoDB.Driver;
using SensorNotifications.Data;
using SensorNotifications.Helpers;
using SensorNotifications.Models;
using System.Globalization;

namespace SensorNotifications.Services;

public class NotificationService
{
    private readonly ILogger<NotificationService> _logger;
    private readonly MongoDb _db;

    public NotificationService(ILogger<NotificationService> logger, MongoDb db)
    {
        _logger = logger;
        _db = db;
    }

    public async Task HandleMeasurement(SensorMeasurements sensorMeasurements)
    {
        var rules = _db.NotificationRules
            .Find(r => (r.SensorId == null || r.SensorId == string.Empty || r.SensorId == sensorMeasurements.SensorId)
                        && sensorMeasurements.Measurements.Keys.Contains(r.Measurement)).ToList();

        var measurements = sensorMeasurements.Measurements;

        foreach (var rule in rules)
        {
            // try get the rule measurement from the measurements
            if (!measurements.TryGetValue(rule.Measurement, out var value))
            {
                _logger.LogWarning("Measurement not found: {Measurement}.", rule.Measurement);
                continue;
            }

            // convert to double
            if (!double.TryParse(value.ToString(), CultureInfo.InvariantCulture, out var doubleValue))
            {
                _logger.LogWarning("Measurement not a number: {Measurement}.", value);
                continue;
            }

            // check if the value triggers or clears the rule
            var triggers = rule.Triggers(doubleValue);

            if (triggers)
            {
                _logger.LogDebug("Rule triggered: {Rule}, Measurement: {Measurement}.", rule.ToJsonString(), measurements.ToJsonString());
                // find matching notifications by sensorId and ruleId which does not have a endTimestamp
                var notifications = _db.Notifications
                    .Find(n => n.SensorId == sensorMeasurements.SensorId && n.RuleId == rule.Id && n.EndTimestamp == null)
                    .ToList();
                // if there are no notifications, create one
                if (notifications.Count == 0)
                {
                    var notification = new Notification
                    {
                        SensorId = sensorMeasurements.SensorId,
                        RuleId = rule.Id,
                        Message = rule.GetMessage(sensorMeasurements),
                        StartTimestamp = sensorMeasurements.Timestamp,
                        EndTimestamp = null
                    };
                    await _db.Notifications.InsertOneAsync(notification);
                    _logger.LogInformation("Notification created: {Notification}.", notification.ToJsonString());
                }
            }
            else
            {
                _logger.LogInformation("Rule cleared: {Rule}, Measurement: {Measurement}.", rule.ToJsonString(), measurements.ToJsonString());
                // get latest matching notifications by sensorId and ruleId which does not have a endTimestamp
                // there should be only one
                var notifications = _db.Notifications
                    .Find(n => n.SensorId == sensorMeasurements.SensorId && n.RuleId == rule.Id && n.EndTimestamp == null)
                    .SortByDescending(n => n.StartTimestamp)
                    .ToList();

                if (notifications.Count > 1)
                {
                    _logger.LogWarning("Unexpected number of notifications: {Count}.", notifications.Count);
                    continue;
                }
                else if (notifications.Count == 0)
                {
                    _logger.LogDebug("No notifications to clear found.");
                    continue;
                }

                // update the notification with the endTimestamp
                var notification = notifications.First();
                notification.EndTimestamp = sensorMeasurements.Timestamp;
                await _db.Notifications.ReplaceOneAsync(n => n.Id == notification.Id, notification);

                _logger.LogInformation("Notification updated: {Notification}.", notification.ToJsonString());
            }
        }
    }
}