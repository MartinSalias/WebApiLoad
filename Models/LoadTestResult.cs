namespace ApiLoadTest.Models;

public class LoadTestResult
{
    public int ThreadsSpawned { get; init; }
    public long RequestsStarted { get; init; }
    public Dictionary<int, long> StatusCodes { get; init; } = [];
    public Dictionary<string, long> ExceptionCounts { get; init; } = [];

    public long TotalStatusCodes => StatusCodes.Values.Sum();
    public long TotalExceptions => ExceptionCounts.Values.Sum();
    public long TotalSuccess => StatusCodes
        .Where(kv => kv.Key >= 200 && kv.Key < 300)
        .Sum(kv => kv.Value);
    public long TotalFailed => TotalStatusCodes - TotalSuccess + TotalExceptions;
    public long InFlightDropped => RequestsStarted - TotalStatusCodes - TotalExceptions;
    public long TotalRequests => TotalStatusCodes + TotalExceptions;
}
