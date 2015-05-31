using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Instrument;
using Instrument.Services;
using NUnit.Framework;

namespace InstrumentViewer.Tests.Instrument.Services
{
    [TestFixture]
    public class InstrumentPriceServiceTests
    {
        [Test]
        public void ObserveInstrumentPricesShouldReturnLastTenCachedPrices()
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
            Assert.AreEqual(10, returnedPrices.Count);
            Assert.AreEqual(11, returnedPrices.First().Price);
            Assert.AreEqual(20, returnedPrices.Last().Price);
        }
    }
}
