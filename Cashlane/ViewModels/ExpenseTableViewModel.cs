using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Cashlane.Models;
using Cashlane.Services;

namespace Cashlane.ViewModels;

public class ExpenseTableViewModel : ViewModelBase
{
    private readonly DatabaseService _db;
    private readonly MainViewModel _mainVm;
    private Expense? _selectedExpense;
    private int _totalCount;
    private double _totalAmount;

    public ExpenseTableViewModel(DatabaseService db, MainViewModel mainVm)
    {
        _db = db;
        _mainVm = mainVm;
    }

    public ObservableCollection<Expense> Expenses { get; } = new();

    public Expense? SelectedExpense
    {
        get => _selectedExpense;
        set => SetProperty(ref _selectedExpense, value);
    }

    public int TotalCount
    {
        get => _totalCount;
        set => SetProperty(ref _totalCount, value);
    }

    public double TotalAmount
    {
        get => _totalAmount;
        set => SetProperty(ref _totalAmount, value);
    }

    public ICommand AddCommand => new RelayCommand(() => _mainVm.OpenAddModal());

    public ICommand EditCommand => new RelayCommand(() =>
    {
        if (SelectedExpense != null)
            _mainVm.OpenEditModal(SelectedExpense);
    });

    public ICommand DeleteCommand => new RelayCommand(() =>
    {
        if (SelectedExpense == null) return;
        var result = MessageBox.Show(
            "确定要删除这条费用记录吗？此操作不可恢复。",
            "确认删除",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning);
        if (result == MessageBoxResult.Yes)
        {
            _db.DeleteExpense(SelectedExpense.Id);
            _mainVm.LoadData();
            _mainVm.ToastVm.Show("🗑️ 记录已删除");
        }
    });

    public ICommand ImportCommand => new RelayCommand(() =>
    {
        _mainVm.ToastVm.Show("📥 导入功能开发中...");
    });

    public ICommand ExportCommand => new RelayCommand(() =>
    {
        _mainVm.ToastVm.Show("📤 导出功能开发中...");
    });

    public void LoadExpenses(List<Expense> expenses)
    {
        Expenses.Clear();
        foreach (var e in expenses)
            Expenses.Add(e);
        TotalCount = expenses.Count;
        TotalAmount = expenses.Sum(e => e.ReimburseAmount + e.Prepaid);
    }
}
