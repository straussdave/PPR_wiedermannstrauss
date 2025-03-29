using BenchmarkDotNet.Attributes;
using System.Collections.Concurrent;

[ShortRunJob]
public class WordCountParallelWithTasks
{
    private string[] allLines = null!;
    private const string FilePath = "war_and_peace.txt";

    [GlobalSetup]
    public void Setup()
    {
        allLines = File.ReadAllLines(FilePath);
    }

    [Benchmark]
    public Dictionary<string, int> CountWordsParallel()
    {
        int totalLines = allLines.Length;
        int taskCount = Environment.ProcessorCount;
        int batchSize = totalLines / taskCount;
        var tasks = new Task<Dictionary<string, int>>[taskCount];

        for (int i = 0; i < taskCount; i++)
        {
            int start = i * batchSize;
            int end = Math.Min(start + batchSize, totalLines);

            tasks[i] = Task.Run(() => CountWordsInLines(allLines, start, end));
        }

        Task.WaitAll(tasks);

        // ZusammenfÃ¼hren der Ergebnisse
        var combinedCounts = new Dictionary<string, int>();

        foreach (var task in tasks)
        {
            var localCounts = task.Result;
            foreach (var kvp in localCounts)
            {
                if (!combinedCounts.ContainsKey(kvp.Key))
                    combinedCounts[kvp.Key] = 0;
                combinedCounts[kvp.Key] += kvp.Value;
            }
        }

        return combinedCounts;

        //return new Dictionary<string, int>();
    }

    private Dictionary<string, int> CountWordsInLines(string[] lines, int start, int end)
    {
        var wordCounts = new Dictionary<string, int>();

        for (int i = start; i < end; i++)
        {
            var words = lines[i].Split([' ', ',', '!', '.', ':', ';', '?', '"', '&']);
            foreach (var word in words)
            {
                if (!string.IsNullOrWhiteSpace(word))
                {
                    if (!wordCounts.ContainsKey(word))
                        wordCounts[word] = 0;
                    wordCounts[word]++;
                }
            }
        }

        return wordCounts;
    }
}