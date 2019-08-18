using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using spy.format;

namespace spy.input
{
    public interface IInput<out T>
    {
        void Start(Action<T> handler);
    }

    // csv or other files derive from base, pass in format parser
    // abstract class BaseFileInput<T> : IInput<T>
    // class CsvInput : BaseFileInput<CsvRow>
    // ctor: new CsvInput((CsvRow row) => { /* doSomethingWith(row); */  })

    [SpyInput(name = "textfile")]
    public class FileInput : IInput<StringFormat>
    {
        private string _path;

        [SpySetting(name = "path", description = "path to file to watch")]
        public string LogFile { get; set; }

        [SpySetting(name = "interval", description = "polling interval")]
        public int Interval { get; set; } = 10;

        public FileInput()
        {
            _path = LogFile;
        }

        public void Start(Action<StringFormat> handler)
        {
            Task.Run(() => _start(handler));
        }
        public void _start(Action<StringFormat> handler)
        {
            var wh = new AutoResetEvent(false);
            var fsw = new FileSystemWatcher(Path.GetDirectoryName(_path));
            fsw.Filter = Path.GetFileName(_path);
            fsw.EnableRaisingEvents = true;
            fsw.Changed += (s,e) => wh.Set();
            Console.WriteLine($"First time on thread id = {Thread.CurrentThread.ManagedThreadId}");
            var fs = new FileStream(_path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using (var sr = new StreamReader(fs))
            {
                string line;
                while (true)
                {
                    line = sr.ReadLine();
                    if (!string.IsNullOrEmpty(line))
                        handler(line);
                    else
                        wh.WaitOne(Interval);
                }
            }
        }
    }

}