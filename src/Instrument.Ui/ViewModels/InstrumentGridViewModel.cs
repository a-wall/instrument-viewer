using System.ComponentModel;
using Shared;

namespace Instrument.Ui.ViewModels
{
    public class InstrumentGridViewModel
    {
        private ObservableKeyedCollection<string, InstrumentPriceViewModel> _instrumentPrices;
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableKeyedCollection<string, InstrumentPriceViewModel> InstrumentPrices
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
