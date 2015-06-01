using System.Windows;
using System.Windows.Data;
using MahApps.Metro.Controls;

namespace Shared.Ui.Dialog
{
    public interface IDialogService
    {
        void ShowDialog(FrameworkElement view, object viewModel);
    }
    
    public class DialogService : IDialogService
    {
        public void ShowDialog(FrameworkElement view, object viewModel)
        {
            var window = new MetroWindow
            {
                Content = view,
                DataContext = viewModel,
                SizeToContent = SizeToContent.WidthAndHeight,
                Owner = Application.Current.MainWindow,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            
            window.SetResourceReference(FrameworkElement.StyleProperty, "MetroWindow");
            
            if (viewModel is IDialogAware)
            {
                window.SetBinding(Window.TitleProperty, new Binding("Title") {Source = viewModel});
            }

            window.Show();
        }
    }
}
