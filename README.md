# WebApiLoad — Load Test Harness

A lightweight HTTP load testing tool for .NET 9. Spawns concurrent workers that hammer a target endpoint for a specified duration and reports per-HTTP-status-code results.

## Features

- JSON configuration file (endpoint, threads, duration, optional auth)
- Optional login flow — obtains a Bearer token and includes it on every request
- Concurrent workers via `Task.Run` — no external dependencies
- Per-HTTP-status-code counting via `ConcurrentDictionary`
- Per-exception-type breakdown (e.g. `HttpRequestException`, `Timeout`)
- Tracks total requests started vs completed — `InFlightDropped` shows requests initiated but never completed
- Logs per-call failure details: HTTP errors (code + reason) and exceptions (type + message)
- Timestamped log file with full summary

## How to use

1. Copy `template-config.json` to `test-config.json` and edit it:

   ```bash
   cp template-config.json test-config.json
   ```

   Basic (no auth):
   ```json
   {
     "endpointUrl": "https://api.example.com/health",
     "threadCount": 100,
     "durationSeconds": 30
   }
   ```

   With authentication:
   ```json
   {
     "endpointUrl": "https://api.example.com/data",
     "loginUrl": "https://api.example.com/login",
     "userName": "admin",
     "password": "secret",
     "companyVATid": "VAT123",
     "threadCount": 100,
     "durationSeconds": 30
   }
   ```

2. Run:
   ```bash
   dotnet run
   ```

## Output

Console (summary only):

```
  Threads spawned:  100
  Requests started: 5000
  Status code breakdown:
    200: 4132
    429: 12
    500: 3
  Exception breakdown:
    HttpRequestException: 5
    Timeout: 2
  In-flight requests dropped on stop: 846
  Successful calls: 4132
  Failed calls:     22
  Total requests:   4154
```

Log file (`load-test-result-YYYYMMDD-HHmmss.log`) — same summary plus per-call failure details:

```
  Failure details (10):
    [15:42:25.503] Request cancelled (test duration expired)
    [15:42:25.504] Request cancelled (test duration expired)
    [15:42:25.505] HTTP 500 Internal Server Error
    [15:42:25.506] HttpRequestException: Connection refused
    [15:42:25.507] TaskCanceledException: The request was canceled due to the configured HttpClient.Timeout of 30 seconds elapsing.
```

## Project structure

```
Models/LoadTestConfig.cs     — config model (endpoint, credentials, threads, duration)
Models/LoadTestResult.cs     — result model (started, per-code counts, per-exception counts, in-flight dropped)
Models/LoginResponse.cs      — login API response (token + expiration)
Services/LoadTester.cs       — spawns workers, tracks counters
Services/ResultLogger.cs     — writes log file
Services/AuthService.cs      — login call, returns Bearer token
Program.cs                   — entry point
template-config.json         — config template (copy to test-config.json and edit)
test-config.json             — local runtime config (gitignored, not tracked)
```
