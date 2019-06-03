using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using spy.format;
using spy.input;
using spy.output;

namespace spy
{

    // polling vs recording events (accept http posts)
    // how to connect with buffers
    // just map the common fields between an input and output
    // where should spy itself write logs?
    // should still add transformations
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            //GetInputs();
            //StartPipeline();

            var config = Config.LoadFrom(@"C:\code\spy\spy\example.json");
            var errors = config.Validate();

            Console.Read();
            
        }


        static void StartPipeline()
        {
            var watcher = new FileInput(@"C:\code\spy\spy\test.txt");
            var consoleOut = new ConsoleOutput();

            var q = new ConcurrentQueue<StringFormat>();
            watcher.Start((string s) => q.Enqueue(s));
            consoleOut.Start(q);
        }

        static void GetInputs()
        {
            var types = Assembly.GetExecutingAssembly().GetTypes()
                .SelectMany(t => t.GetCustomAttributes())
                .Where(t => t.GetType() == typeof(SpyInput))
                .Select(t => ((SpyInput)t).name);

            foreach (var t in types)
            {
                Console.WriteLine(t);
            }
        }
    }

}
