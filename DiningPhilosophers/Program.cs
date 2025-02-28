using System.Collections.Generic;
using System;
using System.Diagnostics;

namespace DiningPhilosophers
{
    internal class Program
    {
        static volatile bool running = true;

        static Stopwatch Run(int maxThinkingTime, int philosopherIndex, int maxEatingTime, object[]forks, int numberOfPhilosophers, Stopwatch stopwatch)
        {
            while (running)
            {
                int thinkingTime = new Random().Next(0, maxThinkingTime);
                Thread.Sleep(thinkingTime);
                Console.WriteLine("phil " + philosopherIndex + " finished Thinking");

                //int leftForkIndex = philosopherIndex; 
                int firstForkIndex = philosopherIndex % 2 == 0 ? ((philosopherIndex + 1) % numberOfPhilosophers) : (philosopherIndex); //solving Circular Wait

                object leftFork = forks[firstForkIndex];
                Monitor.Enter(leftFork);
                Console.WriteLine("phil " + philosopherIndex + " took first fork: " + firstForkIndex);
                Thread.Sleep(10);

                //int rightForkIndex = (philosopherIndex + 1) % numberOfPhilosophers; 
                int secondForkIndex = philosopherIndex % 2 == 0 ? (philosopherIndex) : ((philosopherIndex + 1) % numberOfPhilosophers); //solving Circular Wait

                object rightFork = forks[secondForkIndex];
                Monitor.Enter(rightFork);
                Console.WriteLine("phil " + philosopherIndex + " took second fork: " + secondForkIndex);

                int eatingTime = new Random().Next(0, maxEatingTime);
                stopwatch.Start();//measuring eating time
                Thread.Sleep(eatingTime);
                stopwatch.Stop();//measuring eating time
                Console.WriteLine("phil " + philosopherIndex + " is done eating");

                Monitor.Exit(leftFork);
                Monitor.Exit(rightFork);
            }

            return stopwatch;
            
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
            Stopwatch[] stopwatchesToMeasureEating = new Stopwatch[numberOfPhilosophers];

            for (int i = 0; i < numberOfPhilosophers; i++)
            {
                int localIndex = i;
                stopwatchesToMeasureEating[i] = new Stopwatch();
                philosophers[i] = new Thread(() => Run(maxThinkingTime, localIndex, maxEatingTime, forks, numberOfPhilosophers, stopwatchesToMeasureEating[localIndex]));
                philosophers[i].Name = i.ToString();
            }

            Stopwatch measuringDinnerTime = new Stopwatch();    
            measuringDinnerTime.Start();

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

            measuringDinnerTime.Stop();
            
            long eatingTime = 0;
            foreach (Stopwatch stopwatch in stopwatchesToMeasureEating)
            {
               eatingTime += stopwatch.ElapsedMilliseconds;
            }

            Console.WriteLine("Total eating time was: " + eatingTime);
            Console.WriteLine("Average eating time was: " + eatingTime/numberOfPhilosophers);
            Console.WriteLine("Total dinner time was: " + measuringDinnerTime.ElapsedMilliseconds);


        }
    }
}
