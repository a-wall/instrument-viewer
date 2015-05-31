using System;
using Shared;

namespace Instrument.FileSystem
{
    public class InstrumentPriceStringAdapter : IAdapter<InstrumentPrice, string>
    {
        public InstrumentPrice Adapt(string value)
        {
            //Dow:12,860
            var pair = value.Split(':');
            return new InstrumentPrice(pair[0], Decimal.Parse(pair[1]));
        }

        public string Adapt(InstrumentPrice value)
        {
            return string.Format("{0}:{1}", value.Instrument, value.Price);
        }
    }
}
