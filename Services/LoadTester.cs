using System.Collections.Concurrent;
using ApiLoadTest.Models;

namespace ApiLoadTest.Services;

public class LoadTester(HttpClient httpClient, LoadTestConfig config)
{
    private readonly ConcurrentDictionary<int, long> _statusCounts = new();
    private long _requestExceptions;
    private long _inFlightDropped;

    public async Task<LoadTestResult> RunAsync()
    {
        _statusCounts.Clear();
        _requestExceptions = 0;
        _inFlightDropped = 0;

        using var cts = new CancellationTokenSource(
            TimeSpan.FromSeconds(config.DurationSeconds));

        var tasks = new List<Task>();

        for (int i = 0; i < config.ThreadCount; i++)
        {
            tasks.Add(Task.Run(() => Worker(cts.Token), cts.Token));
        }

        var timeoutTask = Task.Delay(TimeSpan.FromSeconds(config.DurationSeconds + 5), CancellationToken.None);
        var allTasks = Task.WhenAll(tasks);
        await Task.WhenAny(allTasks, timeoutTask);

        return new LoadTestResult
        {
            ThreadsSpawned = config.ThreadCount,
            StatusCodes = _statusCounts.ToDictionary(kv => kv.Key, kv => kv.Value),
            RequestExceptions = Interlocked.Read(ref _requestExceptions),
            InFlightDropped = Interlocked.Read(ref _inFlightDropped)
        };
    }

    private async Task Worker(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            try
            {
                using var response = await httpClient.GetAsync(config.EndpointUrl, ct);
                var code = (int)response.StatusCode;
                _statusCounts.AddOrUpdate(code, 1, (_, count) => count + 1);
            }
            catch (OperationCanceledException)
            {
                Interlocked.Increment(ref _inFlightDropped);
                break;
            }
            catch
            {
                Interlocked.Increment(ref _requestExceptions);
            }
        }
    }
}
