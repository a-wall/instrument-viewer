using System;
using System.Reactive.Linq;

namespace Instrument.Services
{
    public class InstrumentPriceService : IInstrumentPriceService
    {
        readonly Random _random = new Random();
        
        public IObservable<InstrumentPrice> ObserveInstrumentPrices()
        {
            return Observable.Generate(100, _ => true, i => i + _random.Next(-1, 2), i => new InstrumentPrice("VOD.L", i), _ => TimeSpan.FromMilliseconds(100));
        }
    }
}
