using System;
using System.Net;

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
            Download();

            Console.ReadLine();
        }

        private static async void Download()
        {
            var downloader = new WebClient();

            // The await keyword here means that this is a point 
            // in which the method can be split.
            // With the use of async and await the compiler is smart enough
            // to figure out to rewrite the method in multiple parts 
            // and as a result doing all the heavy lifting of dealing with
            // Tasks and asynchronous operations.
            byte[] rawData = await downloader.DownloadDataTaskAsync(url);

            Console.WriteLine(rawData.Length);
        }
    }
}
