using System;
using System.Collections.Concurrent;
using System.Threading;

namespace ProducerConsumerExample
{
    class Program
    {
        static BlockingCollection<ulong> blockingCollection = new BlockingCollection<ulong>(10);

        static Random random = new Random();

        static void Main(string[] args)
        {
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
            // lets make reading a lot slower in order for generate to run out of capacity before it can continue adding new values
            Thread.Sleep(10000);

            do
            {
                // Take a value if it is available, or block and wait until a new value is available to consume
                var number = blockingCollection.Take();
                Console.WriteLine($"[Fib: {number}]");
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

                Console.WriteLine($"Adding next fib...");

                // it will Add up to 10 items and then stop until an element has been removed from the collection
                blockingCollection.Add(Fibonacci(i));
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

            for (uint i = 0; i < n - 1; i++)
            {
                ulong next = checked(a + b);

                a = b;
                b = next;
            }

            return b;
        }
    }
}
