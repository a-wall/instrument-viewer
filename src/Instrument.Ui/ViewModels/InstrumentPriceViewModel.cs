using System.ComponentModel;
using Shared;

namespace Instrument.Ui.ViewModels
{
    public class InstrumentPriceViewModel : INotifyPropertyChanged
    {
        private string _instrument;
        private PriceViewModel _currentPrice;
        private PriceViewModel _averagePrice;
        public event PropertyChangedEventHandler PropertyChanged;

        public InstrumentPriceViewModel()
        {
            _currentPrice = new PriceViewModel();
            _averagePrice = new PriceViewModel();
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

        public PriceViewModel CurrentPrice
        {
            get { return _currentPrice; }
            set
            {
                if (_currentPrice != value)
                {
                    _currentPrice = value;
                    PropertyChanged.Raise(this, () => CurrentPrice);
                }
            }
        }

        public PriceViewModel AveragePrice
        {
            get { return _averagePrice; }
            set
            {
                if (_averagePrice != value)
                {
                    _averagePrice = value;
                    PropertyChanged.Raise(this, () => AveragePrice);
                }
            }
        }
    }
}
