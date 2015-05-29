using System.Collections.Generic;
using System.Collections.ObjectModel;
using Instrument.Ui.ViewModels;
using Instrument.Ui.Views;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;

namespace Instrument.Ui
{
    public class InstrumentUiModule : IModule
    {
        private readonly IRegionManager _regionManager;

        public InstrumentUiModule(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }

        public void Initialize()
        {
            var viewModel = new InstrumentGridViewModel
            {
                InstrumentPrices = new ObservableCollection<InstrumentPriceViewModel>(new InstrumentPriceViewModel[]
                {
                    new InstrumentPriceViewModel { Instrument = "VOD.L", Price = new PriceViewModel { Value = 100, Change = PriceChangeDirection.Increase}},
                    new InstrumentPriceViewModel { Instrument = "BARC.L", Price = new PriceViewModel { Value = 50, Change = PriceChangeDirection.Decrease}}
                })
            };
            _regionManager.RegisterViewWithRegion("MainRegion", () =>
            {
                return new InstrumentGridView {DataContext = viewModel};
            });
        }
    }
}
