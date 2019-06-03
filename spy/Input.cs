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
    public interface IInput<T>
    {
        Dictionary<string, string> Settings { get; set; }
        void Start(Action<T> handler);
    }

    // csv or other files derive from base, pass in format parser
    // abstract class BaseFileInput<T> : IInput<T>
    // class CsvInput : BaseFileInput<CsvRow>
    // ctor: new CsvInput((CsvRow row) => { /* doSomethingWith(row); */  })

    [SpyInput(name = "textfile")]
    public class FileInput : IInput<string>
    {
        private string _path;

        public FileInput(string fullPath)
        {
            _path = fullPath;
        }

        public Dictionary<string, string> Settings { get; set; }

        public void Start(Action<string> handler)
        {
            Task.Run(() => _start(handler));
        }
        public void _start(Action<string> handler)
        {
            var wh = new AutoResetEvent(false);
            var fsw = new FileSystemWatcher(Path.GetDirectoryName(_path));
            fsw.Filter = Path.GetFileName(_path);
            fsw.EnableRaisingEvents = true;
            fsw.Changed += (s,e) => wh.Set();

            var fs = new FileStream(_path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using (var sr = new StreamReader(fs))
            {
                var s = "";
                while (true)
                {
                    s = sr.ReadLine();
                    if (s != null)
                        handler(s);
                    else
                        wh.WaitOne(1000);
                }
            }
        }
    }

}