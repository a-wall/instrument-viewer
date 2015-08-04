using System.Collections.Generic;
using System.Linq;

namespace Instrument
{
    public static class InstrumentPriceExtensions
    {
        public static string ToSummary(this IEnumerable<InstrumentPrice> prices)
        {
            return string.Join(", ", prices.Select(p => p.ToString()));
        }
    }
}
