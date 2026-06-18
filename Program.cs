using System.Text.Json;
using ApiLoadTest.Models;
using ApiLoadTest.Services;

const string ConfigFile = "test-config.json";

if (!File.Exists(ConfigFile))
{
    Console.Error.WriteLine($"Config file '{ConfigFile}' not found.");
    return 1;
}

var configJson = await File.ReadAllTextAsync(ConfigFile);
var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
var config = JsonSerializer.Deserialize<LoadTestConfig>(configJson, options);

if (config is null || string.IsNullOrWhiteSpace(config.EndpointUrl))
{
    Console.Error.WriteLine("Invalid config file. 'endpointUrl' is required.");
    return 1;
}

var httpClient = new HttpClient
{
    Timeout = TimeSpan.FromSeconds(30)
};

var tester = new LoadTester(httpClient, config);

Console.WriteLine($"Starting load test:");
Console.WriteLine($"  Endpoint: {config.EndpointUrl}");
Console.WriteLine($"  Threads:  {config.ThreadCount}");
Console.WriteLine($"  Duration: {config.DurationSeconds} seconds");
Console.WriteLine();

var result = await tester.RunAsync();

await ResultLogger.LogAsync(config, result);

Console.WriteLine();
Console.WriteLine($"Summary:");
Console.WriteLine($"  Threads spawned:  {result.ThreadsSpawned}");
Console.WriteLine($"  Status code breakdown:");

foreach (var kv in result.StatusCodes.OrderBy(kv => kv.Key))
{
    Console.WriteLine($"    {kv.Key}: {kv.Value}");
}

Console.WriteLine($"  Request exceptions (network errors): {result.RequestExceptions}");
Console.WriteLine($"  Successful calls: {result.TotalSuccess}");
Console.WriteLine($"  Failed calls:     {result.TotalFailed}");
Console.WriteLine($"  Total requests:   {result.TotalRequests}");

return 0;
