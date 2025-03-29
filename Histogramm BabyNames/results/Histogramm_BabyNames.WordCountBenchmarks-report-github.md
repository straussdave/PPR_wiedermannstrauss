```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.26100.3476)
12th Gen Intel Core i5-12400F, 1 CPU, 12 logical and 6 physical cores
.NET SDK 8.0.202
  [Host]     : .NET 8.0.3 (8.0.324.11423), X64 RyuJIT AVX2 [AttachedDebugger]
  DefaultJob : .NET 8.0.3 (8.0.324.11423), X64 RyuJIT AVX2


```
| Method             | BatchSize | Mean     | Error   | StdDev  |
|------------------- |---------- |---------:|--------:|--------:|
| CountWordsParallel | 162457    | 336.3 ms | 6.63 ms | 9.07 ms |
| CountWordsParallel | 162457    | 309.8 ms | 5.89 ms | 5.51 ms |
