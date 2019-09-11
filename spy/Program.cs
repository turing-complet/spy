using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using CommandLine;
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
            Parser.Default.ParseArguments<Options>(args)
                   .WithParsed<Options>(opt =>
                   {
                       if (opt.Help)
                       {
                           GetInputs();
                       }
                       else
                       {
                           Run(opt);
                       }
                   });
        }

        static void Run(Options options)
        {
            //TestPipeline();

            var config = Config.LoadFrom(options.Config);
            var errors = config.Validate();
            if (errors.Any()) throw new ConfigException(errors);

            var pipeline = new Pipeline(config);
            pipeline.Run();
            
            Console.Read();
        }


        static void TestPipeline()
        {
            var watcher = new FileInput { LogFile = @"C:\code\spy\spy\test.txt" };
            var consoleOut = new ConsoleOutput();

            var q = new ConcurrentQueue<StringFormat>();
            watcher.Start((StringFormat s) => q.Enqueue(s));
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

    // https://github.com/commandlineparser/commandline
    public class Options
    {
        [Option('h', "help", Required = false, HelpText = "Show help.")]
        public bool Help { get; set; }

        [Option('c', "config", Required = false, HelpText = "Use config file.")]
        public string Config { get; set; } = @"C:\code\spy\spy\example.json";

        [Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages. Doesn't do anything yet.")]
        public bool Verbose { get; set; }
    }

}
