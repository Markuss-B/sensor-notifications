using MongoDB.Bson;
using MongoDB.Driver;
using SensorNotifications.Data;
using SensorNotifications.Helpers;
using SensorNotifications.Models;
using SensorNotifications.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorNotifications.Watchers;

public class MeasurementWatcher : BackgroundService
{
    private readonly ILogger<MeasurementWatcher> _logger;
    private readonly MongoDb _db;
    private readonly NotificationService _notificationService;

    public MeasurementWatcher(ILogger<MeasurementWatcher> logger, MongoDb db, NotificationService notificationService)
    {
        _logger = logger;
        _db = db;
        _notificationService = notificationService;
    }

    /// <summary>
    /// Start watching for measurements.
    /// </summary>
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Executing measurement watcher.");

        var col = _db.SensorMeasurements;
        var options = new ChangeStreamOptions { FullDocument = ChangeStreamFullDocumentOption.UpdateLookup }; // Get the full document on change
        var pipeline = new EmptyPipelineDefinition<ChangeStreamDocument<SensorMeasurements>>()
            .Match(change => change.OperationType == ChangeStreamOperationType.Insert); // Only watch for insert operations

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogInformation("Watching for measurements.");

                using var cursor = await col.WatchAsync(pipeline, options); // Watch for changes

                await cursor.ForEachAsync(async change =>
                {
                    var measurements = change.FullDocument;
                    _logger.LogInformation("Measurement insertion detected: {Change}.", measurements.ToJsonString());
                    await _notificationService.HandleMeasurement(measurements); // Handle the measurement

                }, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Measurement watcher stopped.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error watching for measurements.");
            }
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping measurement watcher.");
        await base.StopAsync(cancellationToken);
    }
}
