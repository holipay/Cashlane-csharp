using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Cashlane.Models;
using Cashlane.ViewModels;
using Microsoft.Win32;

namespace Cashlane.Views;

public partial class ExpenseTableView : UserControl
{
    public ExpenseTableView()
    {
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;
    }

    private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (DataContext is ExpenseTableViewModel vm)
        {
            vm.ConfirmDelete = (title, message) =>
            {
                var result = MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Warning);
                return result == MessageBoxResult.Yes;
            };

            vm.ShowOpenFileDialog = () =>
            {
                var dlg = new OpenFileDialog
                {
                    Title = "选择要导入的 Excel 文件",
                    Filter = "Excel 文件|*.xlsx;*.xls",
                    FilterIndex = 1
                };
                return dlg.ShowDialog() == true ? dlg.FileName : null;
            };

            vm.ShowSaveFileDialog = () =>
            {
                var dlg = new SaveFileDialog
                {
                    Title = "导出费用记录",
                    Filter = "Excel 文件|*.xlsx",
                    FileName = $"费用记录_{DateTime.Now:yyyyMMdd}",
                    DefaultExt = ".xlsx"
                };
                return dlg.ShowDialog() == true ? dlg.FileName : null;
            };
        }
    }

    private void OnRowDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is ExpenseTableViewModel vm && vm.SelectedExpense != null)
        {
            var mainWindow = Window.GetWindow(this);
            if (mainWindow?.DataContext is MainViewModel mainVm)
            {
                mainVm.OpenEditModal(vm.SelectedExpense);
            }
        }
    }
}
