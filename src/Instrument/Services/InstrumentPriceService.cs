using System;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Shared;

namespace Instrument.Services
{
    public sealed class InstrumentPriceService : IInstrumentPriceService, IDisposable
    {
        private readonly IObservable<InstrumentPrice> _prices;
        private readonly CompositeDisposable _disposable;

        public InstrumentPriceService(IObservable<InstrumentPrice> prices)
        {
            _disposable = new CompositeDisposable();

            // Here we cache up to 10 last updates for every instruments.
            // This is to satisfy the requirement that every late subscriber should be able to get
            // last 5 prices to calculate the average price and last 10 prices to show the historical prices.
            // In a real app we usualy only cache the latest price. The historical prices are provided by a separate service like KDB.
            var pricesCache = prices.GroupBy(p => p.Instrument)
                                    .Select(gp =>
                                    {
                                        var sub = gp.Replay(10, ImmediateScheduler.Instance);
                                        sub.Connect().AddToDisposable(_disposable);
                                        return sub.AsObservable();
                                    })
                                    .Replay();

            pricesCache.Connect().AddToDisposable(_disposable);

            _prices = pricesCache.Merge();
        }

        public IObservable<InstrumentPrice> ObserveInstrumentPrices()
        {
            return _prices;
        }

        public IObservable<InstrumentPrice> ObserveHistoricalPrices(string instrumentId)
        {
            // Here we know that the number of instruments will be small so .Where(...) is ok to use.
            // If the number of instruments and the number of cached elements is large,
            // the more sofisticated cache based on Dictionary should be used.
            return _prices.Where(p => instrumentId.Equals(p.Instrument, StringComparison.InvariantCultureIgnoreCase));
        }

        public void Dispose()
        {
            _disposable.Dispose();
        }
    }
}
