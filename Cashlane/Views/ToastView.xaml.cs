using System.Windows;
using System.Windows.Controls;
using Cashlane.ViewModels;

namespace Cashlane.Views;

public partial class ToastView : Border
{
    public ToastView()
    {
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;
    }

    private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (e.OldValue is ToastViewModel oldVm)
            oldVm.PropertyChanged -= OnVmPropertyChanged;
        if (e.NewValue is ToastViewModel newVm)
            newVm.PropertyChanged += OnVmPropertyChanged;
    }

    private void OnVmPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ToastViewModel.IsVisible) && DataContext is ToastViewModel vm)
        {
            Visibility = vm.IsVisible ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
