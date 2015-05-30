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
                                           _viewModel.InstrumentPrices[p.Instrument].Update(p);
                                       }
                                       else
                                       {
                                           _viewModel.InstrumentPrices.Add(p.ToViewModel());
                                       }
                                   })
                                   .AddToDisposable(_disposable);

            _instrumentPriceService.ObserveInstrumentPrices()
                                   .GroupBy(p => p.Instrument)
                                   .Select(gp => gp.Buffer(5, 1)
                                                   .Select(p => new InstrumentPrice(gp.Key, p.Average(x => x.Price)))
                                                   .Conflate(_conflateTimeSpan, _backgroundScheduler))
                                   .Merge()
                                   .SubscribeOn(_backgroundScheduler)
                                   .ObserveOn(_uiScheduler)
                                   .Subscribe(p =>
                                   {
                                       if (_viewModel.InstrumentPrices.Contains(p.Instrument))
                                       {
                                           //_viewModel.InstrumentPrices[p.Instrument].Update(p);
                                           _viewModel.InstrumentPrices[p.Instrument].AveragePrice.Update(p.Price);
                                       }
                                       else
                                       {
                                           //_viewModel.InstrumentPrices.Add(p.ToViewModel());
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
