using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Instrument;
using Instrument.Ui;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.UnityExtensions;
using Microsoft.Practices.Unity;

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
            //base.ConfigureModuleCatalog();

            var moduleCatalog = new ModuleCatalog();
            moduleCatalog.AddModule(typeof(InstrumentModule));
            moduleCatalog.AddModule(typeof(InstrumentUiModule));

            ModuleCatalog = moduleCatalog;
        }
    }
}
