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
        void Start(Action<T> handler);
    }

    // csv or other files derive from base, pass in format parser
    // abstract class BaseFileInput<T> : IInput<T>
    // class CsvInput : BaseFileInput<CsvRow>
    // ctor: new CsvInput((CsvRow row) => { /* doSomethingWith(row); */  })

    public class FileInput : IInput<string>
    {
        private string _dir;
        private string _file;
        private HashSet<string> fields;
        public FileInput(string fullPath, HashSet<string> fields)
        {
            this._dir = Path.GetDirectoryName(fullPath);
            this._file = Path.GetFileName(fullPath);
            this.fields = fields;
        }

        public void Start(Action<string> handler)
        {
            Task.Run(() => _start(handler));
        }
        public void _start(Action<string> handler)
        {
            var wh = new AutoResetEvent(false);
            var fsw = new FileSystemWatcher(_dir);
            fsw.Filter = _file;
            fsw.EnableRaisingEvents = true;
            fsw.Changed += (s,e) => wh.Set();

            var fs = new FileStream(_file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
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