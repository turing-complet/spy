using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
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
    // toml config
    // should still add transformations
    // base input/ouput classes for thread start, managing buffer, etc
    // write pipeline.Run() first to see what interfaces should look like
    // chain output to input as a handler. multiple outputs => use "compound" action
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var fields = new HashSet<string> { "timestamp", "level", "msg" };
            // pipeline.AddInput(new CsvInput("./logs.txt", fields));
            // pipeline.AddOutput(new AppInsights("instrumentation-key"));
            // pipeline.AddOutput(new ConsoleOutput());
            // pipeline.Run();

            var watcher = new FileInput("/home/jon/code/spy/test.txt", fields);
            var consoleOut = new ConsoleOutput();

            var q = new ConcurrentQueue<StringFormat>();
            watcher.Start((string s) => q.Enqueue(s));
            consoleOut.Start(q);

            Console.Read();
            
        }
    }

}
