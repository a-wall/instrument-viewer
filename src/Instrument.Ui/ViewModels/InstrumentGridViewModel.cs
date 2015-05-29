using System.Collections.ObjectModel;
using System.ComponentModel;
using Shared;

namespace Instrument.Ui.ViewModels
{
    public class InstrumentGridViewModel
    {
        private ObservableCollection<InstrumentPriceViewModel> _instrumentPrices;
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<InstrumentPriceViewModel> InstrumentPrices
        {
            get { return _instrumentPrices; }
            set
            {
                if (_instrumentPrices != value)
                {
                    _instrumentPrices = value;
                    PropertyChanged.Raise(this, () => InstrumentPrices);
                }
            }
        }
    }
}
