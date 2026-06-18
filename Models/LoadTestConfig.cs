namespace ApiLoadTest.Models;

public class LoadTestConfig
{
    public string EndpointUrl { get; set; } = string.Empty;
    public string? LoginUrl { get; set; }
    public string? UserName { get; set; }
    public string? Password { get; set; }
    public string? CompanyVATid { get; set; }
    public int ThreadCount { get; set; } = 1;
    public int DurationSeconds { get; set; } = 10;
}
