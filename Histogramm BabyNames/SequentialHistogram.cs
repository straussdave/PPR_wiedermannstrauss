using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Histogramm_BabyNames
{
    [ShortRunJob]
    public class SequentialHistogram
    {
        private string[] allLines = null!;
        private const string FilePath = "war_and_peace.txt";

        [GlobalSetup]
        public void Setup()
        {
            // Load all lines of the text file
            allLines = File.ReadAllLines(FilePath);
        }


        [Benchmark]
        public Dictionary<string, int> CountWordsSequenziell()
        {
            var wordCounts = new Dictionary<string, int>();

            foreach (string line in allLines)
            {
                foreach (string word in line.Split(new[] { ' ', ',', '.', '!', '?', ';', ':', '"', '&' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    wordCounts.TryAdd(word, 0);
                    wordCounts[word]++;
                }
            }

            return wordCounts;
        }
    }
}
