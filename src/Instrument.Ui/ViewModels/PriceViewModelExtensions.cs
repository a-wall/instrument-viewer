namespace Instrument.Ui.ViewModels
{
    public static class PriceViewModelExtensions
    {
        public static void Update(this PriceViewModel viewModel, decimal price)
        {
            if (price > viewModel.Value)
            {
                viewModel.Change = PriceChangeDirection.Increased;
            }
            else if (price < viewModel.Value)
            {
                viewModel.Change = PriceChangeDirection.Decreased;
            }
            else
            {
                viewModel.Change = PriceChangeDirection.Unchanged;
            }
            viewModel.Value = price;
        }
    }
}
