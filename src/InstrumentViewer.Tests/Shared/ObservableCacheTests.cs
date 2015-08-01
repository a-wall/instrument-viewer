using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using NSubstitute;
using NUnit.Framework;
using Shared;

namespace InstrumentViewer.Tests.Shared
{
    [TestFixture]
    public class ObservableCacheTests
    {
        [Test]
        public void ObserveShouldReturnCachedValue()
        {
            // Arrange
            var source = new[] 
            { 
                new KeyValuePair<string, int>("A", 0),
                new KeyValuePair<string, int>("B", 0),
                new KeyValuePair<string, int>("C", 0)
            };
            var observerA = Substitute.For<IObserver<KeyValuePair<string, int>>>();
            var observerB = Substitute.For<IObserver<KeyValuePair<string, int>>>();
            var observerC = Substitute.For<IObserver<KeyValuePair<string, int>>>();

            // Act
            var cache = new ObservableCache<string, KeyValuePair<string, int>>(source.ToObservable(), x => x.Key, ImmediateScheduler.Instance);
            cache.Initialize();
            cache.Observe("A").Subscribe(observerA);
            cache.Observe("B").Subscribe(observerB);
            cache.Observe("C").Subscribe(observerC);

            // Assert
            observerA.Received().OnNext(new KeyValuePair<string, int>("A", 0));
            observerB.Received().OnNext(new KeyValuePair<string, int>("B", 0));
            observerC.Received().OnNext(new KeyValuePair<string, int>("C", 0));
        }
    }
}
