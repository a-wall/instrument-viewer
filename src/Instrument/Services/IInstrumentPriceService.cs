using System;

namespace Instrument.Services
{
    public interface IInstrumentPriceService
    {
        IObservable<InstrumentPrice> ObserveInstrumentPrices();

        IObservable<InstrumentPrice> ObserveHistoricalPrices(string instrumentId);
    }
}