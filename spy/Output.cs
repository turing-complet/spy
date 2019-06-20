using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using spy.format;

namespace spy.output
{
    public abstract class Output<T> where T:IFormat
    {
        [SpySetting(name = "interval", description = "polling interval")]
        public int Interval { get; set; } = 10;
        public abstract void Forward(T data);

        public void Start(ConcurrentQueue<T> queue)
        {
            Task.Run(() => {
                while (true)
                {
                    T entry;
                    queue.TryDequeue(out entry);
                    if (entry != null)
                    {
                        Forward(entry);
                    }
                    Thread.Sleep(Interval);
                }
            });
        }
    }

    [SpyOutput(name = "console")]
    public class ConsoleOutput : Output<StringFormat>
    {
        public override void Forward(StringFormat data)
        {
            Console.WriteLine(data.Entry);
        }
    }
}