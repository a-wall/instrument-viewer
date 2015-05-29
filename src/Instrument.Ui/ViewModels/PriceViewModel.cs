using System.ComponentModel;
using Shared;

namespace Instrument.Ui.ViewModels
{
    public class PriceViewModel : INotifyPropertyChanged
    {
        private PriceChangeDirection _change;
        private decimal _value;
        public event PropertyChangedEventHandler PropertyChanged;

        public decimal Value
        {
            get { return _value; }
            set
            {
                if (_value != value)
                {
                    _value = value;
                    PropertyChanged.Raise(this, () => Value);
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
                    PropertyChanged.Raise(this, () => Change);
                }
            }
        }
    }
}
