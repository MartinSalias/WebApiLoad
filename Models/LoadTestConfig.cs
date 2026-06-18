namespace ApiLoadTest.Models;

public class LoadTestConfig
{
    public string EndpointUrl { get; set; } = string.Empty;
    public int ThreadCount { get; set; } = 1;
    public int DurationSeconds { get; set; } = 10;
}
