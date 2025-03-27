using BenchmarkDotNet.Attributes;
using System.Collections.Concurrent;
using System.Runtime.InteropServices;

namespace Histogramm_BabyNames;

[MemoryDiagnoser]
[ShortRunJob]
[IterationCount(3)]
[WarmupCount(1)]
[InvocationCount(100)]
[ThreadingDiagnoser]
public class WordCountBenchmarks
{
    private string[] allLines = null!;
    private const string FilePath = "war_and_peace.txt";

    // Wird nur von der Parallel-Methode verwendet!
    [Params( 400,500, 80000,90000,10000,120000)]
    public int BatchSize;

    [GlobalSetup]
    public void Setup()
    {
        allLines = File.ReadAllLines(FilePath);
    }

    [Benchmark(Baseline = true)]
    public Dictionary<string, int> CountWordsSequenziell()
    {
        var wordCounts = new Dictionary<string, int>();

        foreach (string line in allLines)
        {
            foreach (string word in line.Split([' ', ',', '.', '!', '?', ';', ':', '"', '&']))
            {
                if (!string.IsNullOrWhiteSpace(word))
                {
                    wordCounts.TryAdd(word, 0);
                    wordCounts[word]++;
                }
            }
        }

        return wordCounts;
    }

    [Benchmark]
    public ConcurrentDictionary<string, int> CountWordsParallel()
    {
        var partitioner = Partitioner.Create(0, allLines.Length, BatchSize);
        var globalCounts = new ConcurrentDictionary<string, int>();

        Parallel.ForEach(partitioner, range =>
        {
            var localCounts = new Dictionary<string, int>();

            for (int i = range.Item1; i < range.Item2; i++)
            {
                foreach (string word in allLines[i].Split([' ', ',', '.', '!', '?', ';', ':', '"', '&']))
                {
                    if (!string.IsNullOrWhiteSpace(word))
                    {
                        if (!localCounts.ContainsKey(word))
                            localCounts[word] = 0;
                        localCounts[word]++;
                    }
                }
            }

            foreach (var kvp in localCounts)
            {
                globalCounts.AddOrUpdate(kvp.Key, kvp.Value, (_, oldVal) => oldVal + kvp.Value);
            }
        });

        return globalCounts;
    }
}
