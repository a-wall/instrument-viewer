using System.ComponentModel;

namespace Instrument.Ui.ViewModels
{
    public class PriceViewModel : INotifyPropertyChanged
    {
        private PriceChangeDirection _change;
        private decimal _value;
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public decimal Value
        {
            get { return _value; }
            set
            {
                if (_value != value)
                {
                    _value = value;
                    OnPropertyChanged("Value");
                }
            }
        }

        public PriceChangeDirection Change
        {
            get { return _change; }
            set
            {
                if (_change != value)
                {
                    _change = value;
                    OnPropertyChanged("Change");
                }
            }
        }
    }
}
