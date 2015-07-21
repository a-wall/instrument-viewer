using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Instrument.Services;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Unity;

namespace Instrument
{
    public class InstrumentPriceModule : IModule
    {
        private readonly IUnityContainer _container;

        public InstrumentPriceModule(IUnityContainer container)
        {
            _container = container;
        }

        public void Initialize()
        {
            _container.RegisterType<IInstrumentPriceService, CachingInstrumentPriceService>(new ContainerControlledLifetimeManager());
        }
    }
}
