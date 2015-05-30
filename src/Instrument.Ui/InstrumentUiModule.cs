using System;
using System.Reactive.Concurrency;
using Instrument.Services;
using Instrument.Ui.ViewControllers;
using Instrument.Ui.Views;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;

namespace Instrument.Ui
{
    public class InstrumentUiModule : IModule
    {
        private readonly IRegionManager _regionManager;
        private readonly IInstrumentPriceService _instrumentPriceService;

        public InstrumentUiModule(IRegionManager regionManager, IInstrumentPriceService instrumentPriceService)
        {
            _regionManager = regionManager;
            _instrumentPriceService = instrumentPriceService;
        }

        public void Initialize()
        {
            var viewController = new InstrumentGridViewController(_instrumentPriceService, TimeSpan.FromMilliseconds(500), DispatcherScheduler.Current, new EventLoopScheduler());
            viewController.Initialize();
            _regionManager.RegisterViewWithRegion("MainRegion", () => new InstrumentGridView {DataContext = viewController.ViewModel});
        }
    }
}
