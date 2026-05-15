using System.Windows;
using System.Windows.Controls;
using Cashlane.ViewModels;

namespace Cashlane.Views;

public partial class ExpenseModalView : Border
{
    public ExpenseModalView()
    {
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;
    }

    private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (DataContext is ExpenseModalViewModel vm)
        {
            UpdateTabs();
        }
    }

    private void OnBasicTab(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (DataContext is ExpenseModalViewModel vm)
        {
            vm.Tab = "basic";
            UpdateTabs();
        }
    }

    private void OnSettleTab(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (DataContext is ExpenseModalViewModel vm)
        {
            vm.Tab = "settle";
            UpdateTabs();
        }
    }

    private void UpdateTabs()
    {
        if (DataContext is not ExpenseModalViewModel vm) return;

        if (vm.Tab == "basic")
        {
            BasicPanel.Visibility = Visibility.Visible;
            SettlePanel.Visibility = Visibility.Collapsed;
            BasicTabBorder(true);
            SettleTabBorder(false);
        }
        else
        {
            BasicPanel.Visibility = Visibility.Collapsed;
            SettlePanel.Visibility = Visibility.Visible;
            BasicTabBorder(false);
            SettleTabBorder(true);
        }
    }

    private void BasicTabBorder(bool active)
    {
        BasicTab.BorderBrush = active
            ? (System.Windows.Media.Brush)FindResource("AccentBrush")
            : System.Windows.Media.Brushes.Transparent;
    }

    private void SettleTabBorder(bool active)
    {
        SettleTab.BorderBrush = active
            ? (System.Windows.Media.Brush)FindResource("AccentBrush")
            : System.Windows.Media.Brushes.Transparent;
    }

    private void OnSave(object sender, RoutedEventArgs e)
    {
        if (DataContext is ExpenseModalViewModel vm)
        {
            vm.SaveCommand.Execute(null);
        }
    }

    private void OnCancel(object sender, RoutedEventArgs e)
    {
        var mainWindow = Window.GetWindow(this);
        if (mainWindow?.DataContext is MainViewModel mainVm)
        {
            mainVm.CloseModal();
        }
    }
}
