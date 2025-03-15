using System.Diagnostics;

namespace Quicksort_In_Place_Parallel
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            int arraySize = 10000000;
            //Array anlegen und befüllen
            int[] array = new int[arraySize];
            Random rnd = new Random();
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = rnd.Next(1, arraySize * 10);
            }

            //BenchmarkStatic(array);
            //await BenchmarkNaiveParallel(array);
            await BenchmarkThresholdApproachAsync(array);

            Console.WriteLine("DONE");
        }

        private static void swap(int[] array, int i, int j)
        {
            int tmp = array[i];
            array[i] = array[j];
            array[j] = tmp;
        }

        private static async Task AsyncQuicksortAsync(int[] array, int left, int right)
        {
            if (left >= right)
            {
                return;
            }
            //pivot element bestimmen 
            int pivotIndex = (left + right) / 2;
            int pivot = array[pivotIndex];
            int i = left;
            int j = right;

            while (i<=j) //bedingung noch checken
            {
                while (array[i] < pivot) i++;
                while (array[j] > pivot) j--;

                if (i <= j)
                {
                    swap(array, i, j);
                    i++;
                    j--;
                }
            }

            // Erstelle Tasks für linke und rechte Partition
            Task leftTask = (left < j) ? Task.Run(() => AsyncQuicksortAsync(array, left, j)) : Task.CompletedTask;
            Task rightTask = (i < right) ? Task.Run(() => AsyncQuicksortAsync(array, i, right)) : Task.CompletedTask;

            // Warte auf beide Tasks
            await Task.WhenAll(leftTask, rightTask);
        }
        private static async Task ThresholdQuicksortAsync(int[] array, int left, int right, int threshold)
        {
            if (left >= right)
            {
                return;
            }
            //pivot element bestimmen 
            int pivotIndex = (left + right) / 2;
            int pivot = array[pivotIndex];
            int i = left;
            int j = right;

            while (i <= j) //bedingung noch checken
            {
                while (array[i] < pivot) i++;
                while (array[j] > pivot) j--;

                if (i <= j)
                {
                    swap(array, i, j);
                    i++;
                    j--;
                }
            }

            int size = right - left;
            if (size < threshold)
            {
                SyncQuicksort(array, left, j);
                SyncQuicksort(array, i, right);
            }
            else
            {
                // Erstelle Tasks für linke und rechte Partition
                Task leftTask = (left < j) ? Task.Run(() => ThresholdQuicksortAsync(array, left, j, threshold)) : Task.CompletedTask;
                Task rightTask = (i < right) ? Task.Run(() => ThresholdQuicksortAsync(array, i, right, threshold)) : Task.CompletedTask;

                // Warte auf beide Tasks
                await Task.WhenAll(leftTask, rightTask);
            }
        }
        private static void SyncQuicksort(int[] array, int left, int right)
        {
            if (left >= right)
            {
                return;
            }
            //pivot element bestimmen 
            int pivotIndex = (left + right) / 2;
            int pivot = array[pivotIndex];
            int i = left;
            int j = right;

            while (i <= j) //bedingung noch checken
            {
                while (array[i] < pivot) i++;
                while (array[j] > pivot) j--;

                if (i <= j)
                {
                    swap(array, i, j);
                    i++;
                    j--;
                }
            }
            if (left < j)
                SyncQuicksort(array, left, j);
            if (i < right)
                SyncQuicksort(array, i, right);
        }


        static void BenchmarkStatic(int[] array)
        {
            Console.WriteLine("--------- Static Benchmark START ---------\n\n\n");
            int[] arrayToSort = new int[array.Length];
            for (int j = 0; j < array.Length; j++)
            {
                arrayToSort[j] = array[j];
            }
            Stopwatch sw = new Stopwatch();
            sw.Start();
            SyncQuicksort(arrayToSort, 0, arrayToSort.Length - 1);
            sw.Stop();
            if (isSorted(arrayToSort))
            {
                Console.WriteLine("Static Quicksort took: " + sw.ElapsedMilliseconds + "ms");
            }
            else
            {
                Console.WriteLine("Static Quicksort did not sort correctly!!!");
            }
            Console.WriteLine("\n\n\n--------- Static Benchmark END ---------");
        }

        static async Task BenchmarkNaiveParallel(int[] array)
        {
            Console.WriteLine("--------- Naive Parallel Benchmark START ---------\n\n\n");
            int[] arrayToSort = new int[array.Length];
            for (int j = 0; j < array.Length; j++)
            {
                arrayToSort[j] = array[j];
            }
            Stopwatch sw = new Stopwatch();
            sw.Start();
            await AsyncQuicksortAsync(arrayToSort, 0, arrayToSort.Length - 1);
            sw.Stop();
            if(isSorted(arrayToSort))
            {
                Console.WriteLine("Naive ParallelQuicksort took: " + sw.ElapsedMilliseconds + "ms");
            }
            else
            {
                Console.WriteLine("Naive ParallelQuicksort did not sort correctly!!!");
            }
            Console.WriteLine("\n\n\n--------- Naive Parallel Benchmark END ---------");
        }

        static async Task BenchmarkThresholdApproachAsync(int[] array)
        {
            Console.WriteLine("--------- Parallel with Threshold Benchmark START ---------\n\n\n");

            string filePath = "ThresholdResults.csv";
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine("Threshold,Time(ms)");

                for (int i = array.Length / 1000; i < array.Length / 3; i = i + array.Length / 1000)
                {
                    int[] arrayToSort = (int[])array.Clone();
                    Stopwatch sw = new Stopwatch();
                    int threshold = i;
                    sw.Start();
                    await ThresholdQuicksortAsync(arrayToSort, 0, arrayToSort.Length - 1, threshold);
                    sw.Stop();

                    long elapsedMs = sw.ElapsedMilliseconds;
                    writer.WriteLine($"{threshold},{elapsedMs}");

                    if (isSorted(arrayToSort))
                    {
                        Console.WriteLine($"ParallelQuicksort with Threshold <{threshold}> took: {elapsedMs}ms");
                    }
                    else
                    {
                        Console.WriteLine($"ParallelQuicksort with Threshold <{threshold}> did not sort correctly!!!");
                    }
                }
            }

            Console.WriteLine("\n\n\n--------- Parallel with Threshold Benchmark END ---------");
            Console.WriteLine($"Results written to {filePath}");
        }

        static bool isSorted(int[] a)
        {
            for (int i = 1; i < a.Length; i++)
            {
                if (a[i - 1] > a[i]) return false;
            }
            return true;
        }
    }
}



