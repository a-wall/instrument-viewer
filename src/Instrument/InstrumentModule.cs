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
    public class InstrumentModule : IModule
    {
        private readonly IUnityContainer _container;

        public InstrumentModule(IUnityContainer container)
        {
            _container = container;
        }

        public void Initialize()
        {
            _container.RegisterType<IInstrumentPriceService, InstrumentPriceService>(new ContainerControlledLifetimeManager());
        }
    }
}
