using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Instrument;
using Instrument.Services;
using NUnit.Framework;

namespace InstrumentViewer.Tests.Instrument.Services
{
    [TestFixture]
    public class InstrumentPriceServiceTests
    {
        [Test]
        public void ObserveInstrumentPricesShouldReturnPublishedPrice()
        {
            // Given
            var prices = new Subject<InstrumentPrice>();

            // When
            var service = new InstrumentPriceService(prices);
            var returnedPrices = new List<InstrumentPrice>();
            service.ObserveInstrumentPrices()
                   .Subscribe(p => returnedPrices.Add(p));
            prices.OnNext(new InstrumentPrice("VOD.L", 100));

            //Then
            Assert.AreEqual(1, returnedPrices.Count);
            Assert.AreEqual(100, returnedPrices[0].Price);
        }
        
        [Test]
        public void ObserveInstrumentPricesShouldReturnLastPrice()
        {
            // Given
            var prices = Enumerable.Range(1, 20)
                                   .Select(p => new InstrumentPrice("VOD.L", p))
                                   .ToObservable();

            // When
            var service = new InstrumentPriceService(prices);
            var returnedPrices = new List<InstrumentPrice>();
            service.ObserveInstrumentPrices()
                   .Subscribe(p => returnedPrices.Add(p));

            //Then
            Assert.IsTrue(returnedPrices.Count > 0);
            Assert.AreEqual(20, returnedPrices.Last().Price);
        }
        
        [Test]
        public void ObserveHistoricalPricesShouldReturnLastTenCachedPrices()
        {
            // Given
            var prices = Enumerable.Range(1, 20)
                                   .Select(p => new InstrumentPrice("VOD.L", p))
                                   .ToObservable();

            // When
            var service = new InstrumentPriceService(prices);
            var returnedPrices = new List<InstrumentPrice>();
            service.ObserveHistoricalPrices("VOD.L")
                   .Subscribe(p => returnedPrices.Add(p));

            //Then
            Assert.AreEqual(10, returnedPrices.Count);
            Assert.AreEqual(11, returnedPrices.First().Price);
            Assert.AreEqual(20, returnedPrices.Last().Price);
        }
    }
}
