using System.Diagnostics;

namespace Quicksort_In_Place_Parallel
{
    internal class Program
    {
        private static void swap(int[] array, int i, int j)
        {
            int tmp = array[i];
            array[i] = array[j];
            array[j] = tmp;
        }

        private static void AsyncQuicksort(int[] array, int left, int right)
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
            if (left < j)
                Parallel.Invoke(() =>AsyncQuicksort(array, left, j));
            if (i < right)
                Parallel.Invoke(() =>AsyncQuicksort(array, i, right));
        }
        private static void ThresholdQuicksort(int[] array, int left, int right, int threshold)
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
                if ((j-left) < threshold)
                    SyncQuicksort(array, left, j);
                else
                    //Task.Factory.StartNew(() => ThresholdQuicksort(array, left, j, threshold));
                    Parallel.Invoke(() => AsyncQuicksort(array, left, j));
            if (i < right)
                if ((right - i) < threshold)
                    SyncQuicksort(array, i, right);
                else
                    //Task.Factory.StartNew(() => ThresholdQuicksort(array, i, right, threshold));
                    Parallel.Invoke(() => AsyncQuicksort(array, i, right));
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
        static void Main(string[] args)
        {
            int arraySize = 1000000;
            int maxNumber = 10000000;
            //int arraySize = 10;
            //int maxNumber = 50;
            //Array anlegen und befüllen
            int[] array = new int[arraySize];
            Random rnd = new Random();
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = rnd.Next(1, maxNumber);
            }

            int[] arrayToSort = new int[arraySize];
            //for (int j = 0; j < array.Length; j++)
            //{
            //    arrayToSort[j] = array[j];
            //}
            //Stopwatch sw2 = new Stopwatch();
            //sw2.Start();
            //AsyncQuicksort(arrayToSort, 0, arrayToSort.Length - 1);
            //sw2.Stop();
            //Console.WriteLine("Naive ParallelQuicksort took: " + sw2.ElapsedMilliseconds + " Milli Seconds");
            //Console.WriteLine("\nSortiertes Array:");
            //Console.WriteLine(string.Join(", ", arrayToSort));
            for (int i = 0; i < 100; i++)
            {
                for (int j = 0; j < array.Length; j++)
                {
                    arrayToSort[j] = array[j];
                }
                //Console.WriteLine("Unsortiertes Array:");
                //Console.WriteLine(string.Join(", ", arrayToSort));
                Stopwatch sw1 = new Stopwatch();
                int threshold = i;
                sw1.Start();
                ThresholdQuicksort(arrayToSort, 0, arrayToSort.Length - 1, threshold);
                //SyncQuicksort(arrayToSort, 0, arrayToSort.Length - 1);
                sw1.Stop();
                //Console.WriteLine("\nSortiertes Array:");
                Console.WriteLine("Threshold: "+threshold+" Time: " + sw1.ElapsedMilliseconds + "\n\n\n");
            }
            Console.WriteLine("DONE");

            //Console.WriteLine();
            //Console.WriteLine();
            //Console.WriteLine();
            //Console.WriteLine();
            //Console.WriteLine("\nSortiertes Array:");
            //Console.WriteLine(string.Join(", ", array));

        }
    }
}



