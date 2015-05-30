namespace Instrument.Ui.ViewModels
{
    public static class InstrumentPriceViewModelExtensions
    {
        public static void UpdateCurrentPrice(this InstrumentPriceViewModel viewModel, InstrumentPrice price)
        {
            viewModel.CurrentPrice.Update(price.Price);
        }

        public static void UpdateAveragePrice(this InstrumentPriceViewModel viewModel, InstrumentPrice price)
        {
            viewModel.AveragePrice.Update(price.Price);
        }

        public static InstrumentPriceViewModel ToViewModel(this InstrumentPrice price)
        {
            return new InstrumentPriceViewModel
            {
                Instrument = price.Instrument,
                CurrentPrice = new PriceViewModel
                {
                    Value = price.Price
                },
                AveragePrice = new PriceViewModel()
            };
        }
    }
}
