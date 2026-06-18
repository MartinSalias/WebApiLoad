namespace ApiLoadTest.Models;

public class LoadTestResult
{
    public int ThreadsSpawned { get; init; }
    public Dictionary<int, long> StatusCodes { get; init; } = [];
    public long RequestExceptions { get; init; }

    public long TotalSuccess => StatusCodes
        .Where(kv => kv.Key >= 200 && kv.Key < 300)
        .Sum(kv => kv.Value);

    public long TotalFailed => TotalRequests - TotalSuccess;
    public long TotalRequests => StatusCodes.Values.Sum() + RequestExceptions;
}
