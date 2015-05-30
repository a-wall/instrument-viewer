using System.Collections.Generic;
using System.Linq;

namespace Instrument.Ui
{
    public static class AggegatedInstrumentPriceExtensions
    {
        public static AggregatedInstrumentPrice ToAggregatedInstrumentPrice(this IEnumerable<InstrumentPrice> prices)
        {
            prices = prices.ToList();
            return new AggregatedInstrumentPrice(prices.First().Instrument, prices.First().Price, prices.Average(p => p.Price));
        }
    }
}
