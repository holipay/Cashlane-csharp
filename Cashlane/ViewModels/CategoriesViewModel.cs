using System.Collections.ObjectModel;
using System.Windows.Input;
using Cashlane.Models;
using Cashlane.Services;

namespace Cashlane.ViewModels;

public class CategoriesViewModel : ViewModelBase
{
    private readonly DatabaseService _db;
    private readonly MainViewModel _mainVm;
    private Category? _selectedCategory;
    private string _name = "";
    private int _parentId;
    private bool _isEditing;
    private int _editingId;

    public CategoriesViewModel(DatabaseService db, MainViewModel mainVm)
    {
        _db = db;
        _mainVm = mainVm;
        LoadCategories();
    }

    public ObservableCollection<Category> Categories { get; } = new();
    public ObservableCollection<LookupItem> ParentOptions { get; } = new();

    public Category? SelectedCategory
    {
        get => _selectedCategory;
        set
        {
            if (SetProperty(ref _selectedCategory, value) && value != null)
                LoadFromCategory(value);
        }
    }

    public string Name { get => _name; set => SetProperty(ref _name, value); }
    public int ParentId { get => _parentId; set => SetProperty(ref _parentId, value); }
    public bool IsEditing { get => _isEditing; set => SetProperty(ref _isEditing, value); }

    public Func<string, string, bool>? ConfirmDelete { get; set; }

    public ICommand NewCommand => new RelayCommand(() =>
    {
        ClearForm();
        IsEditing = false;
    });

    public ICommand SaveCommand => new RelayCommand(() =>
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            _mainVm.ToastVm.Show("⚠️ 分类名称不能为空");
            return;
        }

        var category = new Category
        {
            Name = Name,
            ParentId = ParentId
        };

        if (IsEditing && _editingId > 0)
        {
            _db.UpdateCategory(_editingId, category);
            _mainVm.ToastVm.Show("✅ 分类已更新");
        }
        else
        {
            _db.CreateCategory(category);
            _mainVm.ToastVm.Show("✅ 分类已添加");
        }
        LoadCategories();
        ClearForm();
    });

    public ICommand DeleteCommand => new RelayCommand(() =>
    {
        if (SelectedCategory == null) return;
        var confirmed = ConfirmDelete?.Invoke("确认删除", $"确定要删除分类「{SelectedCategory.Name}」吗？") ?? false;
        if (confirmed)
        {
            _db.DeleteCategory(SelectedCategory.Id);
            _mainVm.ToastVm.Show("🗑️ 分类已删除");
            LoadCategories();
            ClearForm();
        }
    });

    private void LoadCategories()
    {
        Categories.Clear();
        ParentOptions.Clear();
        ParentOptions.Add(new LookupItem { Id = 0, Name = "（无，作为一级科目）" });

        foreach (var c in _db.GetAllCategories())
        {
            Categories.Add(c);
            if (c.ParentId == 0)
                ParentOptions.Add(new LookupItem { Id = c.Id, Name = c.Name });
        }
    }

    private void LoadFromCategory(Category c)
    {
        _editingId = c.Id;
        IsEditing = true;
        Name = c.Name;
        ParentId = c.ParentId;
    }

    private void ClearForm()
    {
        _editingId = 0;
        IsEditing = false;
        Name = "";
        ParentId = 0;
        SelectedCategory = null;
    }
}
