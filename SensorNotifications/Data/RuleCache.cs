using MongoDB.Bson;
using MongoDB.Driver;
using SensorNotifications.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorNotifications.Data;

public class RuleCache
{
    private readonly ILogger<RuleCache> _logger;
    private ConcurrentDictionary<NotificationRule, byte> _rules;

    public RuleCache(ILogger<RuleCache> logger)
    {
        _logger = logger;
        _rules = new ConcurrentDictionary<NotificationRule, byte>();
    }

    /// <summary>
    /// Load rules into cache.
    /// </summary>
    public void LoadRules(IEnumerable<NotificationRule> rules)
    {
        _rules = new ConcurrentDictionary<NotificationRule, byte>(rules.Select(s => new KeyValuePair<NotificationRule, byte>(s, 0)));
        _logger.LogInformation("Loaded rules: {Rules}.", rules.ToJson());
    }

    /// <summary>
    /// Add a rule to the cache.
    /// </summary>
    public void AddRule(NotificationRule rule)
    {
        _rules.TryAdd(rule, 0);
        _logger.LogInformation("Added rule: {Rule}.", rule);
    }

    /// <summary>
    /// Remove a rule from the cache.
    /// </summary>
    public void RemoveRule(NotificationRule rule)
    {
        _rules.TryRemove(rule, out _);
        _logger.LogInformation("Removed rule: {Rule}.", rule);
    }

    /// <summary>
    /// Clear the rule cache.
    /// </summary>
    public void Clear()
    {
        _rules.Clear();
        _logger.LogInformation("Cleared all rules.");
    }
}