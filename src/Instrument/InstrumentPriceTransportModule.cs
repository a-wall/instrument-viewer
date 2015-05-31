using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using System.Text;
using Instrument.FileSystem;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Unity;

namespace Instrument
{
    public class InstrumentPriceTransportModule : IModule
    {
        private readonly IUnityContainer _container;

        public InstrumentPriceTransportModule(IUnityContainer container)
        {
            _container = container;
        }

        public void Initialize()
        {
            // Uncomment if you wish to test with randomly generated prices
            //var random = new Random();
            //var prices = Observable.Generate(100, _ => true, i => i + random.Next(-1, 2), i => new InstrumentPrice("VOD.L", i), _ => TimeSpan.FromMilliseconds(100))
            //                       .Merge(Observable.Generate(70, _ => true, i => i + random.Next(-1, 2), i => new InstrumentPrice("BT.L", i), _ => TimeSpan.FromMilliseconds(100)))
            //                       .Publish()
            //                       .RefCount();
            //_container.RegisterInstance<IObservable<InstrumentPrice>>(prices);

            var adapter = new InstrumentPriceStringAdapter();
            var fileName = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).AbsolutePath) + @"\Data\Sample Data.txt";

            var fileStreamReader = new ObservableFileStreamReader<InstrumentPrice>(fileName, adapter.Adapt);

            _container.RegisterInstance<IObservable<InstrumentPrice>>(fileStreamReader.Observe());
        }
    }
}
