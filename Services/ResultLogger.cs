using ApiLoadTest.Models;

namespace ApiLoadTest.Services;

public static class ResultLogger
{
    public static async Task LogAsync(LoadTestConfig config, LoadTestResult result)
    {
        var timestamp = DateTime.Now.ToString("yyyyMMdd-HHmmss");
        var fileName = $"load-test-result-{timestamp}.log";

        var lines = new List<string>
        {
            $"Load Test Results - {DateTime.Now:yyyy-MM-dd HH:mm:ss}",
            $"",
            $"Configuration:",
            $"  Endpoint:       {config.EndpointUrl}",
            $"  Threads:        {config.ThreadCount}",
            $"  Duration:       {config.DurationSeconds} seconds",
            $"",
            $"Results:",
            $"  Threads spawned:  {result.ThreadsSpawned}",
            $"  Status code breakdown:"
        };

        foreach (var kv in result.StatusCodes.OrderBy(kv => kv.Key))
        {
            lines.Add($"    {kv.Key}: {kv.Value}");
        }

        lines.Add($"  Request exceptions (network errors): {result.RequestExceptions}");
        lines.Add($"  Successful calls: {result.TotalSuccess}");
        lines.Add($"  Failed calls:     {result.TotalFailed}");
        lines.Add($"  Total requests:   {result.TotalRequests}");

        await File.WriteAllTextAsync(fileName, string.Join(Environment.NewLine, lines));
        Console.WriteLine($"Results written to {fileName}");
    }
}
