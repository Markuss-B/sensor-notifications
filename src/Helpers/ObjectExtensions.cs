﻿using System.Text.Json;

namespace SensorNotifications.Helpers;

internal static class ObjectExtensions
{
    public static string ToJsonString<TObject>(this TObject @object)
    {
        var output = "NULL";
        if (@object != null)
        {
            output = JsonSerializer.Serialize(@object, new JsonSerializerOptions
            {
                WriteIndented = true
            });
        }

        return $"[{@object?.GetType().Name}]:\r\n{output}";
    }
}