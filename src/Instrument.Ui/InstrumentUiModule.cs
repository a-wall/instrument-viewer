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
        private readonly CompositeDisposable _disposable;

        public InstrumentUiModule(IRegionManager regionManager, IInstrumentPriceService instrumentPriceService, IDialogService dialogService)
        {
            _regionManager = regionManager;
            _instrumentPriceService = instrumentPriceService;
            _dialogService = dialogService;
            _disposable = new CompositeDisposable();
        }

        public void Initialize()
        {
            // We throttle the updates (300ms) to avoid extra load on the dispatcher
            // There's no need to show every single price change on the client
            var viewController = new InstrumentGridViewController(_instrumentPriceService, TimeSpan.FromMilliseconds(300), ShowHistoricalPrices, DispatcherScheduler.Current, new EventLoopScheduler());
            viewController.Initialize();
            viewController.AddToDisposable(_disposable);
            _regionManager.RegisterViewWithRegion("MainRegion", () => new InstrumentGridView {DataContext = viewController.ViewModel});

            Observable.FromEventPattern<ExitEventHandler, ExitEventArgs>(h => Application.Current.Exit += h, h => Application.Current.Exit -= h)
                      .Take(1)                      
                      .Subscribe(_ => _disposable.Dispose());
        }

        private void ShowHistoricalPrices(string instrument)
        {
            // We throttle the updates (300ms) to avoid extra load on the dispatcher
            var historicalPricesViewController = new InstrumentHistoricalPricesViewController(_instrumentPriceService, TimeSpan.FromMilliseconds(300), DispatcherScheduler.Current, TaskPoolScheduler.Default);
            historicalPricesViewController.Initialize(instrument);
            historicalPricesViewController.AddToDisposable(_disposable);
            var historicalPricesView = new InstrumentHistoricalPricesView();
            _dialogService.ShowDialog(historicalPricesView, historicalPricesViewController.ViewModel); // there should be some callback notifying the dialog is closed to dispose historicalPricesViewController
        }
    }
}
