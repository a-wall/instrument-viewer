using System;
using System.ComponentModel;
using System.Reactive;
using System.Reactive.Linq;

namespace Shared
{
    public static class PropertyChangedExtensions
    {
        public static IObservable<EventPattern<PropertyChangedEventArgs>> ObservePropertyChanged(this INotifyPropertyChanged source)
        {
            return Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(h => source.PropertyChanged += h, h => source.PropertyChanged -= h);
        }
    }
}
