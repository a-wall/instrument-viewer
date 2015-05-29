using System.ComponentModel;
using Shared;

namespace Instrument.Ui.ViewModels
{
    public class InstrumentPriceViewModel : INotifyPropertyChanged
    {
        private string _instrument;
        private PriceViewModel _price;
        public event PropertyChangedEventHandler PropertyChanged;

        public InstrumentPriceViewModel()
        {
            _price = new PriceViewModel();
        }

        public string Instrument
        {
            get { return _instrument; }
            set
            {
                if (_instrument != value)
                {
                    _instrument = value;
                    PropertyChanged.Raise(this, () => Instrument);
                }
            }
        }

        public PriceViewModel Price
        {
            get { return _price; }
            set
            {
                if (_price != value)
                {
                    _price = value;
                    PropertyChanged.Raise(this, () => Price);
                }
            }
        }
    }
}
