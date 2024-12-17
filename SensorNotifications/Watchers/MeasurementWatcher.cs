using MongoDB.Driver;
using SensorNotifications.Data;
using SensorNotifications.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorNotifications.Watchers;

public class MeasurementWatcher : BackgroundService
{
    private readonly ILogger<MeasurementWatcher> _logger;
    private readonly RuleCache _ruleCache;
    private readonly MongoDb _db;

    public MeasurementWatcher(ILogger<MeasurementWatcher> logger, RuleCache ruleCache, MongoDb db)
    {
        _logger = logger;
        _ruleCache = ruleCache;
        _db = db;
    }
    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        await base.StartAsync(cancellationToken);
    }
    /// <summary>
    /// Start watching for measurements.
    /// </summary>
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var col = _db.SensorMeasurements;
        var options = new ChangeStreamOptions { FullDocument = ChangeStreamFullDocumentOption.UpdateLookup };
        var pipeline = new EmptyPipelineDefinition<ChangeStreamDocument<SensorMeasurements>>()
            .Match("{ operationType: { $in: [ 'insert', 'update', 'replace', 'delete' ] } }");
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                using var cursor = await col.WatchAsync(pipeline, options);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }
    }
}
