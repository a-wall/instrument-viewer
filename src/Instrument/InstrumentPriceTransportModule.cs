using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
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
            var prices = Observable.Generate(100, _ => true, i => i + random.Next(-1, 2), i => new InstrumentPrice("VOD.L", i), _ => TimeSpan.FromMilliseconds(100))
                                   .Merge(Observable.Generate(70, _ => true, i => i + random.Next(-1, 2), i => new InstrumentPrice("BT.L", i), _ => TimeSpan.FromMilliseconds(100)))
                                   .Publish()
                                   .RefCount();
            
            _container.RegisterInstance<IObservable<InstrumentPrice>>(prices);
        }
    }
}
