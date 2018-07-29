using System;
using System.Diagnostics;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Delegates.UnitTests
{
    [TestClass]
    public class DelegatesTests
    {
        private void DoWork()
        {
            Debug.WriteLine($"Current executing thread ID of DoWork method is: {Thread.CurrentThread.ManagedThreadId.ToString()}");
            Debug.WriteLine("Hello World!");
        }

        // To define a delegate we need the following:
        // - The keyword delegate
        // - void and () describes what our method signature and return type looks like
        // - DoWorkDelegate is the name of our new type
        delegate void DoWorkDelegate();

        [TestMethod]
        public void Demo01()
        {
            Debug.WriteLine($"Current executing thread ID of Demo01 unit test method is: {Thread.CurrentThread.ManagedThreadId.ToString()}");

            // With a delegate instead of calling DoWork directly
            //DoWork();

            // We can do this
            DoWorkDelegate doWorkDelegate = new DoWorkDelegate(DoWork);

            // The delegate keyword behind the scene is generating a whole class
            //doWorkDelegate(); // or doWorkDelegate.Invoke();

            // So doWorkDelegate is actually an object that has methods in it
            // With this we can introduce asynchronous code like the begin/end pattern

            AsyncCallback asyncCallback = new AsyncCallback(TheCallback);

            IAsyncResult ar = doWorkDelegate.BeginInvoke(asyncCallback, doWorkDelegate);

            // do more work
            var result = 1 + 2;
            Debug.WriteLine($"Result is {result}");

            ar.AsyncWaitHandle.WaitOne();
        }

        // Lets say that we have one method that says please call this when the async method is finished
        private static void TheCallback(IAsyncResult ar)
        {
            var doWorkDelegate = ar.AsyncState as DoWorkDelegate;

            doWorkDelegate.EndInvoke(ar); // this is where you use a try / catch
        }
    }
}
