using System.Collections.Generic;
using System;
using System.Diagnostics;

namespace DiningPhilosophers
{
    internal class Program
    {

        /// <summary>
        /// This method unlocks the lockObject in case of an exception
        /// </summary>
        /// <param name="lockObj"> Object to lock </param>
        //static void MyLock2(object lockObj)
        //{
        //    var lockTaken = false;
        //    try
        //    {
        //        Monitor.Enter(lockObj, ref lockTaken);
        //        return;
        //    }
        //    catch (Exception ex)
        //    {
        //        if (lockTaken)
        //            Monitor.Exit(lockObj);
        //    }
        //}

        static Stopwatch Run(int maxThinkingTime, int philosopherIndex, int maxEatingTime, object[] forks, int numberOfPhilosophers, Stopwatch stopwatch, CancellationTokenSource cts)
        {
            while (!cts.IsCancellationRequested)
            {

                int firstForkIndex = philosopherIndex % 2 == 0 ? ((philosopherIndex + 1) % numberOfPhilosophers) : (philosopherIndex); //solving Circular Wait
                object firstFork = forks[firstForkIndex];

                int secondForkIndex = philosopherIndex % 2 == 0 ? (philosopherIndex) : ((philosopherIndex + 1) % numberOfPhilosophers); //solving Circular Wait
                object secondFork = forks[secondForkIndex];

                int thinkingTime = new Random().Next(0, maxThinkingTime);
                Thread.Sleep(thinkingTime);
                Console.WriteLine("phil " + philosopherIndex + " finished Thinking");

                bool firstLockTaken = false;
                bool secondLockTaken = false;

                try
                {
                    Monitor.Enter(firstFork, ref firstLockTaken);
                    Console.WriteLine("phil " + philosopherIndex + " took first fork: " + firstForkIndex);
                    Monitor.Enter(secondFork, ref secondLockTaken);
                    Console.WriteLine("phil " + philosopherIndex + " took second fork: " + secondForkIndex);

                    int eatingTime = new Random().Next(0, maxEatingTime);
                    stopwatch.Start();//measuring eating time
                    Thread.Sleep(eatingTime);
                    stopwatch.Stop();//measuring eating time
                    Console.WriteLine("phil " + philosopherIndex + " is done eating");
                }
                finally
                {
                    if (firstLockTaken)
                    {
                        Monitor.Exit(firstFork);
                    }
                    if (secondLockTaken)
                    {
                        Monitor.Exit(secondFork);
                    }
                }

                
            }
            

            return stopwatch;


        }

        static void Main(string[] args)
        {
            CancellationTokenSource cts = new CancellationTokenSource();
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
                philosophers[i] = new Thread(() => Run(maxThinkingTime, localIndex, maxEatingTime, forks, numberOfPhilosophers, stopwatchesToMeasureEating[localIndex], cts));
                philosophers[i].Name = i.ToString();
            }

            Stopwatch measuringDinnerTime = new Stopwatch();    
            measuringDinnerTime.Start();

            foreach (Thread philosopher in philosophers)
            {
                philosopher.Start();
            }

            Console.ReadKey();
            cts.Cancel();

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
            Console.WriteLine("eating time per philosopher: " + eatingTime/numberOfPhilosophers);
            Console.WriteLine("Total dinner time was: " + measuringDinnerTime.ElapsedMilliseconds);


        }
    }
}
