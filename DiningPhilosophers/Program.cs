using System;
using System.Threading;

namespace DiningPhilosophers
{
    internal class Program
    {
        static volatile bool running = true;
        static readonly object syncLock = new object();

        static void Run(int maxThinkingTime, int philosopherIndex, int maxEatingTime, object[] forks, int numberOfPhilosophers)
        {
            Random rand = new Random();

            while (running)
            {
                // Thinking phase
                int thinkingTime = rand.Next(0, maxThinkingTime);
                Thread.Sleep(thinkingTime);
                Console.WriteLine($"phil {philosopherIndex} finished Thinking");

                int leftForkIndex = philosopherIndex;
                object leftFork = forks[leftForkIndex];

                if (!running) return; // Exit if stopping

                Monitor.Enter(leftFork);
                Console.WriteLine($"phil {philosopherIndex} took first fork: {leftForkIndex}");
                Thread.Sleep(10); // Simulate delay

                int rightForkIndex = (philosopherIndex + 1) % numberOfPhilosophers;
                object rightFork = forks[rightForkIndex];

                bool hasRightFork = false;

                lock (syncLock)
                {
                    while (running)
                    {
                        if (Monitor.TryEnter(rightFork))
                        {
                            hasRightFork = true;
                            break;
                        }

                        Console.WriteLine($"phil {philosopherIndex} waiting for second fork: {rightForkIndex}");
                        Monitor.Wait(syncLock, 100);
                    }
                }

                if (!running)
                {
                    Monitor.Exit(leftFork);
                    return;
                }

                if (hasRightFork)
                {
                    Console.WriteLine($"phil {philosopherIndex} took second fork: {rightForkIndex}");

                    int eatingTime = rand.Next(0, maxEatingTime);
                    Thread.Sleep(eatingTime);
                    Console.WriteLine($"phil {philosopherIndex} is done eating");

                    Monitor.Exit(rightFork);
                }

                Monitor.Exit(leftFork);
            }
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Number of philosophers: ");
            int numberOfPhilosophers = int.Parse(Console.ReadLine());
            Console.WriteLine("Maximum thinking time: ");
            int maxThinkingTime = int.Parse(Console.ReadLine());
            Console.WriteLine("Maximum eating time: ");
            int maxEatingTime = int.Parse(Console.ReadLine());

            object[] forks = new object[numberOfPhilosophers];
            for (int i = 0; i < numberOfPhilosophers; i++)
            {
                forks[i] = new object();
            }

            Thread[] philosophers = new Thread[numberOfPhilosophers];
            for (int i = 0; i < numberOfPhilosophers; i++)
            {
                int localIndex = i;
                philosophers[i] = new Thread(() => Run(maxThinkingTime, localIndex, maxEatingTime, forks, numberOfPhilosophers));
            }

            foreach (Thread philosopher in philosophers)
            {
                philosopher.Start();
            }

            Console.ReadLine();
            running = false;

            lock (syncLock)
            {
                Monitor.PulseAll(syncLock);
            }

            foreach (Thread philosopher in philosophers)
            {
                philosopher.Join();
            }

            Console.WriteLine("All philosophers stopped.");
        }
    }
}
