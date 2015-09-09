using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Subjects;

namespace Shared.Subjects
{
    public sealed class AccumulatingReplaySubject<T> : ISubject<T>, IDisposable
    {
        private bool _hasValue;
        private T _value;
        private Func<T, T, T> _accumulator;
        private readonly object _gate = new object();
        private List<IObserver<T>> _observers;
        private bool _isStopped;
        private Exception _error;
        private bool _isDisposed;

        public AccumulatingReplaySubject(T seed, Func<T, T, T> accumulator)
        {
            _value = seed;
            _accumulator = accumulator;
            _observers = new List<IObserver<T>>(1);
        }

        public bool IsDisposed
        {
            get
            {
                lock (_gate)
                {
                    return _isDisposed;
                }
            }
        }

        public void OnNext(T value)
        {
            lock (_gate)
            {
                CheckDisposed();

                if (!_isStopped)
                {
                    Next(value);

                    var observers = _observers.ToArray();
                    foreach (var observer in observers)
                        observer.OnNext(value);
                }
            }
        }

        public void OnError(Exception error)
        {
            lock (_gate)
            {
                CheckDisposed();

                if (!_isStopped)
                {
                    _isStopped = true;
                    _error = error;

                    var observers = _observers.ToArray();
                    foreach (var observer in observers)
                        observer.OnError(error);

                    _observers = new List<IObserver<T>>();
                }
            }
        }

        public void OnCompleted()
        {
            lock (_gate)
            {
                CheckDisposed();

                if (!_isStopped)
                {
                    _isStopped = true;

                    var observers = _observers.ToArray();
                    foreach (var observer in observers)
                        observer.OnCompleted();

                    _observers = new List<IObserver<T>>();
                }
            }
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            var subscription = Disposable.Empty;

            lock (_gate)
            {
                CheckDisposed();

                Replay(observer);

                if (_error != null)
                {
                    observer.OnError(_error);
                }
                else if (_isStopped)
                {
                    observer.OnCompleted();
                }

                if (!_isStopped)
                {
                    subscription = new Subscription<T>(this, observer);
                    _observers.Add(observer);
                }
            }

            return subscription;
        }

        public void Dispose()
        {
            lock (_gate)
            {
                _isDisposed = true;
                _observers = null;
                _value = default(T);
            }
        }

        private void CheckDisposed()
        {
            if (_isDisposed)
                throw new ObjectDisposedException(string.Empty);
        }

        private void Unsubscribe(IObserver<T> observer)
        {
            lock (_gate)
            {
                if (!_isDisposed)
                {
                    _observers.Remove(observer);
                }
            }
        }

        private void Next(T value)
        {
            _hasValue = true;
            _value = _accumulator(_value, value);
        }

        private void Replay(IObserver<T> observer)
        {
            if (_hasValue)
            {
                observer.OnNext(_value);
            }
        }
        private sealed class Subscription<TInner> : IDisposable
        {
            private readonly AccumulatingReplaySubject<TInner> _subject;
            private readonly IObserver<TInner> _observer;

            public Subscription(AccumulatingReplaySubject<TInner> subject, IObserver<TInner> observer)
            {
                _subject = subject;
                _observer = observer;
            }

            public void Dispose()
            {
                _subject.Unsubscribe(_observer);
            }
        }
    }
}
