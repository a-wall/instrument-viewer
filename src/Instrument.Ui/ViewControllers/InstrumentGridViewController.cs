using System;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Instrument.Services;
using Instrument.Ui.ViewModels;
using Shared;

namespace Instrument.Ui.ViewControllers
{
    public sealed class InstrumentGridViewController : IDisposable
    {
        private readonly IInstrumentPriceService _instrumentPriceService;
        private InstrumentGridViewModel _viewModel;
        private readonly CompositeDisposable _disposable;
        private readonly TimeSpan _conflateTimeSpan;
        private readonly IScheduler _uiScheduler;
        private readonly IScheduler _backgroundScheduler;

        public InstrumentGridViewController(IInstrumentPriceService instrumentPriceService, TimeSpan conflateTimeSpan, IScheduler uiScheduler, IScheduler backgroundScheduler)
        {
            _instrumentPriceService = instrumentPriceService;
            _conflateTimeSpan = conflateTimeSpan;
            _uiScheduler = uiScheduler;
            _backgroundScheduler = backgroundScheduler;
            _disposable = new CompositeDisposable();
        }

        public InstrumentGridViewModel ViewModel
        {
            get { return _viewModel; }
        }

        public void Initialize()
        {
            _viewModel = new InstrumentGridViewModel
            {
                InstrumentPrices = new ObservableKeyedCollection<string, InstrumentPriceViewModel>(ip => ip.Instrument)
            };

            // Here we update current and average prices separately. 
            // We could combine both streams but it will make the code less readable

            _instrumentPriceService.ObserveInstrumentPrices()
                                   .GroupBy(p => p.Instrument)
                                   .Select(gp => gp.Conflate(_conflateTimeSpan, _backgroundScheduler)) // we conflate every instrument separately
                                   .Merge()
                                   .SubscribeOn(_backgroundScheduler)
                                   .ObserveOn(_uiScheduler)
                                   .Subscribe(p =>
                                   {
                                       if (_viewModel.InstrumentPrices.Contains(p.Instrument))
                                       {
                                           _viewModel.InstrumentPrices[p.Instrument].UpdateCurrentPrice(p);
                                       }
                                       else
                                       {
                                           _viewModel.InstrumentPrices.Add(p.ToViewModel());
                                       }
                                   })
                                   .AddToDisposable(_disposable);

            _instrumentPriceService.ObserveInstrumentPrices()
                                   .GroupBy(p => p.Instrument)
                                   .Select(gp => gp.Scan(Enumerable.Empty<decimal>(), (acum, price) => acum.StartWith(price.Price).Take(5))
                                                   .Select(p => new InstrumentPrice(gp.Key, p.Average(x => x)))
                                                   .Conflate(_conflateTimeSpan, _backgroundScheduler))
                                   .Merge()
                                   .SubscribeOn(_backgroundScheduler)
                                   .ObserveOn(_uiScheduler)
                                   .Subscribe(p =>
                                   {
                                       if (_viewModel.InstrumentPrices.Contains(p.Instrument))
                                       {
                                           _viewModel.InstrumentPrices[p.Instrument].UpdateAveragePrice(p);
                                       }
                                   })
                                   .AddToDisposable(_disposable);
        }

        public void Dispose()
        {
            _disposable.Dispose();
        }
    }
}
