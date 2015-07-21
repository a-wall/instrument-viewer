using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shared;
using System.Reactive.Disposables;
using System.Reactive.Concurrency;

namespace Instrument.Services
{
    public class CachingInstrumentPriceService : IInstrumentPriceService
    {
        private readonly ObservableCache<string, InstrumentPrice> _cache;
        private readonly CompositeDisposable _disposable;

        public CachingInstrumentPriceService(IObservable<InstrumentPrice> prices)
        {
            _disposable = new CompositeDisposable();

            _cache = new ObservableCache<string, InstrumentPrice>(prices, p => p.Instrument, TaskPoolScheduler.Default, 10);
            _cache.Initialize();
            _disposable.Add(_cache);
        }

        public IObservable<InstrumentPrice> ObserveInstrumentPrices()
        {
            return _cache.ObserveAll();
        }

        public IObservable<InstrumentPrice> ObserveHistoricalPrices(string instrumentId)
        {
            return _cache.Observe(instrumentId);
        }

        public void Dispose()
        {
            _disposable.Dispose();
        }
    }
}
