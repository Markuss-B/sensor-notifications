using SensorNotifications;
using SensorNotifications.Data;
using SensorNotifications.Models;
using SensorNotifications.Services;
using SensorNotifications.Watchers;

var builder = Host.CreateApplicationBuilder(args);

var config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables()
    .Build();

builder.Services.Configure<MongoDbSettings>(config.GetSection("MongoDbSettings"));

builder.Services.AddSingleton<MongoDb>();
builder.Services.AddSingleton<NotificationService>();

builder.Services.AddHostedService<MeasurementWatcher>();

var host = builder.Build();
host.Run();
