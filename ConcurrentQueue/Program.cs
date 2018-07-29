using System;
using System.Collections.Concurrent;
using System.Threading;

namespace ConcurrentQueueDemo
{
    class Program
    {
        // This is our shared data structure that will be used by multiple threads
        static ConcurrentQueue<ulong> concurrentQueue = new ConcurrentQueue<ulong>();

        static Random random = new Random();

        static void Main(string[] args)
        {
            //concurrentQueue.Enqueue(1);

            //if(concurrentQueue.TryDequeue(out int x))
            //{
            //    Console.WriteLine(x);
            //}

            Thread threadFib = new Thread(new ThreadStart(GenerateFibMethod));
            threadFib.IsBackground = false; // lets do this in order to be a foreground thread, so as the application to not exit until it completes
            threadFib.Start();

            // lets try to read the concurrentqueue with a different thread following the same approach
            Thread threadReader = new Thread(new ThreadStart(ReadFibMethod));
            threadReader.IsBackground = false; // lets do this in order to be a foreground thread, so as the application to not exit until it completes
            threadReader.Start();

            Console.ReadLine();
        }

        private static void ReadFibMethod()
        {
            Console.WriteLine("Starting to read from the queue...");

            do
            {
                if(concurrentQueue.TryDequeue(out ulong fibNum))
                {
                    Console.Write($" [Fib: {fibNum}] ");
                }
                else
                {
                    Console.Write(" _ ");
                }
                // Lets check the queue once every 10 milliseconds
                Thread.Sleep(10);
            } while (true);
        }

        private static void GenerateFibMethod()
        {
            for (ushort i = 0; i < 50; i++)
            {
                // This will run very quickly so lets sleep for a bit here
                // so that the numbers will not show up rapidly and certainly
                // not at the same interval
                Thread.Sleep(random.Next(1, 500));

                var fibIndex = Fibonacci(i);
                concurrentQueue.Enqueue(fibIndex);

                //Console.WriteLine($"Fib({i}): {fibIndex}");
            }
        }

        private static ulong Fibonacci(ushort n)
        {
            return (n == 0) ? 0 : Fib(n);
        }

        private static ulong Fib(ushort n)
        {
            ulong a = 0;
            ulong b = 1;

            for (uint i = 0; i < n-1; i++)
            {
                ulong next = checked(a + b);

                a = b;
                b = next;
            }

            return b;
        }
    }
}
