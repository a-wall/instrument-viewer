using System;
using System.Reactive.Linq;
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
            var random = new Random();
            
            var prices = Observable.Generate(95, _ => true, i => i + random.Next(-1, 2), i => new InstrumentPrice("VOD.L", i), _ => TimeSpan.FromMilliseconds(100))
                                   .Merge(Observable.Generate(97, _ => true, i => i + random.Next(-1, 2), i => new InstrumentPrice("BT.L", i), _ => TimeSpan.FromMilliseconds(100)))
                                   .Merge(Observable.Generate(1450.1M, _ => true, i => i + (decimal)random.Next(-1, 2) / 10, i => new InstrumentPrice("BP.L", i), _ => TimeSpan.FromMilliseconds(100)))
                                   .Merge(Observable.Generate(12830.2M, _ => true, i => i + (decimal)random.Next(-1, 2) / 10, i => new InstrumentPrice("Dow", i), _ => TimeSpan.FromMilliseconds(100)))
                                   .Merge(Observable.Generate(85.6M, _ => true, i => i + (decimal)random.Next(-1, 2) / 10, i => new InstrumentPrice("Oil", i), _ => TimeSpan.FromMilliseconds(100)))
                                   .Publish()
                                   .RefCount();
            
            _container.RegisterInstance<IObservable<InstrumentPrice>>(prices);
        }
    }
}
