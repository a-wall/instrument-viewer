using System;

namespace Instrument.Services
{
    public interface IInstrumentPriceService
    {
        IObservable<InstrumentPrice> ObserveInstrumentPrices(); 
        
        // another useful method would be to subscribe to prices for a specific instrument
        //IObservable<InstrumentPrice> ObserveInstrumentPrices(string instrument); 
    }
}