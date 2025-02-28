using System.Collections.Generic;
using System;

namespace DiningPhilosophers
{
    internal class Program
    {
        static volatile bool running = true;

        static void Run(int maxThinkingTime, int philosopherIndex, int maxEatingTime, object[]forks, int numberOfPhilosophers)
        {
            while (running)
            {
                int thinkingTime = new Random().Next(0, maxThinkingTime);
                Thread.Sleep(thinkingTime);
                Console.WriteLine("phil " + philosopherIndex + " finished Thinking");

                int leftForkIndex = philosopherIndex;
                object leftFork = forks[leftForkIndex];
                Monitor.Enter(leftFork);
                Console.WriteLine("phil " + philosopherIndex + " took first fork: " + leftForkIndex);
                Thread.Sleep(10);

                int rightForkIndex = (philosopherIndex + 1) % numberOfPhilosophers;
                object rightFork = forks[rightForkIndex];
                Monitor.Enter(rightFork);
                Console.WriteLine("phil " + philosopherIndex + " took second fork: " + rightForkIndex);

                int eatingTime = new Random().Next(0, maxEatingTime);
                Thread.Sleep(eatingTime);
                Console.WriteLine("phil " + philosopherIndex + " is done eating");

                Monitor.Exit(leftFork);
                Monitor.Exit(rightFork);
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

            foreach (Thread philosopher in philosophers)
            {
                philosopher.Join();
            }



        }
    }
}
