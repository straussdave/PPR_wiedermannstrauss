using System.Collections.Generic;
using System;

namespace DiningPhilosophers
{
    internal class Program
    {
        static void Run(int maxThinkingTime, int philosopherIndex, int maxEatingTime, object[]forks, int numberOfPhilosophers)
        {
            bool running = true;
            while (running)
            {
                int thinkingTime = new Random().Next(0, maxThinkingTime);
                Thread.Sleep(thinkingTime);
                Console.WriteLine("phil " + philosopherIndex + " finished Thinking");

                int leftForkIndex = philosopherIndex;
                lock (forks[leftForkIndex])
                {
                    object leftFork = forks[leftForkIndex];
                    Console.WriteLine("phil " + philosopherIndex + " took first fork: " + leftForkIndex);

                    int rightForkIndex = (philosopherIndex + 1) % numberOfPhilosophers;
                    lock (forks[rightForkIndex])
                    {
                        object rightFork = forks[rightForkIndex];
                        Console.WriteLine("phil " + philosopherIndex + " took second fork: " + rightForkIndex);

                        int eatingTime = new Random().Next(0, maxEatingTime);
                        Thread.Sleep(eatingTime);

                        Console.WriteLine("phil " + philosopherIndex + " is done eating");
                    }
                }


                
            }
        }
        static void Main(string[] args)
        {
            int numberOfPhilosophers = int.Parse(Console.ReadLine());
            int maxThinkingTime = int.Parse(Console.ReadLine());
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

            foreach (Thread philosopher in philosophers)
            {
                philosopher.Join();
            }



        }
    }
}
