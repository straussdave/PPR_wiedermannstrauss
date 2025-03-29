using BenchmarkDotNet.Attributes;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Configs;

namespace Histogramm_BabyNames;

public class WordCountBenchmarks
{
    private string[] allLines = null!;
    private const string FilePath = "war_and_peace.txt";


    [GlobalSetup]
    public void Setup()
    {
        // Load all lines of the text file
        allLines = File.ReadAllLines(FilePath);
    }

    [Params(162457, 162457)]
    public int BatchSize;

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

            // Merge local counts into global counts
            foreach (var kvp in localCounts)
            {
                globalCounts.AddOrUpdate(kvp.Key, kvp.Value, (_, oldVal) => oldVal + kvp.Value);
            }
        });

        return globalCounts;
    }
}
