namespace Instrument.Ui.ViewModels
{
    public static class InstrumentPriceViewModelExtensions
    {
        public static void Update(this InstrumentPriceViewModel viewModel, InstrumentPrice price)
        {
            viewModel.CurrentPrice.Update(price.Price);
            //viewModel.AveragePrice.Update(price.AveragePrice);
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
                AveragePrice = new PriceViewModel
                {
                    //Value = price.AveragePrice
                }
            };
        }
    }
}
