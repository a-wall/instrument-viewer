using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using Shared;
using NSubstitute;

namespace InstrumentViewer.Tests.Shared
{
    [TestFixture]
    public class ObservableExtensionsReplayWithAccumulatorTests
    {
        [Test]
        public void LateSubscriberGetsAccumulatedValue()
        {
            // Arrange
            var source = new[] { new[] { "first" }, new[] { "second" }, new[] { "third" } };
            var observerOne = Substitute.For<IObserver<IEnumerable<string>>>();
            var observerTwo = Substitute.For<IObserver<IEnumerable<string>>>();

            // Act
            var observable = source.ToObservable()
                                   .Replay(Enumerable.Empty<string>(), (x, x1) => x.Union(x1))
                                   .RefCount();

            observable.Subscribe(observerOne);
            observable.Subscribe(observerTwo);

            // Assert
            observerOne.Received().OnNext(Arg.Is<IEnumerable<string>>(received => received.SequenceEqual(new[] { "first" })));
            observerOne.Received().OnNext(Arg.Is<IEnumerable<string>>(received => received.SequenceEqual(new[] { "second" })));
            observerOne.Received().OnNext(Arg.Is<IEnumerable<string>>(received => received.SequenceEqual(new[] { "third" })));

            observerTwo.Received().OnNext(Arg.Is<IEnumerable<string>>(received => received.SequenceEqual(new[] { "first", "second", "third" })));
        }
    }
}
