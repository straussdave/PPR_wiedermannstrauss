using System.ComponentModel;
using System.Diagnostics;

namespace Quicksort_In_Place_Parallel
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            //ExecuteQuicksortBenchmark(10000000);
            ExecuteMergesortBenchmark(CreateArray(100000));
        }

        private static int[] CreateArray(int arraySize)
        {
            int[] array = new int[arraySize];
            Random rnd = new Random();
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = rnd.Next(1, arraySize * 10);
            }
            return array;
        }

        private static void ExecuteMergesortBenchmark(int[] array)
        {
            SingleThreadBenchmarkMergesort(array);
            //NaiveParallelBenchmarkMergesort(array);
            ThresholdParallelBenchmarkMergesort(array, 10000);
        }

        private static void SingleThreadBenchmarkMergesort(int[] array)
        {
            Console.WriteLine("------------------- Single Thread Benchmark Mergesort START -------------------");
            int[] arrayToSort = new int[array.Length];
            Array.Copy(array, arrayToSort, array.Length);
            //Console.WriteLine("Unsortiertes Array: ");
            //printArray(arrayToSort);
            Stopwatch sw = new Stopwatch();
            sw.Start();
            arrayToSort = Mergesort(arrayToSort);
            sw.Stop();
            if (isSorted(arrayToSort))
            {
                Console.WriteLine("Sort completed in " + sw.ElapsedMilliseconds + "ms");
            }
            else
            {
                Console.WriteLine("Not sorted correctly!!!");
                Console.WriteLine("Sortiertes Array: ");
                printArray(arrayToSort);
            }
            Console.WriteLine("------------------- Single Thread Benchmark Mergesort END -------------------");
        }

        private static void NaiveParallelBenchmarkMergesort(int[] array)
        {
            Console.WriteLine("------------------- Naive Parallel Benchmark Mergesort START -------------------");
            int[] arrayToSort = new int[array.Length];
            Array.Copy(array, arrayToSort, array.Length);
            //Console.WriteLine("Unsortiertes Array: ");
            //printArray(arrayToSort);
            Stopwatch sw = new Stopwatch();
            sw.Start();
            arrayToSort = MergesortNaiveParallel(arrayToSort).Result;
            sw.Stop();
            if (isSorted(arrayToSort))
            {
                Console.WriteLine("Sort completed in " + sw.ElapsedMilliseconds + "ms");
            }
            else
            {
                Console.WriteLine("Not sorted correctly!!!");
                Console.WriteLine("Sortiertes Array: ");
                printArray(arrayToSort);
            }
            Console.WriteLine("------------------- Naive Parallel Benchmark Mergesort END -------------------");
        }

        private static void ThresholdParallelBenchmarkMergesort(int[] array, int threshold)
        {
            Console.WriteLine("------------------- Threshold Parallel Benchmark Mergesort START -------------------");
            int[] arrayToSort = new int[array.Length];
            Array.Copy(array, arrayToSort, array.Length);
            //Console.WriteLine("Unsortiertes Array: ");
            //printArray(arrayToSort);
            Stopwatch sw = new Stopwatch();
            sw.Start();
            arrayToSort = MergesortThresholdParallel(arrayToSort, threshold).Result;
            sw.Stop();
            if (isSorted(arrayToSort))
            {
                Console.WriteLine("Sort completed in " + sw.ElapsedMilliseconds + "ms");
            }
            else
            {
                Console.WriteLine("Not sorted correctly!!!");
                Console.WriteLine("Sortiertes Array: ");
                printArray(arrayToSort);
            }
            Console.WriteLine("------------------- Threshold Parallel Benchmark Mergesort END -------------------");
        }

        private static int[] Mergesort(int[] array)
        {
            if(array.Length == 1)
            {
                return array;
            }

            int[] arrayOne = SplitArray(array, 0, array.Length/2);
            int[] arrayTwo = SplitArray(array, array.Length/2, array.Length);

            arrayOne = Mergesort(arrayOne);
            arrayTwo = Mergesort(arrayTwo);

            return Merge(arrayOne, arrayTwo);
        }

        private static async Task<int[]> MergesortNaiveParallel(int[] array)
        {
            if (array.Length == 1)
            {
                return array;
            }

            Task<int[]> firstTask = Task.Run(() => SplitArray(array, 0, array.Length / 2));
            Task<int[]> secondTask = Task.Run(() => SplitArray(array, array.Length / 2, array.Length));

            Task<int[]> leftTask = Task.Run(() => MergesortNaiveParallel(firstTask.Result));
            Task<int[]> rightTask = Task.Run(() => MergesortNaiveParallel(secondTask.Result));

            await Task.WhenAll(leftTask, rightTask);
            return Merge(leftTask.Result, rightTask.Result);
        }

        private static async Task<int[]> MergesortThresholdParallel(int[] array, int threshold)
        {
            if (array.Length == 1)
            {
                return array;
            }

            if(array.Length > threshold)
            {
                int[] arrayOne =  SplitArray(array, 0, array.Length / 2);
                int[] arrayTwo = SplitArray(array, array.Length / 2, array.Length);

                Task<int[]> leftTask = Task.Run(() => MergesortNaiveParallel(arrayOne));
                Task<int[]> rightTask = Task.Run(() => MergesortNaiveParallel(arrayTwo));
                await Task.WhenAll(leftTask, rightTask);
                return Merge(leftTask.Result, rightTask.Result);
            }
            else
            {
                int[] arrayOne = SplitArray(array, 0, array.Length / 2);
                int[] arrayTwo = SplitArray(array, array.Length / 2, array.Length);

                arrayOne = Mergesort(arrayOne);
                arrayTwo = Mergesort(arrayTwo);

                return Merge(arrayOne, arrayTwo);
            }
        }

        private static int[] SplitArray(int[] array, int begin, int end)
        {
            int size = end - begin;
            int[] result = new int[size];
            for (int i = 0; i < size; i++)
            {
                result[i] = array[begin + i];
            }
            return result;
        }

        private static int[] Merge(int[] a, int[] b)
        {
            int[] c = new int[a.Length + b.Length];
            int i = 0;
            int ai = 0;
            int bi = 0;
            while(ai < a.Length && bi < b.Length )
            {
                if (a[ai] < b[bi])
                {
                    c[i] = a[ai];
                    ai++;
                }
                else
                {
                    c[i] = b[bi];
                    bi++;
                }
                i++;
            }

            while(ai < a.Length)
            {
                c[i] = a[ai];
                ai++;
                i++;
            }

            while(bi < b.Length)
            {
                c[i] = b[bi];
                bi++;
                i++;
            }

            //Console.WriteLine("----------------------------------------");
            //Console.WriteLine("a");
            //printArray(a);
            //Console.WriteLine("b");
            //printArray(b);
            //Console.WriteLine("c");
            //printArray(c);
            //Console.WriteLine("----------------------------------------");

            return c;
        }

        private static async void ExecuteQuicksortBenchmark(int arraySize)
        {
            //Array anlegen und befüllen
            int[] array = new int[arraySize];
            Random rnd = new Random();
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = rnd.Next(1, arraySize * 10);
            }

            //BenchmarkStaticQuicksort(array);
            //await BenchmarkNaiveParallelQuicksort(array);
            await BenchmarkThresholdApproachAsyncQuicksort(array);

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

        static void BenchmarkStaticQuicksort(int[] array)
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

        static async Task BenchmarkNaiveParallelQuicksort(int[] array)
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

        static async Task BenchmarkThresholdApproachAsyncQuicksort(int[] array)
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

        private static void printArray(int[] a)
        {
            Console.WriteLine(string.Join(", ", a));
        }
    }
}



