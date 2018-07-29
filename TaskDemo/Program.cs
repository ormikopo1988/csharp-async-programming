using System;
using System.Threading.Tasks;

namespace TaskDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            // The action inside the task is the work that this task is supposed to do
            Task task1 = new Task(() => 
            {
                Console.WriteLine("Task One");
            });

            Task task2 = task1.ContinueWith((t) =>
            {
                Console.WriteLine("Task continued.");
            });

            task1.Start();

            Task.WaitAll(task1, task2);

            // Another way to run a task if we do not need the object reference is
            Task.Run(() =>
            {
                Console.WriteLine("Simple task to run.");
            });

            // You can also use a TaskFactory to create a task.
            // One extra possibility with the TaskFactory is that you can
            // add your own scheduler, for being able to manipulate the order of your tasks.
            TaskFactory tf = new TaskFactory(TaskScheduler.Default);
            tf.StartNew(() =>
            {
                Console.WriteLine("Task from a task factory.");
            });

            Console.ReadLine();
        }
    }
}
