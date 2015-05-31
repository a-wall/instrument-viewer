using System;
using System.Reactive.Concurrency;
using Instrument.Services;
using Instrument.Ui.ViewControllers;
using Instrument.Ui.Views;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using Shared.Ui.Dialog;

namespace Instrument.Ui
{
    public class InstrumentUiModule : IModule
    {
        private readonly IRegionManager _regionManager;
        private readonly IInstrumentPriceService _instrumentPriceService;
        private readonly IDialogService _dialogService;

        public InstrumentUiModule(IRegionManager regionManager, IInstrumentPriceService instrumentPriceService, IDialogService dialogService)
        {
            _regionManager = regionManager;
            _instrumentPriceService = instrumentPriceService;
            _dialogService = dialogService;
        }

        public void Initialize()
        {
            var viewController = new InstrumentGridViewController(_instrumentPriceService, TimeSpan.FromMilliseconds(300), ShowHistoricalPrices, DispatcherScheduler.Current, new EventLoopScheduler());
            viewController.Initialize();
            _regionManager.RegisterViewWithRegion("MainRegion", () => new InstrumentGridView {DataContext = viewController.ViewModel});
        }

        private void ShowHistoricalPrices(string instrument)
        {
            var historicalPricesViewController = new InstrumentHistoricalPricesViewController();
            historicalPricesViewController.Initialize(instrument);
            var historicalPricesView = new InstrumentHistoricalPricesView();
            _dialogService.ShowDialog(historicalPricesView, historicalPricesViewController.ViewModel);
        }
    }
}
