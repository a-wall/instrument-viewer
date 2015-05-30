namespace Instrument
{
    public class InstrumentPrice
    {
        public InstrumentPrice(string instrument, decimal price)
        {
            Instrument = instrument;
            Price = price;
        }

        public string Instrument { get; private set; }
        public decimal Price { get; private set; }
    }
}
