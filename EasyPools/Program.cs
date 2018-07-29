using System;
using System.Threading;

namespace EasyPools
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine($"Main => Current thread id: {Thread.CurrentThread.ManagedThreadId}.");

            ThreadPool.QueueUserWorkItem(new WaitCallback(DoSomeWork));

            Console.WriteLine($"Main => Is it a background thread? {Thread.CurrentThread.IsBackground}.");

            // Please wait for one signal
            manualResetEvent.WaitOne();

            // Console.ReadLine() is a blocking operation for the main foreground thread.
            // That means that it will wait for the user to press a button for it to exit.
            // So the thread pool threads which are background threads by default will have
            // their chance to run and return. So with the below command we will get the console.writeline
            // messages from the DoSomeWork() callback method. If we comment out this line then the main
            // thread will most probably exit, thus the process and the program will exit and the 
            // background thread will not have the change to finish.
            Console.ReadLine();
        }

        // What this will do is literally set a flag that is manually set and manually reset
        // So signaled or not signaled / flagged or not flagged?
        private static ManualResetEvent manualResetEvent = new ManualResetEvent(false);

        private static void DoSomeWork(object state)
        {
            Console.WriteLine($"DoSomeWork => Current thread id: {Thread.CurrentThread.ManagedThreadId}.");
            Console.WriteLine($"DoSomeWork => Is it a background thread? {Thread.CurrentThread.IsBackground}.");

            // When the job is finished, we can now signal from this background thread
            // over to the main thread - Hey I want to set that flag, I want to signal
            // that if anyone is waiting, they can continue with what they were doing
            manualResetEvent.Set();
        }
    }
}
