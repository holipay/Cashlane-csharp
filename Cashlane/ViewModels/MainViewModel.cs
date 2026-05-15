using System.Collections.ObjectModel;
using Cashlane.Models;
using Cashlane.Services;

namespace Cashlane.ViewModels;

public class MainViewModel : ViewModelBase
{
    private readonly DatabaseService _db;
    private string _activePage = "dashboard";
    private string _pageTitle = "仪表盘";
    private ExpenseModalViewModel? _modalViewModel;
    private bool _isModalOpen;

    public MainViewModel(DatabaseService db)
    {
        _db = db;
        StatsVm = new StatsViewModel(db);
        FilterVm = new FilterViewModel(db, OnFilterChanged);
        ExpenseTableVm = new ExpenseTableViewModel(db, this);
        ContactsVm = new ContactsViewModel(db, this);
        CategoriesVm = new CategoriesViewModel(db, this);
        ToastVm = new ToastViewModel();

        LoadData();
    }

    public StatsViewModel StatsVm { get; }
    public FilterViewModel FilterVm { get; }
    public ExpenseTableViewModel ExpenseTableVm { get; }
    public ContactsViewModel ContactsVm { get; }
    public CategoriesViewModel CategoriesVm { get; }
    public ToastViewModel ToastVm { get; }

    public string ActivePage
    {
        get => _activePage;
        set
        {
            if (SetProperty(ref _activePage, value))
            {
                PageTitle = value switch
                {
                    "dashboard" => "仪表盘",
                    "expenses" => "费用录入",
                    "contacts" => "联系人",
                    "categories" => "科目分类",
                    "reports" => "报表统计",
                    "import" => "导入数据",
                    "export" => "导出数据",
                    "settings" => "系统设置",
                    _ => value
                };
                OnPropertyChanged(nameof(IsDashboardPage));
                OnPropertyChanged(nameof(IsContactsPage));
                OnPropertyChanged(nameof(IsCategoriesPage));
            }
        }
    }

    public bool IsDashboardPage => _activePage is "dashboard" or "expenses";
    public bool IsContactsPage => _activePage == "contacts";
    public bool IsCategoriesPage => _activePage == "categories";

    public string PageTitle
    {
        get => _pageTitle;
        set => SetProperty(ref _pageTitle, value);
    }

    public ExpenseModalViewModel? ModalViewModel
    {
        get => _modalViewModel;
        set => SetProperty(ref _modalViewModel, value);
    }

    public bool IsModalOpen
    {
        get => _isModalOpen;
        set => SetProperty(ref _isModalOpen, value);
    }

    public ICommand NavigateCommand => new RelayCommand<string>(page =>
    {
        if (page != null) ActivePage = page;
    });

    public ICommand RefreshCommand => new RelayCommand(() =>
    {
        LoadData();
        ToastVm.Show("🔄 数据已刷新");
    });

    public void OpenAddModal()
    {
        ModalViewModel = new ExpenseModalViewModel(_db, null, OnModalSave);
        IsModalOpen = true;
    }

    public void OpenEditModal(Expense expense)
    {
        ModalViewModel = new ExpenseModalViewModel(_db, expense, OnModalSave);
        IsModalOpen = true;
    }

    public void CloseModal()
    {
        IsModalOpen = false;
        ModalViewModel = null;
    }

    private void OnModalSave(Expense data)
    {
        if (ModalViewModel?.IsEditing == true && ModalViewModel.EditingId > 0)
        {
            _db.UpdateExpense(ModalViewModel.EditingId, data);
            ToastVm.Show("✅ 费用记录已更新");
        }
        else
        {
            _db.CreateExpense(data);
            ToastVm.Show("✅ 费用记录已保存");
        }
        LoadData();
        CloseModal();
    }

    private void OnFilterChanged()
    {
        LoadExpenses();
    }

    public void LoadData()
    {
        LoadExpenses();
        StatsVm.Refresh();
    }

    private void LoadExpenses()
    {
        var filter = FilterVm.ToFilter();
        var expenses = _db.GetExpenses(filter);
        ExpenseTableVm.LoadExpenses(expenses);
    }
}
