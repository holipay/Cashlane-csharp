using System.Windows;
using System.Windows.Controls;
using Cashlane.ViewModels;

namespace Cashlane.Views;

public partial class ContactsView : UserControl
{
    public ContactsView()
    {
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;
    }

    private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (DataContext is ContactsViewModel vm)
        {
            vm.ConfirmDelete = (title, message) =>
            {
                var result = MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Warning);
                return result == MessageBoxResult.Yes;
            };
        }
    }
}
