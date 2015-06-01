using System;
using System.IO;
using System.Reactive.Linq;
using System.Reflection;
using Instrument.FileSystem;
using Microsoft.Practices.Prism.Modularity;

namespace Instrument
{
    public class InstrumentPriceFileAppenderModule : IModule
    {
        public void Initialize()
        {
            var disposable = new CompositeDisposable();
            var adapter = new InstrumentPriceStringAdapter();
            var fileName = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).AbsolutePath) + @"\Data\Sample Data.txt";

            var random = new Random();

            var prices = Observable.Generate(95, _ => true, i => i + random.Next(-1, 2), i => new InstrumentPrice("VOD.L", i), _ => TimeSpan.FromMilliseconds(1000))
                                   .Merge(Observable.Generate(97, _ => true, i => i + random.Next(-1, 2), i => new InstrumentPrice("BT.L", i), _ => TimeSpan.FromMilliseconds(1000)))
                                   .Merge(Observable.Generate(1450.1M, _ => true, i => i + (decimal)random.Next(-1, 2) / 10, i => new InstrumentPrice("BP.L", i), _ => TimeSpan.FromMilliseconds(1000)))
                                   .Merge(Observable.Generate(12830.2M, _ => true, i => i + (decimal)random.Next(-1, 2) / 10, i => new InstrumentPrice("Dow", i), _ => TimeSpan.FromMilliseconds(1000)))
                                   .Merge(Observable.Generate(85.6M, _ => true, i => i + (decimal)random.Next(-1, 2) / 10, i => new InstrumentPrice("Oil", i), _ => TimeSpan.FromMilliseconds(1000)));

            Observable.Using(() => new FileStream(fileName, FileMode.Append, FileAccess.Write, FileShare.ReadWrite),
                                   fs => Observable.Using(() => new StreamWriter(fs) { AutoFlush = true },
                                                                sw => prices.Select(p => new Tuple<StreamWriter, InstrumentPrice>(sw, p))))
                      .Select(x => new {StreamWriter = x.Item1, Price = adapter.Adapt(x.Item2)})
                      .Subscribe(x => x.StreamWriter.Write("\n" + x.Price))
                      .AddToDisposable(disposable);

            Observable.FromEventPattern<ExitEventHandler, ExitEventArgs>(h => Application.Current.Exit += h, h => Application.Current.Exit -= h)
                      .Take(1)
                      .Subscribe(_ => disposable.Dispose());
        }
    }
}
