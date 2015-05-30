using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using Instrument.Services;
using Instrument.Ui.ViewModels;
using Shared;

namespace Instrument.Ui.ViewControllers
{
    public class InstrumentGridViewController
    {
        private readonly IInstrumentPriceService _instrumentPriceService;
        private InstrumentGridViewModel _viewModel;

        public InstrumentGridViewController(IInstrumentPriceService instrumentPriceService)
        {
            _instrumentPriceService = instrumentPriceService;
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
                                   .SubscribeOn(TaskPoolScheduler.Default)
                                   .ObserveOnDispatcher()
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
                                   });
            //viewModel.InstrumentPrices.AddRange(new[]
            //{
            //    new InstrumentPriceViewModel
            //    {
            //        Instrument = "VOD.L",
            //        Price = new PriceViewModel {Value = 100, Change = PriceChangeDirection.Increased}
            //    },
            //    new InstrumentPriceViewModel
            //    {
            //        Instrument = "BARC.L",
            //        Price = new PriceViewModel {Value = 50, Change = PriceChangeDirection.Decreased}
            //    }
            //});
        }
    }
}
