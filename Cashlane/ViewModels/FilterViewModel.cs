using System.Collections.ObjectModel;
using Cashlane.Models;
using Cashlane.Services;

namespace Cashlane.ViewModels;

public class FilterViewModel : ViewModelBase
{
    private readonly DatabaseService _db;
    private readonly Action _onFilterChanged;
    private int? _companyId;
    private int? _deptId;
    private int? _cat1Id;
    private int? _cat2Id;
    private string? _status;
    private string? _method;
    private string? _dateFrom;
    private string? _dateTo;
    private string? _search;
    private bool _isOpen = true;

    public FilterViewModel(DatabaseService db, Action onFilterChanged)
    {
        _db = db;
        _onFilterChanged = onFilterChanged;
        LoadLookups();
    }

    public ObservableCollection<LookupItem> Companies { get; } = new();
    public ObservableCollection<LookupItem> Departments { get; } = new();
    public ObservableCollection<LookupItem> Categories { get; } = new();

    public int? CompanyId { get => _companyId; set => SetProperty(ref _companyId, value); }
    public int? DeptId { get => _deptId; set => SetProperty(ref _deptId, value); }
    public int? Cat1Id { get => _cat1Id; set => SetProperty(ref _cat1Id, value); }
    public int? Cat2Id { get => _cat2Id; set => SetProperty(ref _cat2Id, value); }
    public string? Status { get => _status; set => SetProperty(ref _status, value); }
    public string? Method { get => _method; set => SetProperty(ref _method, value); }
    public string? DateFrom { get => _dateFrom; set => SetProperty(ref _dateFrom, value); }
    public string? DateTo { get => _dateTo; set => SetProperty(ref _dateTo, value); }
    public string? Search { get => _search; set => SetProperty(ref _search, value); }
    public bool IsOpen { get => _isOpen; set => SetProperty(ref _isOpen, value); }

    public ICommand ToggleCommand => new RelayCommand(() => IsOpen = !IsOpen);

    public ICommand SearchCommand => new RelayCommand(() => _onFilterChanged());

    public ICommand ResetCommand => new RelayCommand(() =>
    {
        CompanyId = null;
        DeptId = null;
        Cat1Id = null;
        Cat2Id = null;
        Status = null;
        Method = null;
        DateFrom = null;
        DateTo = null;
        Search = null;
        _onFilterChanged();
    });

    public ExpenseFilter ToFilter() => new()
    {
        CompanyId = CompanyId,
        DeptId = DeptId,
        Cat1Id = Cat1Id,
        Cat2Id = Cat2Id,
        Status = Status,
        Method = Method,
        DateFrom = DateFrom,
        DateTo = DateTo,
        Search = Search
    };

    private void LoadLookups()
    {
        foreach (var c in _db.GetCompanies()) Companies.Add(c);
        foreach (var d in _db.GetDepartments()) Departments.Add(d);
        foreach (var c in _db.GetCategories(0)) Categories.Add(c);
    }
}
