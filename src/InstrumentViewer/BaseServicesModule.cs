using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Unity;
using Shared.Ui.Dialog;

namespace InstrumentViewer
{
    public class BaseServicesModule : IModule
    {
        private readonly IUnityContainer _container;

        public BaseServicesModule(IUnityContainer container)
        {
            _container = container;
        }

        public void Initialize()
        {
            var dialogService = new DialogService();
            _container.RegisterInstance<IDialogService>(dialogService);
        }
    }
}
