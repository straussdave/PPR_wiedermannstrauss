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
                Task.Factory.StartNew(() =>AsyncQuicksort(array, left, j));
            if (i < right)
                Task.Factory.StartNew(() =>AsyncQuicksort(array, i, right));
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
            int arraySize = 15000000;
            int maxNumber = 1000000000;
            //Array anlegen und befüllen
            int[] array = new int[150000000];
            Random rnd = new Random();
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = rnd.Next(1, maxNumber);
            }

            //Console.WriteLine("Unsortiertes Array:");
            //Console.WriteLine(string.Join(", ", array));
            Console.WriteLine("starting sorting");
            //Console.WriteLine("Unsortiertes Array:");
            //Console.WriteLine(string.Join(", ", array));

            // SyncQuicksort starten
            //SyncQuicksort(array, 0, array.Length - 1);
            AsyncQuicksort(array, 0, array.Length - 1);

            
            Console.WriteLine("Fertig");

            //Console.WriteLine();
            //Console.WriteLine();
            //Console.WriteLine();
            //Console.WriteLine();
            //Console.WriteLine("\nSortiertes Array:");
            //Console.WriteLine(string.Join(", ", array));

        }
    }
}



