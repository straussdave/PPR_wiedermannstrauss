using BenchmarkDotNet.Running;

namespace Histogramm_BabyNames
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<WordCountBenchmarks>();
        }
    }
}
