using Cashlane.Models;
using Cashlane.Services;

namespace Cashlane.ViewModels;

public class StatsViewModel : ViewModelBase
{
    private readonly DatabaseService _db;
    private int _monthCount;
    private double _monthReimburse;
    private int _pendingCount;
    private double _prepaidBalance;

    public StatsViewModel(DatabaseService db)
    {
        _db = db;
    }

    public int MonthCount
    {
        get => _monthCount;
        set => SetProperty(ref _monthCount, value);
    }

    public double MonthReimburse
    {
        get => _monthReimburse;
        set => SetProperty(ref _monthReimburse, value);
    }

    public int PendingCount
    {
        get => _pendingCount;
        set => SetProperty(ref _pendingCount, value);
    }

    public double PrepaidBalance
    {
        get => _prepaidBalance;
        set => SetProperty(ref _prepaidBalance, value);
    }

    public void Refresh()
    {
        var stats = _db.GetStats();
        MonthCount = stats.MonthCount;
        MonthReimburse = stats.MonthReimburse;
        PendingCount = stats.PendingCount;
        PrepaidBalance = stats.PrepaidBalance;
    }
}
