using System.ComponentModel;

namespace Instrument.Ui.ViewModels
{
    public class InstrumentPriceViewModel : INotifyPropertyChanged
    {
        private string _instrument;
        private PriceViewModel _price;
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

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
                    OnPropertyChanged("Instrument");
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
                    OnPropertyChanged("Price");
                }
            }
        }
    }
}
