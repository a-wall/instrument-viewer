namespace Instrument.Ui
{
    public class AggregatedInstrumentPrice
    {
        public AggregatedInstrumentPrice(string instrument, decimal currentPrice, decimal averagePrice)
        {
            Instrument = instrument;
            CurrentPrice = currentPrice;
            AveragePrice = averagePrice;
        }

        public string Instrument { get; private set; }
        public decimal CurrentPrice { get; private set; }
        public decimal AveragePrice { get; private set; }
    }
}
