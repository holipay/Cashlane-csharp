using System.Collections.ObjectModel;
using System.Windows.Input;
using Cashlane.Models;
using Cashlane.Services;

namespace Cashlane.ViewModels;

public class ExpenseTableViewModel : ViewModelBase
{
    private readonly DatabaseService _db;
    private readonly ExcelService _excel;
    private readonly MainViewModel _mainVm;
    private Expense? _selectedExpense;
    private int _totalCount;
    private double _totalAmount;

    /// <summary>
    /// Injected from View: (title, message) => userConfirmed
    /// </summary>
    public Func<string, string, bool>? ConfirmDelete { get; set; }

    /// <summary>
    /// Injected from View: returns a file path for opening, or null if cancelled
    /// </summary>
    public Func<string?>? ShowOpenFileDialog { get; set; }

    /// <summary>
    /// Injected from View: returns a file path for saving, or null if cancelled
    /// </summary>
    public Func<string?>? ShowSaveFileDialog { get; set; }

    public ExpenseTableViewModel(DatabaseService db, ExcelService excel, MainViewModel mainVm)
    {
        _db = db;
        _excel = excel;
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
        var confirmed = ConfirmDelete?.Invoke("确认删除", "确定要删除这条费用记录吗？此操作不可恢复。") ?? false;
        if (confirmed)
        {
            _db.DeleteExpense(SelectedExpense.Id);
            _mainVm.LoadData();
            _mainVm.ToastVm.Show("🗑️ 记录已删除");
        }
    });

    public ICommand ExportCommand => new RelayCommand(() =>
    {
        var path = ShowSaveFileDialog?.Invoke();
        if (path == null) return;

        try
        {
            var expenses = Expenses.ToList();
            _excel.ExportExpenses(path, expenses);
            _mainVm.ToastVm.Show($"📤 已导出 {expenses.Count} 条记录");
        }
        catch (Exception ex)
        {
            _mainVm.ToastVm.Show($"❌ 导出失败: {ex.Message}");
        }
    });

    public ICommand ImportCommand => new RelayCommand(() =>
    {
        var path = ShowOpenFileDialog?.Invoke();
        if (path == null) return;

        try
        {
            var imported = _excel.ImportExpenses(path);
            if (imported.Count == 0)
            {
                _mainVm.ToastVm.Show("⚠️ 文件中没有有效数据");
                return;
            }

            int success = 0;
            foreach (var e in imported)
            {
                // Resolve names to IDs
                e.CompanyId = _db.ResolveCompanyId(e.CompanyName);
                e.DeptId = _db.ResolveDeptId(e.DeptName);
                e.Cat1Id = _db.ResolveCategoryId(e.Cat1Name);
                e.Cat2Id = _db.ResolveCategoryId(e.Cat2Name);
                e.ContactId = _db.ResolveContactId(e.ContactName);

                _db.CreateExpense(e);
                success++;
            }

            _mainVm.LoadData();
            _mainVm.ToastVm.Show($"📥 已导入 {success} 条记录");
        }
        catch (Exception ex)
        {
            _mainVm.ToastVm.Show($"❌ 导入失败: {ex.Message}");
        }
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
