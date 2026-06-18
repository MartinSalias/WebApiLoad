# WebApiLoad — Load Test Harness

A lightweight HTTP load testing tool for .NET 9. Spawns concurrent workers that hammer a target endpoint for a specified duration and reports per-HTTP-status-code results.

## Features

- JSON configuration file (endpoint, thread count, duration)
- Concurrent workers via `Task.Run` — no external dependencies
- Per-HTTP-status-code counting via `ConcurrentDictionary`
- Network exceptions tracked separately from HTTP-level responses
- Timestamped log file with full summary

## How to use

1. Edit `test-config.json`:
   ```json
   {
     "endpointUrl": "https://yourserver/api/endpoint",
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
  Successful calls: 4132
  Failed calls:     15
  Total requests:   4147
```

## Project structure

```
Models/LoadTestConfig.cs     — config model
Models/LoadTestResult.cs     — result model
Services/LoadTester.cs       — spawns workers, tracks counters
Services/ResultLogger.cs     — writes log file
Program.cs                   — entry point
test-config.json             — runtime configuration
```
