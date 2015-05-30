using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using Instrument;
using Instrument.Services;
using Instrument.Ui.ViewControllers;
using Microsoft.Reactive.Testing;
using NUnit.Framework;
using Rhino.Mocks;

namespace InstrumentViewer.Tests.Instrument.Ui.ViewControllers
{
    [TestFixture]
    public class InstrumentGridViewControllerTests
    {
        readonly TimeSpan _conflateTimeSpan = TimeSpan.FromTicks(10);
        
        [Test]
        public void TwoUpdatesShouldCreateTwoInstrumentsInGridViewModel()
        {
            // Given
            var testScheduler = new TestScheduler();
            var instrumentPriceService = MockRepository.GenerateStrictMock<IInstrumentPriceService>();
            instrumentPriceService.Expect(s => s.ObserveInstrumentPrices())
                                  .Return(new[]
                                  {
                                      new InstrumentPrice("VOD.L", 100),
                                      new InstrumentPrice("BT.L", 50)
                                  }
                                  .ToObservable())
                                  .Repeat.Any();

            // When
            var viewController = new InstrumentGridViewController(instrumentPriceService, _conflateTimeSpan, ImmediateScheduler.Instance, testScheduler);
            viewController.Initialize();
            testScheduler.Start();

            // Then
            Assert.AreEqual(2, viewController.ViewModel.InstrumentPrices.Count);
            Assert.AreEqual(100, viewController.ViewModel.InstrumentPrices["VOD.L"].CurrentPrice.Value);
            Assert.AreEqual(50, viewController.ViewModel.InstrumentPrices["BT.L"].CurrentPrice.Value);
        }

        [Test]
        public void AveragePriceShouldBeCalculatedBasedOnLastFiveUpdates()
        {
            // Given
            var testScheduler = new TestScheduler();
            var instrumentPriceService = MockRepository.GenerateStrictMock<IInstrumentPriceService>();
            instrumentPriceService.Expect(s => s.ObserveInstrumentPrices())
                                  .Return(new[]
                                  {
                                      new InstrumentPrice("VOD.L", 100),
                                      new InstrumentPrice("VOD.L", 101),
                                      new InstrumentPrice("VOD.L", 101),
                                      new InstrumentPrice("VOD.L", 102),
                                      new InstrumentPrice("VOD.L", 103),
                                      new InstrumentPrice("VOD.L", 103),
                                  }
                                  .ToObservable())
                                  .Repeat.Any();

            // When
            var viewController = new InstrumentGridViewController(instrumentPriceService, _conflateTimeSpan, ImmediateScheduler.Instance, testScheduler);
            viewController.Initialize();
            testScheduler.Start();

            // Then
            Assert.AreEqual(1, viewController.ViewModel.InstrumentPrices.Count);
            Assert.AreEqual(102, viewController.ViewModel.InstrumentPrices[0].AveragePrice.Value);
        }
    }
}
