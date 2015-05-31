using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;

namespace Instrument.FileSystem
{
    public class ObservableFileStreamReader<T>
    {
        private readonly string _fileName;
        private readonly Func<string, T> _adapter;

        public ObservableFileStreamReader(string fileName, Func<string, T> adapter)
        {
            _fileName = fileName;
            _adapter = adapter;
        }

        public IObservable<T> Observe()
        {
            return Observable.Using(() => new FileStream(_fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite),
                                          fs => ReadLines(fs).ToObservable(new EventLoopScheduler()))
                             .Where(s => !string.IsNullOrEmpty(s))
                             .Select(s => _adapter(s));
        }

        IEnumerable<string> ReadLines(Stream stream)
        {
            using (var sr = new StreamReader(stream))
            {
                while (true)
                {
                    while (sr.EndOfStream)
                    {
                        Thread.Sleep(TimeSpan.FromMilliseconds(100));
                    }
                    yield return sr.ReadLine();
                }
            }
        }
    }
}
