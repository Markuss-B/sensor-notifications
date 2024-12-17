using MongoDB.Driver;
using SensorNotifications.Data;
using SensorNotifications.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorNotifications.Watchers;

public class RuleWatcher : BackgroundService
{
    private readonly ILogger<RuleWatcher> _logger;
    private readonly RuleCache _ruleCache;
    private readonly MongoDb _db;
    public RuleWatcher(ILogger<RuleWatcher> logger, RuleCache ruleCache, MongoDb db)
    {
        _logger = logger;
        _ruleCache = ruleCache;
        _db = db;
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        var rules = _db.NotificationRules.Find(FilterDefinition<NotificationRule>.Empty).ToList();
        _ruleCache.LoadRules(rules);

        await base.StartAsync(cancellationToken);
    }

    /// <summary>
    /// Start watching for rule changes.
    /// </summary>
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var col = _db.NotificationRules;

        var options = new ChangeStreamOptions { FullDocument = ChangeStreamFullDocumentOption.UpdateLookup };
        var pipeline = new EmptyPipelineDefinition<ChangeStreamDocument<NotificationRule>>()
            .Match("{ operationType: { $in: [ 'insert', 'update', 'replace', 'delete' ] } }");

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                using var cursor = await col.WatchAsync(pipeline, options);

                while (await cursor.MoveNextAsync())
                {
                    foreach (var change in cursor.Current)
                    {
                        _ruleCache.RemoveRule(change.FullDocument); // Make sure the rule doesnt exist in the cache
                        if (change.OperationType != ChangeStreamOperationType.Delete) // If the operation is not delete, add the rule to the cache
                        {
                            _ruleCache.AddRule(change.FullDocument);
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Inactive sensor cache stopping.");
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while watching for rule changes.");
            }
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _ruleCache.Clear();
        await base.StopAsync(cancellationToken);
    }
}
