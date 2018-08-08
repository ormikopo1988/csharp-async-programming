using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncAwaitSyntax
{
    class Program
    {
        // deelay is a Delay proxy for http resources
        // Slow loading resources (images, scripts, etc) can break your code. 
        // Test it simulating unexpected network conditions applied to specific resource.
        // See more in http://www.deelay.me/
        static string url = "http://www.deelay.me/5000/https://github.com";

        static void Main(string[] args)
        {
            Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId} enters Main.");

            MainAsync().Wait();

            Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId} exiting Main.");

            Console.ReadLine();
        }

        static async Task MainAsync()
        {
            Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId} enters MainAsync.");

            await DownloadAsync();

            Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId} exits MainAsync.");
        }

        private static async Task DownloadAsync()
        {
            Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId} enters DownloadAsync.");

            byte[] rawData = await DoTheJobAsync(url);

            Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId} result is: {rawData.Length}");

            Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId} exits DownloadAsync.");
        }

        private static async Task<byte[]> DoTheJobAsync(string url)
        {
            Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId} enters DoTheJobAsync.");

            var downloader = new WebClient();

            // The await keyword here means that this is a point 
            // in which the method can be split.
            // With the use of async and await the compiler is smart enough
            // to figure out to rewrite the method in multiple parts 
            // and as a result doing all the heavy lifting of dealing with
            // Tasks and asynchronous operations.
            var result = await downloader.DownloadDataTaskAsync(url);

            Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId} exits DoTheJobAsync.");

            return result;
        }
    }
}
