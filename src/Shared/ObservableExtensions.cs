using Shared.Subjects;
using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Shared
{
    public static class ObservableExtensions
    {
        /// <summary>
        /// Applies a conflation algorithm to an observable stream. 
        /// Anytime the stream OnNext twice below minimumUpdatePeriod, the second update gets delayed to respect the minimumUpdatePeriod
        /// If more than 2 update happen, only the last update is pushed
        /// Updates are pushed and rescheduled using the provided scheduler
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">stream</param>
        /// <param name="minimumUpdatePeriod">minimum delay between 2 updates</param>
        /// <param name="scheduler">to be used to publish updates and schedule delayed updates</param>
        /// <returns></returns>
        public static IObservable<T> Conflate<T>(this IObservable<T> source, TimeSpan minimumUpdatePeriod, IScheduler scheduler)
        {
            return Observable.Create<T>(observer =>
            {
                // indicate when the last update was published
                var lastUpdateTime = DateTimeOffset.MinValue;
                // indicate if an update is currently scheduled
                var updateScheduled = new MultipleAssignmentDisposable();
                // indicate if completion has been requested (we can't complete immediatly if an update is in flight)
                var completionRequested = false;
                var gate = new object();

                var subscription = source
                        .ObserveOn(scheduler)
                        .Subscribe(
                            x =>
                            {
                                var currentUpdateTime = scheduler.Now;

                                bool scheduleRequired;
                                lock (gate)
                                {
                                    scheduleRequired = currentUpdateTime - lastUpdateTime < minimumUpdatePeriod;
                                    if (scheduleRequired && updateScheduled.Disposable != null)
                                    {
                                        updateScheduled.Disposable.Dispose();
                                        updateScheduled.Disposable = null;
                                    }
                                }

                                if (scheduleRequired)
                                {
                                    updateScheduled.Disposable = scheduler.Schedule(lastUpdateTime + minimumUpdatePeriod, () =>
                                    {
                                        observer.OnNext(x);

                                        lock (gate)
                                        {
                                            lastUpdateTime = scheduler.Now;
                                            updateScheduled.Disposable = null;
                                            if (completionRequested)
                                            {
                                                observer.OnCompleted();
                                            }
                                        }
                                    });
                                }
                                else
                                {
                                    observer.OnNext(x);
                                    lock (gate)
                                    {
                                        lastUpdateTime = scheduler.Now;
                                    }
                                }
                            },
                            observer.OnError,
                            () =>
                            {
                                // if we have scheduled an update we need to complete once the update has been published
                                if (updateScheduled.Disposable != null)
                                {
                                    lock (gate)
                                    {
                                        completionRequested = true;
                                    }
                                }
                                else
                                {
                                    observer.OnCompleted();
                                }
                            });

                return subscription;
            });
        }

        public static IConnectableObservable<T> Replay<T>(this IObservable<T> source, T seed, Func<T, T, T> accumulator)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            return source.Multicast(new AccumulatingReplaySubject<T>(seed, accumulator));
        }

        public static IObservable<T> RepeatAfter<T>(this IObservable<T> source, TimeSpan dueTime, IScheduler scheduler = null)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            return RepeatInfinite(source, dueTime, scheduler).Concat();
        }

        public static IObservable<T> RepeatAfter<T>(this IObservable<T> source, TimeSpan dueTime, int repeatCount, IScheduler scheduler = null)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            return RepeatFinite(source, _ => dueTime, repeatCount, scheduler).Concat();
        }

        public static IObservable<T> RetryAfter<T>(this IObservable<T> source, TimeSpan dueTime, IScheduler scheduler = null)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            return RepeatInfinite(source, dueTime, scheduler).Catch();
        }

        public static IObservable<T> RetryAfter<T>(this IObservable<T> source, TimeSpan dueTime, int retryCount, IScheduler scheduler = null)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            return RepeatFinite(source, _ => dueTime, retryCount, scheduler).Catch();
        }

        public static IObservable<T> RetryWithBackOff<T>(this IObservable<T> source, TimeSpan dueTime, int retryCount, IScheduler scheduler = null)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            return RepeatFinite(source, attempt => TimeSpan.FromTicks(dueTime.Ticks * attempt), retryCount, scheduler).Catch();
        }

        private static IEnumerable<IObservable<T>> RepeatInfinite<T>(this IObservable<T> source, TimeSpan dueTime, IScheduler scheduler)
        {
            yield return source;

            while (true)
                yield return source.DelaySubscription(dueTime, scheduler ?? DefaultScheduler.Instance);
        }

        private static IEnumerable<IObservable<T>> RepeatFinite<T>(this IObservable<T> source, Func<int, TimeSpan> dueTime, int retryCount, IScheduler scheduler)
        {
            yield return source;

            for (int i = 1; i <= retryCount; i++)
            {
                yield return source.DelaySubscription(dueTime(i), scheduler ?? DefaultScheduler.Instance);
            }
        }
    }
}
