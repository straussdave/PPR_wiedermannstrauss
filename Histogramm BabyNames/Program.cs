using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Exporters.Csv;
using BenchmarkDotNet.Running;

namespace Histogramm_BabyNames
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(Environment.ProcessorCount);
            // Create a shared configuration with the CSV exporter
            var config = DefaultConfig.Instance
                .AddExporter(CsvExporter.Default) // Adds the CSV exporter to the config
                .WithArtifactsPath("C:\\Users\\david\\source\\repos\\dining_philosophers\\Histogramm BabyNames"); // Optional: specify custom path

            // Run both benchmarks with the same configuration
            BenchmarkRunner.Run<WordCountBenchmarks>(config);
            BenchmarkRunner.Run<WordCountParallelWithTasks>(config);
            BenchmarkRunner.Run<SequentialHistogram>(config);
        }
    }
}