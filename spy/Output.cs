using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;
using spy.format;

namespace spy.output
{
    public abstract class Output<T> where T:IFormat
    {
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
                }
            });
        }
    }

    public class ConsoleOutput : Output<StringFormat>
    {
        public override void Forward(StringFormat data)
        {
            Console.WriteLine(data.Entry);
        }
    }
}