using System.Collections.ObjectModel;
using Cashlane.Models;
using Cashlane.Services;

namespace Cashlane.ViewModels;

public class ExpenseModalViewModel : ViewModelBase
{
    private readonly DatabaseService _db;
    private readonly Action<Expense> _onSave;
    private string _tab = "basic";
    private bool _isEditing;

    // Basic fields
    private string _entryDate = DateTime.Now.ToString("yyyy-MM-dd");
    private string _occurDate = DateTime.Now.ToString("yyyy-MM-dd");
    private int _companyId;
    private int _deptId;
    private int _cat1Id;
    private int _cat2Id;
    private string _invoiceContent = "";
    private string _detail = "";
    private double _quantity = 1;
    private double _unitPrice;
    private double _exchangeRate = 1;
    private string _docId = "";
    private double _prepaid;
    private double _reimburseAmount;

    // Settle fields
    private string _payMethod = "报销";
    private int _contactId;
    private string _payer = "";
    private string _reimbursee = "";
    private string _transferRecipient = "";
    private string _batchInfo = "";
    private string _reimburseStatus = "填单";
    private string _collectDate = "";
    private string _notes = "";

    public ExpenseModalViewModel(DatabaseService db, Expense? expense, Action<Expense> onSave)
    {
        _db = db;
        _onSave = onSave;

        Companies = new ObservableCollection<LookupItem>(db.GetCompanies());
        Departments = new ObservableCollection<LookupItem>(db.GetDepartments());
        Categories = new ObservableCollection<LookupItem>(db.GetCategories(0));
        Contacts = new ObservableCollection<LookupItem>(db.GetContacts());

        if (expense != null)
        {
            _isEditing = true;
            EditingId = expense.Id;
            LoadFromExpense(expense);
        }
    }

    public int EditingId { get; }

    public bool IsEditing => _isEditing;

    public ObservableCollection<LookupItem> Companies { get; }
    public ObservableCollection<LookupItem> Departments { get; }
    public ObservableCollection<LookupItem> Categories { get; }
    public ObservableCollection<LookupItem> Contacts { get; }

    // Tab
    public string Tab
    {
        get => _tab;
        set => SetProperty(ref _tab, value);
    }

    // Basic
    public string EntryDate { get => _entryDate; set => SetProperty(ref _entryDate, value); }
    public string OccurDate { get => _occurDate; set => SetProperty(ref _occurDate, value); }
    public int CompanyId { get => _companyId; set => SetProperty(ref _companyId, value); }
    public int DeptId { get => _deptId; set => SetProperty(ref _deptId, value); }
    public int Cat1Id { get => _cat1Id; set => SetProperty(ref _cat1Id, value); }
    public int Cat2Id { get => _cat2Id; set => SetProperty(ref _cat2Id, value); }
    public string InvoiceContent { get => _invoiceContent; set => SetProperty(ref _invoiceContent, value); }
    public string Detail { get => _detail; set => SetProperty(ref _detail, value); }
    public double Quantity { get => _quantity; set => SetProperty(ref _quantity, value); }
    public double UnitPrice { get => _unitPrice; set => SetProperty(ref _unitPrice, value); }
    public double ExchangeRate { get => _exchangeRate; set => SetProperty(ref _exchangeRate, value); }
    public string DocId { get => _docId; set => SetProperty(ref _docId, value); }
    public double Prepaid { get => _prepaid; set => SetProperty(ref _prepaid, value); }
    public double ReimburseAmount { get => _reimburseAmount; set => SetProperty(ref _reimburseAmount, value); }

    // Settle
    public string PayMethod { get => _payMethod; set => SetProperty(ref _payMethod, value); }
    public int ContactId { get => _contactId; set => SetProperty(ref _contactId, value); }
    public string Payer { get => _payer; set => SetProperty(ref _payer, value); }
    public string Reimbursee { get => _reimbursee; set => SetProperty(ref _reimbursee, value); }
    public string TransferRecipient { get => _transferRecipient; set => SetProperty(ref _transferRecipient, value); }
    public string BatchInfo { get => _batchInfo; set => SetProperty(ref _batchInfo, value); }
    public string ReimburseStatus { get => _reimburseStatus; set => SetProperty(ref _reimburseStatus, value); }
    public string CollectDate { get => _collectDate; set => SetProperty(ref _collectDate, value); }
    public string Notes { get => _notes; set => SetProperty(ref _notes, value); }

    // Commands
    public ICommand TabBasicCommand => new RelayCommand(() => Tab = "basic");
    public ICommand TabSettleCommand => new RelayCommand(() => Tab = "settle");
    public ICommand SaveCommand => new RelayCommand(DoSave);
    public ICommand CancelCommand => new RelayCommand(() => { }, () => false); // handled in view

    private void DoSave()
    {
        var expense = ToExpense();
        _onSave(expense);
    }

    private void LoadFromExpense(Expense e)
    {
        _entryDate = e.EntryDate;
        _occurDate = e.OccurDate;
        _companyId = e.CompanyId;
        _deptId = e.DeptId;
        _cat1Id = e.Cat1Id;
        _cat2Id = e.Cat2Id;
        _invoiceContent = e.InvoiceContent;
        _detail = e.Detail;
        _quantity = e.Quantity;
        _unitPrice = e.UnitPrice;
        _exchangeRate = e.ExchangeRate;
        _docId = e.DocId;
        _prepaid = e.Prepaid;
        _reimburseAmount = e.ReimburseAmount;
        _payMethod = e.PayMethod;
        _contactId = e.ContactId;
        _payer = e.Payer;
        _reimbursee = e.Reimbursee;
        _transferRecipient = e.TransferRecipient;
        _batchInfo = e.BatchInfo;
        _reimburseStatus = e.ReimburseStatus;
        _collectDate = e.CollectDate;
        _notes = e.Notes;
    }

    private Expense ToExpense() => new()
    {
        Id = EditingId,
        EntryDate = _entryDate,
        OccurDate = _occurDate,
        CompanyId = _companyId,
        DeptId = _deptId,
        Cat1Id = _cat1Id,
        Cat2Id = _cat2Id,
        InvoiceContent = _invoiceContent,
        Detail = _detail,
        Quantity = _quantity,
        UnitPrice = _unitPrice,
        ExchangeRate = _exchangeRate,
        DocId = _docId,
        Prepaid = _prepaid,
        ReimburseAmount = _reimburseAmount,
        PayMethod = _payMethod,
        ContactId = _contactId,
        Payer = _payer,
        Reimbursee = _reimbursee,
        TransferRecipient = _transferRecipient,
        BatchInfo = _batchInfo,
        ReimburseStatus = _reimburseStatus,
        CollectDate = _collectDate,
        Notes = _notes
    };
}
