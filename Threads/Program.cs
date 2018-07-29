using System;
using System.Collections.Generic;
using System.Threading;

namespace Threads
{
    class Program
    {
        private static readonly object customLock = new object();

        static void Main(string[] args)
        {
            Console.WriteLine("Starting in the main thread.");

            var thread1 = new Thread(AddToList);
            thread1.Start();

            var thread2 = new Thread(AddToList);
            thread2.Start();

            var thread3 = new Thread(AddToList);
            thread3.Start();

            var thread4 = new Thread(AddToList);
            thread4.Start();

            var thread5 = new Thread(AddToList);
            thread5.Start();

            //new Thread(AddToList).Start();
            //new Thread(AddToList).Start();
            //new Thread(AddToList).Start();
            //new Thread(AddToList).Start();
            //new Thread(AddToList).Start();

            Console.WriteLine("Ending of the main thread.");

            Console.ReadLine();
        }

        private static List<int> numbers = new List<int>();

        private static void AddToList()
        {
            // This would sometimes throw an index out of bounds exception because List<T> implementation
            // is not thread safe.
            //try
            //{
            //    for (int i = 0; i < 100; i++)
            //    {
            //        numbers.Add(i);
            //    }
            //}
            //catch(Exception ex)
            //{
            //    Console.WriteLine(ex.Message);
            //}

            // How to avoid this? Use the lock keyword - Use the lock here and wrap all the for loop
            // if we want each thread to put their values inside the collection in sequence
            //lock (customLock)
            //{
            //    for (int i = 0; i < 100; i++)
            //    {
            //        numbers.Add(i);
            //    }
            //}

            // If we do not care about the sequence we can move the lock one step inside the for 
            // wrapping only the .Add operation as to be thread safe.
            
            for (int i = 0; i < 100; i++)
            {
                // Lets make it more interesting and make sure that each thread does not run 
                // at its full speed and as a result be able to add at once all of its values.
                // Lets make it a race.
                Thread.Sleep(random.Next(1, 500));
                lock (customLock)
                {
                    numbers.Add(i);
                }
            }
        }

        private static Random random = new Random();

        // what happens with shared state?
        private static int total = 0;

        private static void DoSomeWork()
        {
            // No danger in this - Each thread has its local variable stack
            //int x = 10;
            //x++;

            // this is dangerous
            Thread.Sleep(random.Next(1, 2500));
            int myTotal = total;
            Thread.Sleep(random.Next(1, 2500));
            total = myTotal + 1;

            //Console.WriteLine($"Thread with ID {Thread.CurrentThread.ManagedThreadId} is doing work on variable x with value {x}.");
            Console.WriteLine($"Thread with ID {Thread.CurrentThread.ManagedThreadId} says {total}.");
        }
    }
}
