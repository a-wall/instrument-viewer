using System;
using System.Reactive.Linq;

namespace Instrument.Services
{
    public sealed class InstrumentPriceService : IInstrumentPriceService
    {
        readonly Random _random = new Random();
        private IObservable<InstrumentPrice> _prices;

        public InstrumentPriceService()
        {
            _prices = Observable.Generate(100, _ => true, i => i + _random.Next(-1, 2), i => new InstrumentPrice("VOD.L", i), _ => TimeSpan.FromMilliseconds(100))
                             .Merge(Observable.Generate(70, _ => true, i => i + _random.Next(-1, 2), i => new InstrumentPrice("BT.L", i), _ => TimeSpan.FromMilliseconds(100)))
                             .Publish()
                             .RefCount();
        }

        public IObservable<InstrumentPrice> ObserveInstrumentPrices()
        {
            return _prices;
        }
    }
}
