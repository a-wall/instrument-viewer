using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NSubstitute;
using Microsoft.Reactive.Testing;
using System.Reactive.Linq;
using Shared;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Diagnostics;

namespace InstrumentViewer.Tests.Shared
{
    [TestFixture]
    public class ObservableExtensionsRetryTests
    {
        [Test]
        public void RetryAfterShouldRetryAfterError()
        {
            // Arrange
            var scheduler = new TestScheduler();
            var attempts = new IObservable<string>[]
            {
                Observable.Throw<string>(new Exception("first fail")),
                Observable.Throw<string>(new Exception("second fail")),
                Observable.Return("success")
            };
            var observer = Substitute.For<IObserver<string>>();
            var attempt = 0;

            // Act
            Observable.Defer(() => attempts[attempt++])
                      .RetryAfter(TimeSpan.FromSeconds(1), scheduler)
                      .Subscribe(observer);

            scheduler.AdvanceBy(TimeSpan.FromSeconds(2).Ticks);

            // Assert
            observer.Received().OnNext("success");
            Assert.AreEqual(3, attempt);
        }

        [Test]
        public void RetryAfterWithRetryCountShouldOnlyRetrySpecifiedNumberOfTimes()
        {
            // Arrange
            var scheduler = new TestScheduler();
            var attempts = new IObservable<string>[]
            {
                Observable.Throw<string>(new Exception("first fail")),
                Observable.Throw<string>(new Exception("second fail")),
                Observable.Return("success")
            };
            var observer = Substitute.For<IObserver<string>>();
            var attempt = 0;

            // Act
            Observable.Defer(() => attempts[attempt++])
                      .RetryAfter(TimeSpan.FromSeconds(1), 1, scheduler)
                      .Subscribe(observer);

            scheduler.AdvanceBy(TimeSpan.FromSeconds(2).Ticks);

            // Assert
            observer.DidNotReceive().OnNext(Arg.Any<string>());
            Assert.AreEqual(2, attempt);
        }

        [Test]
        public void RetryWithBackoffTest()
        {
            // Arrange
            var scheduler = new TestScheduler();
            var now = scheduler.Now;
            var data = scheduler.CreateHotObservable(
                new Recorded<Notification<string>>(TimeSpan.FromSeconds(1).Ticks, Notification.CreateOnNext("first")),
                new Recorded<Notification<string>>(TimeSpan.FromSeconds(3).Ticks, Notification.CreateOnError<string>(new Exception())),
                new Recorded<Notification<string>>(TimeSpan.FromSeconds(5).Ticks, Notification.CreateOnNext("second")));
            var observer = Substitute.For<IObserver<Timestamped<string>>>();

            // Act
            data.RetryWithBackOff(TimeSpan.FromSeconds(1), 5, scheduler)
                .Timestamp(scheduler)
                .Subscribe(observer);

            scheduler.AdvanceBy(TimeSpan.FromSeconds(5).Ticks);

            // Assert
            observer.Received().OnNext(new Timestamped<string>("first", now.AddSeconds(1)));
            observer.Received().OnNext(new Timestamped<string>("second", now.AddSeconds(5)));
        }
    }
}
