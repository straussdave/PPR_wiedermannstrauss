// See https://aka.ms/new-console-template for more information

using System;
using System.Drawing;

namespace Mandelbrot
{
    internal class Program
    {

        static void Main(string[] args)
        {
            Program p = new Program();

            
            Console.WriteLine("Width of image: ");
            int width = int.Parse(Console.ReadLine());
            Console.WriteLine("Height of image: ");
            int height = int.Parse(Console.ReadLine());
            //Console.WriteLine("min x: ");
            //double minX = double.Parse(Console.ReadLine());
            //Console.WriteLine("max x: ");
            //double maxX = double.Parse(Console.ReadLine());
            //Console.WriteLine("min y: ");
            //double minY = double.Parse(Console.ReadLine());
            //Console.WriteLine("max y: ");
            //double maxY = double.Parse(Console.ReadLine());
            //Console.WriteLine("max iterations: ");
            //int maxIterations = int.Parse(Console.ReadLine());

            double minX = -0.85;
            double minY = -0.1;
            double maxX = -0.65;
            double maxY = 0.1;
            int maxIterations = 1000;

            Bitmap bitmap = new Bitmap(width, height);

            // array to store results
            Color[,] result = new Color[width, height];

            // process each row in parallel
            p.SerialCalculation(result, height, width, minX, maxX, minY, maxY, maxIterations);
            

            p.CreateMandelbrotImage(bitmap, result);
            string filePath = Path.Combine(p.GetDirectory(), "mandelbrot.png");
            bitmap.Save(filePath);
        }

        private void ParallelCalculation(Color[,] result, int height, int width, double minX, double maxX, double minY, double maxY, int maxIterations)
        {
            Parallel.For(0, height, y =>
            {
                for (int x = 0; x < width; x++)
                {
                    int localx = x;
                    int localy = y;
                    result[x, y] = calcPixel(localx, localy, minX, maxX, minY, maxY, width, height, maxIterations);
                }
            });
        }

        private void SerialCalculation(Color[,] result, int height, int width, double minX, double maxX, double minY, double maxY, int maxIterations)
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int localX = x;
                    int localY = y;
                    result[x, y] = calcPixel(localX, localY, minX, maxX, minY, maxY, width, height, maxIterations);
                }
            }
        }

        private string GetDirectory()
        {
            string binDirectory = Directory.GetCurrentDirectory();
            return Directory.GetParent(Directory.GetParent(Directory.GetParent(binDirectory).FullName).FullName).FullName;
        }

        private Color calcPixel(int px, int py, double minX, double maxX, double minY, double maxY, int width, int height, int maxIterations)
        {
            var tuple = normalizeToViewRectangle(px, py, minX, maxX, minY, maxY, width, height);
            double cx = tuple.Item1;
            double cy = tuple.Item2;
            double zx = cx;
            double zy = cy;
            for (int n = 0; n < maxIterations; n++)
            {
                double x = (zx * zx - zy * zy) + cx;
                double y = (zx * zx + zy * zy) + cy;
                if ((x * x + y * y) > 4)
                {
                    return GetColor(n);
                }
                zx = x;
                zy = y;
            }
            return Color.Black;
        }

        private Tuple<double,double> normalizeToViewRectangle(int px, int py, double minX, double maxX, double minY, double maxY, int width, int height)
        {
            double normalizedX = normalize(minX, maxX, px, width);
            double normalizedY = normalize(minY, maxY, py, height);
            return Tuple.Create(normalizedX,normalizedY);
        }

        private double normalize(double min, double max, double p, int size)
        {
            return min + (p / size) * (max - min);
        }

        private Color GetColor(int n)
        {
            int r = (n * 9) % 256;
            int g = (n * 6) % 256;
            int b = (n * 3) % 256;

            return Color.FromArgb(r, g, b);
        }

        private void CreateMandelbrotImage(Bitmap bitmap, Color[,] result)
        {
            for (int y = 0; y < result.GetLength(1); y++)
            {
                for (int x = 0; x < result.GetLength(0); x++)
                {
                    bitmap.SetPixel(x, y, result[x, y]);
                }
            }
        }
    }
}