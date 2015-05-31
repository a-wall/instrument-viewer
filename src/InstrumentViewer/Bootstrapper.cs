﻿using System.Windows;
using Instrument;
using Instrument.Ui;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.UnityExtensions;

namespace InstrumentViewer
{
    public class Bootstrapper : UnityBootstrapper 
    {
        protected override DependencyObject CreateShell()
        {
            return new Shell();
        }

        protected override void InitializeShell()
        {
            base.InitializeShell();

            Application.Current.MainWindow = (Window)Shell;
            Application.Current.MainWindow.Show();
        }

        protected override void ConfigureModuleCatalog()
        {
            var moduleCatalog = new ModuleCatalog();
            moduleCatalog.AddModule(typeof (BaseServicesModule));
            moduleCatalog.AddModule(typeof (InstrumentPriceTransportModule));
            moduleCatalog.AddModule(typeof (InstrumentPriceModule));
            moduleCatalog.AddModule(typeof (InstrumentUiModule));

            ModuleCatalog = moduleCatalog;
        }
    }
}
