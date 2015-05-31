using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Instrument.Services;
using Instrument.Ui.ViewModels;
using Microsoft.Practices.Prism;
using Shared;

namespace Instrument.Ui.ViewControllers
{
    public class InstrumentHistoricalPricesViewController : IDisposable
    {
        private readonly IInstrumentPriceService _instrumentPriceService;
        private readonly TimeSpan _conflateTimeSpan;
        private readonly IScheduler _uiScheduler;
        private readonly IScheduler _backgroundScheduler;
        private readonly InstrumentHistoricalPricesViewModel _viewModel;
        private readonly CompositeDisposable _disposable;

        public InstrumentHistoricalPricesViewController(IInstrumentPriceService instrumentPriceService,
                                                        TimeSpan conflateTimeSpan,
                                                        IScheduler uiScheduler,
                                                        IScheduler backgroundScheduler)
        {
            _instrumentPriceService = instrumentPriceService;
            _conflateTimeSpan = conflateTimeSpan;
            _uiScheduler = uiScheduler;
            _backgroundScheduler = backgroundScheduler;
            _viewModel = new InstrumentHistoricalPricesViewModel();
            _disposable = new CompositeDisposable();
        }

        public void Initialize(string instrument)
        {
            _viewModel.Title = string.Format("{0} - Historical Prices", instrument);

            _viewModel.Prices = new ObservableCollection<decimal>();

            // Here we show a snapshot of 10 last prices and update the GUI once in one go
            // If needed, the logic can easily be changed to keep updating the historical grid with latest prices
            _instrumentPriceService.ObserveHistoricalPrices(instrument)
                                   .Buffer(_conflateTimeSpan, 10, _backgroundScheduler)
                                   .Where(prices => prices.Count > 0)
                                   .SelectMany(prices => prices)
                                   .Take(10)
                                   .SubscribeOn(_backgroundScheduler)
                                   .ObserveOn(_uiScheduler)
                                   .Subscribe(p => _viewModel.Prices.Add(p.Price))
                                   .AddToDisposable(_disposable);
        }

        public InstrumentHistoricalPricesViewModel ViewModel
        {
            get { return _viewModel; }
        }

        public void Dispose()
        {
            _disposable.Dispose();
        }
    }
}
