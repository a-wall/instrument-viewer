namespace Instrument.Ui.ViewModels
{
    public static class InstrumentPriceViewModelExtensions
    {
        public static void Update(this InstrumentPriceViewModel viewModel, InstrumentPrice price)
        {
            viewModel.Price.Update(price.Price);
        }

        public static InstrumentPriceViewModel ToViewModel(this InstrumentPrice price)
        {
            return new InstrumentPriceViewModel
            {
                Instrument = price.Instrument,
                Price = new PriceViewModel
                {
                    Value = price.Price
                }
            };
        }
    }
}
