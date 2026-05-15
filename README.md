## Cashlane C# 版

基于 [Cashlane-tauri](https://github.com/holipay/Cashlane-tauri) 项目转换的 C# WPF 版本。

### 技术栈

| 原版 (Tauri) | C# 版 |
|---|---|
| Tauri 2 (Rust) | .NET 8 + WPF |
| React 18 + TypeScript | WPF XAML + MVVM |
| Vite | MSBuild |
| rusqlite | Dapper + Microsoft.Data.Sqlite |
| SQLite | SQLite |

### 项目结构

```
Cashlane-csharp/
├── Cashlane.sln
└── Cashlane/
    ├── Cashlane.csproj
    ├── App.xaml / App.xaml.cs          # 应用入口 + DI
    ├── Models/
    │   ├── Expense.cs                  # 费用模型
    │   ├── ExpenseFilter.cs            # 筛选条件
    │   ├── LookupItem.cs               # 下拉选项
    │   └── Stats.cs                    # 统计数据
    ├── ViewModels/
    │   ├── ViewModelBase.cs            # MVVM 基类
    │   ├── RelayCommand.cs             # 命令实现
    │   ├── MainViewModel.cs            # 主窗口 VM
    │   ├── StatsViewModel.cs           # 统计卡片 VM
    │   ├── FilterViewModel.cs          # 筛选面板 VM
    │   ├── ExpenseTableViewModel.cs    # 费用表格 VM
    │   ├── ExpenseModalViewModel.cs    # 弹窗表单 VM
    │   └── ToastViewModel.cs           # Toast 提示 VM
    ├── Views/
    │   ├── MainWindow.xaml             # 主窗口
    │   ├── SidebarView.xaml            # 左侧导航
    │   ├── StatsCardsView.xaml         # 统计卡片
    │   ├── FilterPanelView.xaml        # 筛选面板
    │   ├── ExpenseTableView.xaml       # 费用表格
    │   ├── ExpenseModalView.xaml       # 新增/编辑弹窗
    │   └── ToastView.xaml              # Toast 提示
    ├── Services/
    │   └── DatabaseService.cs          # SQLite 数据库服务
    ├── Converters/
    │   └── Converters.cs               # WPF 值转换器
    └── Assets/
        └── app.ico                     # 应用图标
```

### 运行方式

```bash
# 确保安装了 .NET 8 SDK
dotnet restore
dotnet run --project Cashlane
```

### 功能对照

- ✅ 仪表盘统计卡片（本月费用笔数、报销合计、待审批、预付余额）
- ✅ 费用记录 CRUD（新增、编辑、删除）
- ✅ 多条件筛选（公司、部门、科目、状态、日期、搜索）
- ✅ 费用表格展示（排序、选中、状态徽章）
- ✅ 新增/编辑弹窗（基本信息 + 结算信息双 Tab）
- ✅ SQLite 数据库 + 示例数据
- ✅ 左侧导航栏（功能分类）
- ✅ Toast 提示
- 🚧 导入/导出（占位）
- 🚧 报表统计（占位）
- 🚧 联系人/科目分类管理（占位）
