using System;
using System.Reactive.Disposables;

namespace Shared
{
    public static class DisposableExtensions
    {
        public static void AddToDisposable(this IDisposable disposable, CompositeDisposable compositeDisposable)
        {
            compositeDisposable.Add(disposable);
        }
    }
}
