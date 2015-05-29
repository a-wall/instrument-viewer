using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Instrument.Ui.ViewModels
{
    public class InstrumentGridViewModel
    {
        private ObservableCollection<InstrumentPriceViewModel> _instrumentPrices;
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public ObservableCollection<InstrumentPriceViewModel> InstrumentPrices
        {
            get { return _instrumentPrices; }
            set
            {
                if (_instrumentPrices != value)
                {
                    _instrumentPrices = value;
                    OnPropertyChanged("InstrumentPrices");
                }
            }
        }
    }
}
