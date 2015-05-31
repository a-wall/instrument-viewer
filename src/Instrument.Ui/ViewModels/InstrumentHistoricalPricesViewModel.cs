using System.Collections.ObjectModel;
using System.ComponentModel;
using Shared;
using Shared.Ui.Dialog;

namespace Instrument.Ui.ViewModels
{
    public class InstrumentHistoricalPricesViewModel : INotifyPropertyChanged, IDialogAware
    {
        private string _title;
        private ObservableCollection<decimal> _prices;
        public event PropertyChangedEventHandler PropertyChanged;

        public string Title
        {
            get { return _title; }
            set
            {
                if (_title != value)
                {
                    _title = value;
                    PropertyChanged.Raise(this, () => Title);
                }
            }
        }

        public ObservableCollection<decimal> Prices
        {
            get { return _prices; }
            set
            {
                if (_prices != value)
                {
                    _prices = value;
                    PropertyChanged.Raise(this, () => Prices);
                }
            }
        }
    }
}
