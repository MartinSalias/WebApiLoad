using System.Collections.Concurrent;
using ApiLoadTest.Models;

namespace ApiLoadTest.Services;

public class LoadTester(HttpClient httpClient, LoadTestConfig config)
{
    private readonly ConcurrentDictionary<int, long> _statusCounts = new();
    private readonly ConcurrentDictionary<string, long> _exceptionCounts = new();
    private readonly ConcurrentQueue<string> _failureDetails = new();
    private long _requestsStarted;

    public async Task<LoadTestResult> RunAsync()
    {
        _statusCounts.Clear();
        _exceptionCounts.Clear();
        _failureDetails.Clear();
        _requestsStarted = 0;

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
            RequestsStarted = Interlocked.Read(ref _requestsStarted),
            StatusCodes = _statusCounts.ToDictionary(kv => kv.Key, kv => kv.Value),
            ExceptionCounts = _exceptionCounts.ToDictionary(kv => kv.Key, kv => kv.Value),
            FailureDetails = [.. _failureDetails]
        };
    }

    private async Task Worker(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            Interlocked.Increment(ref _requestsStarted);

            try
            {
                using var response = await httpClient.GetAsync(config.EndpointUrl, ct);
                var code = (int)response.StatusCode;
                _statusCounts.AddOrUpdate(code, 1, (_, count) => count + 1);

                if (!response.IsSuccessStatusCode)
                {
                    var reason = response.ReasonPhrase ?? "";
                    _failureDetails.Enqueue(
                        $"[{DateTime.Now:HH:mm:ss.fff}] HTTP {code} {reason}");
                }
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested)
            {
                _failureDetails.Enqueue(
                    $"[{DateTime.Now:HH:mm:ss.fff}] Request cancelled (test duration expired)");
                break;
            }
            catch (Exception ex)
            {
                var name = ex.GetType().Name;
                _exceptionCounts.AddOrUpdate(name, 1, (_, count) => count + 1);
                _failureDetails.Enqueue(
                    $"[{DateTime.Now:HH:mm:ss.fff}] {name}: {ex.Message}");
            }
        }
    }
}
