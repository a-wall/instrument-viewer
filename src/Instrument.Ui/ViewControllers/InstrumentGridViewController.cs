using System;
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
                                   .Conflate(_conflateTimeSpan, _backgroundScheduler)
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
        }

        public void Dispose()
        {
            _disposable.Dispose();
        }
    }
}
