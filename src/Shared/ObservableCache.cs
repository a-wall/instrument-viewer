using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;

namespace Shared
{
    public class ObservableCache<TKey, TSource> : IDisposable
    {
        private readonly IConnectableObservable<TSource> _source;
        private readonly Func<TSource, TKey> _keySelector;
        private readonly IScheduler _scheduler;
        private int _historySize;
        private readonly Dictionary<TKey, ISubject<TSource>> _items;
        private readonly Subject<TKey> _keys = new Subject<TKey>();
        private readonly CompositeDisposable _disposable;
        private bool _completed;
        private Exception _error;

        public ObservableCache(IObservable<TSource> source, Func<TSource, TKey> keySelector, IScheduler scheduler, int historySize = 1)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (keySelector == null)
                throw new ArgumentNullException("keySelector");

            _source = source.Publish();
            _keySelector = keySelector;
            _scheduler = scheduler;
            _historySize = historySize;
            _items = new Dictionary<TKey, ISubject<TSource>>();
            _disposable = new CompositeDisposable();
        }

        public void Initialize()
        {
            _source.ObserveOn(_scheduler)
                   .Subscribe(x => Publish(_keySelector(x), x),
                              ex => 
                              {
                                  _error = ex;
                                  ErrorSubscribers();
                              },
                              () =>
                              {
                                  _completed = true;
                                  CompleteSubscribers();
                              })
                   .AddToDisposable(_disposable);

            _source.Connect().AddToDisposable(_disposable);
        }

        private IObservable<TSource> GetObservable(TKey key)
        {
            if (_disposable.IsDisposed)
            {
                throw new ObjectDisposedException("ObservableCache");
            }
            
            ISubject<TSource> value;
            if (_items.TryGetValue(key, out value))
            {
                return value;
            }

            if (_error != null)
            {
                return Observable.Throw<TSource>(_error);
            }
            if (_completed)
            {
                return Observable.Empty<TSource>();
            }

            _items[key] = new ReplaySubject<TSource>(_historySize);
            _keys.OnNext(key);
            return _items[key];
        }

        private void Publish(TKey key, TSource value)
        {
            if (_error != null || _completed || _disposable.IsDisposed)
            {
                return;
            }

            ISubject<TSource> subject;
            if (_items.TryGetValue(key, out subject))
            {
                subject.OnNext(value);
            }
            else
            {
                _items[key] = new ReplaySubject<TSource>(_historySize);
                _items[key].OnNext(value);
                _keys.OnNext(key);
            }
        }

        public IObservable<TSource> Observe(TKey key)
        {
            return Observable.Create<TSource>(observer => GetObservable(key).Subscribe(observer))
                             .SubscribeOn(_scheduler);
        }

        public IObservable<TSource> ObserveAll()
        {
            return ObserveKeys().Select(Observe).Merge();
        }

        public IObservable<TKey> ObserveKeys()
        {
            return Observable.Create<TKey>(observer => _items.Keys
                                                             .ToObservable(ImmediateScheduler.Instance)
                                                             .Merge(_keys)
                                                             .Subscribe(observer))
                             .SubscribeOn(_scheduler);
        }

        public void Dispose()
        {
            _disposable.Dispose();
            _scheduler.Schedule(CompleteSubscribers);
        }

        private void CompleteSubscribers()
        {
            _items.Values.ToList().ForEach(x => x.OnCompleted());
            _keys.OnCompleted();
        }

        private void ErrorSubscribers()
        {
            _items.Values.ToList().ForEach(x => x.OnError(_error));
            _keys.OnError(_error);
        }
    }
}
