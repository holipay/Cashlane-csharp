using System.Collections.ObjectModel;
using System.Windows.Input;
using Cashlane.Models;
using Cashlane.Services;

namespace Cashlane.ViewModels;

public class ContactsViewModel : ViewModelBase
{
    private readonly DatabaseService _db;
    private readonly MainViewModel _mainVm;
    private Contact? _selectedContact;
    private string _shortName = "";
    private string _fullName = "";
    private string _phone = "";
    private string _notes = "";
    private bool _isEditing;
    private int _editingId;

    public ContactsViewModel(DatabaseService db, MainViewModel mainVm)
    {
        _db = db;
        _mainVm = mainVm;
        LoadContacts();
    }

    public ObservableCollection<Contact> Contacts { get; } = new();

    public Contact? SelectedContact
    {
        get => _selectedContact;
        set
        {
            if (SetProperty(ref _selectedContact, value) && value != null)
                LoadFromContact(value);
        }
    }

    public string ShortName { get => _shortName; set => SetProperty(ref _shortName, value); }
    public string FullName { get => _fullName; set => SetProperty(ref _fullName, value); }
    public string Phone { get => _phone; set => SetProperty(ref _phone, value); }
    public string Notes { get => _notes; set => SetProperty(ref _notes, value); }
    public bool IsEditing { get => _isEditing; set => SetProperty(ref _isEditing, value); }

    public Func<string, string, bool>? ConfirmDelete { get; set; }

    public ICommand NewCommand => new RelayCommand(() =>
    {
        ClearForm();
        IsEditing = false;
    });

    public ICommand SaveCommand => new RelayCommand(() =>
    {
        if (string.IsNullOrWhiteSpace(ShortName))
        {
            _mainVm.ToastVm.Show("⚠️ 简称不能为空");
            return;
        }

        var contact = new Contact
        {
            ShortName = ShortName,
            FullName = FullName,
            Phone = Phone,
            Notes = Notes
        };

        if (IsEditing && _editingId > 0)
        {
            _db.UpdateContact(_editingId, contact);
            _mainVm.ToastVm.Show("✅ 联系人已更新");
        }
        else
        {
            _db.CreateContact(contact);
            _mainVm.ToastVm.Show("✅ 联系人已添加");
        }
        LoadContacts();
        ClearForm();
    });

    public ICommand DeleteCommand => new RelayCommand(() =>
    {
        if (SelectedContact == null) return;
        var confirmed = ConfirmDelete?.Invoke("确认删除", $"确定要删除联系人「{SelectedContact.ShortName}」吗？") ?? false;
        if (confirmed)
        {
            _db.DeleteContact(SelectedContact.Id);
            _mainVm.ToastVm.Show("🗑️ 联系人已删除");
            LoadContacts();
            ClearForm();
        }
    });

    private void LoadContacts()
    {
        Contacts.Clear();
        foreach (var c in _db.GetAllContacts())
            Contacts.Add(c);
    }

    private void LoadFromContact(Contact c)
    {
        _editingId = c.Id;
        IsEditing = true;
        ShortName = c.ShortName;
        FullName = c.FullName;
        Phone = c.Phone;
        Notes = c.Notes;
    }

    private void ClearForm()
    {
        _editingId = 0;
        IsEditing = false;
        ShortName = "";
        FullName = "";
        Phone = "";
        Notes = "";
        SelectedContact = null;
    }
}
