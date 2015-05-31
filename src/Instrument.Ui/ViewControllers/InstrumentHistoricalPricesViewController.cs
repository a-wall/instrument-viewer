using System;
using System.Collections.ObjectModel;
using Instrument.Ui.ViewModels;
using Microsoft.Practices.Prism;

namespace Instrument.Ui.ViewControllers
{
    public class InstrumentHistoricalPricesViewController : IDisposable
    {
        private readonly InstrumentHistoricalPricesViewModel _viewModel;

        public InstrumentHistoricalPricesViewController()
        {
            _viewModel = new InstrumentHistoricalPricesViewModel();
        }

        public void Initialize(string instrument)
        {
            _viewModel.Title = string.Format("{0} - Historical Prices", instrument);

            _viewModel.Prices = new ObservableCollection<decimal>(new decimal[] {100, 101, 102, 105, 110, 115, 116, 117, 119, 120});
        }

        public InstrumentHistoricalPricesViewModel ViewModel
        {
            get { return _viewModel; }
        }

        public void Dispose()
        {
            
        }
    }
}
