# WebApiLoad — Load Test Harness

A lightweight HTTP load testing tool for .NET 9. Spawns concurrent workers that hammer a target endpoint for a specified duration and reports per-HTTP-status-code results.

## Features

- JSON configuration file (endpoint, threads, duration, optional auth)
- Optional login flow — obtains a Bearer token and includes it on every request
- Concurrent workers via `Task.Run` — no external dependencies
- Per-HTTP-status-code counting via `ConcurrentDictionary`
- Network exceptions tracked separately from HTTP-level responses
- Counts in-flight requests dropped when the test timer expires
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

Console and log file (`load-test-result-YYYYMMDD-HHmmss.log`):

```
  Threads spawned:  100
  Status code breakdown:
    200: 4132
    429: 12
    500: 3
  Request exceptions (network errors): 0
  In-flight requests dropped on stop: 2
  Successful calls: 4132
  Failed calls:     15
  Total requests:   4147
```

## Project structure

```
Models/LoadTestConfig.cs     — config model (endpoint, credentials, threads, duration)
Models/LoadTestResult.cs     — result model (per-code counts, in-flight dropped)
Models/LoginResponse.cs      — login API response (token + expiration)
Services/LoadTester.cs       — spawns workers, tracks counters
Services/ResultLogger.cs     — writes log file
Services/AuthService.cs      — login call, returns Bearer token
Program.cs                   — entry point
template-config.json         — config template (copy to test-config.json and edit)
test-config.json             — local runtime config (gitignored, not tracked)
```
