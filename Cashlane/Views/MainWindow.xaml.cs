using System.Windows;
using System.Windows.Input;
using Cashlane.ViewModels;

namespace Cashlane.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void OnOverlayClick(object sender, MouseButtonEventArgs e)
    {
        // Only close if clicking the overlay background, not the modal itself
        if (e.OriginalSource == sender && DataContext is MainViewModel vm)
        {
            vm.CloseModal();
        }
    }
}
