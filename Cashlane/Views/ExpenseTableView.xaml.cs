using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Cashlane.Models;
using Cashlane.ViewModels;

namespace Cashlane.Views;

public partial class ExpenseTableView : UserControl
{
    public ExpenseTableView()
    {
        InitializeComponent();
    }

    private void OnRowDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is ExpenseTableViewModel vm && vm.SelectedExpense != null)
        {
            // Find the MainViewModel through the visual tree
            var mainWindow = Window.GetWindow(this);
            if (mainWindow?.DataContext is MainViewModel mainVm)
            {
                mainVm.OpenEditModal(vm.SelectedExpense);
            }
        }
    }
}
