using System;
using System.Collections.Concurrent;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace ConcurrentDictionaryDemo
{
    class Program
    {
        // This is our shared data structure that will be used by multiple threads
        static ConcurrentDictionary<int, string> concurrentDictionary = new ConcurrentDictionary<int, string>();

        static void Main(string[] args)
        {
            // Basic Operation Examples on ConcurrentDictionary

            //if(concurrentDictionary.TryAdd(1, "value1"))
            //{
            //    Console.WriteLine("Key 1 was added.");
            //}

            //string value = concurrentDictionary.GetOrAdd(1, "VALUE1");

            //Console.WriteLine($"Value retrieved: {value}");

            // if 1 does not exist => add "value1"
            // if 1 does exist => return existingValue to upper case
            //string newVal = concurrentDictionary.AddOrUpdate(1, "VALUE1", (int existingKey, string existingValue) => {
            //    return existingValue.ToUpper();
            //});

            //Console.WriteLine($"Value retrieved: {newVal}");

            //if(concurrentDictionary.TryGetValue(1, out string val3))
            //{
            //    Console.WriteLine($"Value retrieved: {val3}");
            //}

            // Lets see an example of using a concurrent dictionary to find the count of every word in a txt file
            string executableLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string txtLocation = Path.Combine(executableLocation, "SampleTextFile_10kb.txt");

            string[] lines = File.ReadAllLines(txtLocation);

            Parallel.ForEach<string>(lines, (string line) => 
            {
                if(!string.IsNullOrWhiteSpace(line))
                {
                    string[] words = line.Split(' ');

                    foreach (var word in words)
                    {
                        if (string.IsNullOrWhiteSpace(word))
                        {
                            continue;
                        }

                        wordCount.AddOrUpdate(word, 1, (existingKey, existingCount) => 
                        {
                            return existingCount + 1;
                        });
                    }
                }
            });

            Console.ReadLine();
        }

        static ConcurrentDictionary<string, uint> wordCount = new ConcurrentDictionary<string, uint>();
    }
}
