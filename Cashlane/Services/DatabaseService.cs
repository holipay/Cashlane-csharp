using System.Data;
using Dapper;
using Microsoft.Data.Sqlite;
using Cashlane.Models;

namespace Cashlane.Services;

public class DatabaseService : IDisposable
{
    private readonly string _connectionString;
    private IDbConnection? _connection;

    public DatabaseService(string dbPath = "cashlane.db")
    {
        _connectionString = $"Data Source={dbPath}";
    }

    private IDbConnection GetConnection()
    {
        if (_connection == null)
        {
            _connection = new SqliteConnection(_connectionString);
            ((SqliteConnection)_connection).Open();
        }
        return _connection;
    }

    public void Initialize()
    {
        var conn = GetConnection();
        InitDb(conn);
    }

    private void InitDb(IDbConnection conn)
    {
        conn.Execute(@"
            CREATE TABLE IF NOT EXISTS companies (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                name TEXT NOT NULL
            );
            CREATE TABLE IF NOT EXISTS departments (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                name TEXT NOT NULL
            );
            CREATE TABLE IF NOT EXISTS categories (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                parent_id INTEGER DEFAULT 0,
                name TEXT NOT NULL
            );
            CREATE TABLE IF NOT EXISTS contacts (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                short_name TEXT NOT NULL,
                full_name TEXT DEFAULT '',
                phone TEXT DEFAULT '',
                notes TEXT DEFAULT ''
            );
            CREATE TABLE IF NOT EXISTS expenses (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                sid TEXT DEFAULT '',
                entry_date TEXT NOT NULL,
                occur_date TEXT NOT NULL,
                company_id INTEGER DEFAULT 0,
                dept_id INTEGER DEFAULT 0,
                cat1_id INTEGER DEFAULT 0,
                cat2_id INTEGER DEFAULT 0,
                invoice_content TEXT DEFAULT '',
                detail TEXT DEFAULT '',
                quantity REAL DEFAULT 1,
                unit_price REAL DEFAULT 0,
                exchange_rate REAL DEFAULT 1,
                prepaid REAL DEFAULT 0,
                reimburse_amount REAL DEFAULT 0,
                contact_id INTEGER DEFAULT 0,
                pay_method TEXT DEFAULT '报销',
                payer TEXT DEFAULT '',
                reimbursee TEXT DEFAULT '',
                transfer_recipient TEXT DEFAULT '',
                doc_id TEXT DEFAULT '',
                batch_info TEXT DEFAULT '',
                reimburse_status TEXT DEFAULT '填单',
                is_asset INTEGER DEFAULT 0,
                collect_date TEXT DEFAULT '',
                notes TEXT DEFAULT ''
            );
        ");

        var count = conn.ExecuteScalar<long>("SELECT COUNT(*) FROM companies");
        if (count == 0)
            SeedDemoData(conn);
    }

    private void SeedDemoData(IDbConnection conn)
    {
        conn.Execute("INSERT INTO companies (name) VALUES ('示例科技'), ('示例商贸')");

        conn.Execute("INSERT INTO departments (name) VALUES ('技术部'), ('市场部'), ('财务部'), ('行政部'), ('人事部')");

        conn.Execute(@"
            INSERT INTO categories (id, parent_id, name) VALUES
                (1, 0, '办公费'), (2, 0, '差旅费'), (3, 0, '招待费'),
                (4, 0, '营销费'), (5, 0, '汽车费'), (6, 0, '福利费'),
                (7, 1, '文具'), (8, 1, '耗材'), (9, 1, '软件'),
                (10, 2, '交通'), (11, 2, '住宿'),
                (12, 3, '餐饮'), (13, 3, '礼品'),
                (14, 4, '广告'), (15, 4, '设计'),
                (16, 5, '油费'), (17, 6, '团建');
        ");

        conn.Execute(@"
            INSERT INTO contacts (short_name, full_name) VALUES
                ('得力办公', '得力办公用品有限公司'),
                ('12306', '中国铁路客户服务中心'),
                ('海底捞', '海底捞国际控股有限公司'),
                ('百度', '百度在线网络技术有限公司'),
                ('中石化', '中国石油化工集团');
        ");

        conn.Execute(@"
            INSERT INTO expenses (sid, entry_date, occur_date, company_id, dept_id, cat1_id, cat2_id,
                invoice_content, detail, quantity, unit_price, prepaid, reimburse_amount,
                contact_id, pay_method, reimbursee, reimburse_status, doc_id, notes) VALUES
                ('', '2026-05-14', '2026-05-14', 1, 1, 1, 7,
                 'A4打印纸采购', '得力A4打印纸 5箱', 5, 256, 0, 1280,
                 1, '报销', '张三', '完成', 'EXP-001', ''),
                ('', '2026-05-13', '2026-05-13', 1, 2, 2, 10,
                 '北京-上海高铁', '二等座 G101', 1, 553, 0, 553,
                 2, '报销', '李四', '签录', 'EXP-002', '出差报销'),
                ('', '2026-05-12', '2026-05-12', 2, 2, 3, 12,
                 '客户晚宴', '海底捞火锅', 1, 3680, 3680, 0,
                 3, '请款', '王五', '填单', 'EXP-003', '客户来访'),
                ('', '2026-05-11', '2026-05-10', 1, 2, 4, 14,
                 '百度推广费用', '5月推广预算', 1, 12000, 12000, 0,
                 4, '请款', '赵六', '完成', 'EXP-004', ''),
                ('', '2026-05-10', '2026-05-10', 2, 4, 5, 16,
                 '95号汽油', '中石化加油站', 1, 420, 0, 420,
                 5, '报销', '孙七', '取消', 'EXP-005', '已取消'),
                ('', '2026-05-09', '2026-05-09', 1, 1, 1, 9,
                 'JetBrains全家桶', '年度授权 x3', 3, 2499, 7497, 0,
                 0, '请款', '张三', '完成', 'EXP-006', '开发工具'),
                ('', '2026-05-08', '2026-05-08', 2, 5, 6, 17,
                 '部门团建活动', '户外拓展训练', 1, 8500, 0, 8500,
                 0, '报销', '周八', '签录', 'EXP-007', 'Q2团建'),
                ('', '2026-05-07', '2026-05-07', 1, 3, 1, 8,
                 '墨盒采购', 'HP原装墨盒 x2', 2, 380, 0, 760,
                 0, '报销', '钱八', '完成', 'EXP-008', '');
        ");
    }

    // ─── Expense CRUD ───────────────────────────────────────────────

    public List<Expense> GetExpenses(ExpenseFilter filter)
    {
        var conn = GetConnection();
        var sql = @"
            SELECT e.id, e.sid, e.entry_date AS EntryDate, e.occur_date AS OccurDate,
                   e.company_id AS CompanyId, COALESCE(c.name, '') AS CompanyName,
                   e.dept_id AS DeptId, COALESCE(d.name, '') AS DeptName,
                   e.cat1_id AS Cat1Id, COALESCE(cat1.name, '') AS Cat1Name,
                   e.cat2_id AS Cat2Id, COALESCE(cat2.name, '') AS Cat2Name,
                   e.invoice_content AS InvoiceContent, e.detail AS Detail,
                   e.quantity AS Quantity, e.unit_price AS UnitPrice,
                   e.exchange_rate AS ExchangeRate, e.prepaid AS Prepaid,
                   e.reimburse_amount AS ReimburseAmount,
                   e.contact_id AS ContactId, COALESCE(ct.short_name, '') AS ContactName,
                   e.pay_method AS PayMethod, e.payer AS Payer,
                   e.reimbursee AS Reimbursee, e.transfer_recipient AS TransferRecipient,
                   e.doc_id AS DocId, e.batch_info AS BatchInfo,
                   e.reimburse_status AS ReimburseStatus, e.is_asset AS IsAsset,
                   e.collect_date AS CollectDate, e.notes AS Notes
            FROM expenses e
            LEFT JOIN companies c ON c.id = e.company_id
            LEFT JOIN departments d ON d.id = e.dept_id
            LEFT JOIN categories cat1 ON cat1.id = e.cat1_id
            LEFT JOIN categories cat2 ON cat2.id = e.cat2_id
            LEFT JOIN contacts ct ON ct.id = e.contact_id
        ";

        var conditions = new List<string>();
        var parameters = new DynamicParameters();

        if (filter.CompanyId.HasValue)
        {
            conditions.Add("e.company_id = @CompanyId");
            parameters.Add("CompanyId", filter.CompanyId.Value);
        }
        if (filter.DeptId.HasValue)
        {
            conditions.Add("e.dept_id = @DeptId");
            parameters.Add("DeptId", filter.DeptId.Value);
        }
        if (filter.Cat1Id.HasValue)
        {
            conditions.Add("e.cat1_id = @Cat1Id");
            parameters.Add("Cat1Id", filter.Cat1Id.Value);
        }
        if (filter.Cat2Id.HasValue)
        {
            conditions.Add("e.cat2_id = @Cat2Id");
            parameters.Add("Cat2Id", filter.Cat2Id.Value);
        }
        if (!string.IsNullOrEmpty(filter.Status))
        {
            conditions.Add("e.reimburse_status = @Status");
            parameters.Add("Status", filter.Status);
        }
        if (!string.IsNullOrEmpty(filter.Method))
        {
            conditions.Add("e.pay_method = @Method");
            parameters.Add("Method", filter.Method);
        }
        if (!string.IsNullOrEmpty(filter.DateFrom))
        {
            conditions.Add("e.occur_date >= @DateFrom");
            parameters.Add("DateFrom", filter.DateFrom);
        }
        if (!string.IsNullOrEmpty(filter.DateTo))
        {
            conditions.Add("e.occur_date <= @DateTo");
            parameters.Add("DateTo", filter.DateTo);
        }
        if (!string.IsNullOrEmpty(filter.Search))
        {
            conditions.Add("(e.detail LIKE @Search OR e.invoice_content LIKE @Search OR e.notes LIKE @Search)");
            parameters.Add("Search", $"%{filter.Search}%");
        }

        if (conditions.Count > 0)
            sql += " WHERE " + string.Join(" AND ", conditions);

        sql += " ORDER BY e.id DESC";

        return conn.Query<Expense>(sql, parameters).ToList();
    }

    public int CreateExpense(Expense expense)
    {
        var conn = GetConnection();
        var docId = $"EXP-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() % 10000:D3}";

        conn.Execute(@"
            INSERT INTO expenses (sid, entry_date, occur_date, company_id, dept_id,
                cat1_id, cat2_id, invoice_content, detail, quantity, unit_price,
                exchange_rate, prepaid, reimburse_amount, contact_id, pay_method,
                payer, reimbursee, transfer_recipient, doc_id, batch_info,
                reimburse_status, is_asset, collect_date, notes)
            VALUES (@Sid, @EntryDate, @OccurDate, @CompanyId, @DeptId,
                @Cat1Id, @Cat2Id, @InvoiceContent, @Detail, @Quantity, @UnitPrice,
                @ExchangeRate, @Prepaid, @ReimburseAmount, @ContactId, @PayMethod,
                @Payer, @Reimbursee, @TransferRecipient, @DocId, @BatchInfo,
                @ReimburseStatus, @IsAsset, @CollectDate, @Notes);
        ", new
        {
            expense.Sid, expense.EntryDate, expense.OccurDate,
            expense.CompanyId, expense.DeptId, expense.Cat1Id, expense.Cat2Id,
            expense.InvoiceContent, expense.Detail, expense.Quantity, expense.UnitPrice,
            expense.ExchangeRate, expense.Prepaid, expense.ReimburseAmount,
            expense.ContactId, expense.PayMethod, expense.Payer, expense.Reimbursee,
            expense.TransferRecipient, DocId = docId, expense.BatchInfo,
            expense.ReimburseStatus, expense.IsAsset, expense.CollectDate, expense.Notes
        });

        return conn.ExecuteScalar<int>("SELECT last_insert_rowid()");
    }

    public void UpdateExpense(int id, Expense expense)
    {
        var conn = GetConnection();
        conn.Execute(@"
            UPDATE expenses SET
                sid=@Sid, entry_date=@EntryDate, occur_date=@OccurDate,
                company_id=@CompanyId, dept_id=@DeptId,
                cat1_id=@Cat1Id, cat2_id=@Cat2Id,
                invoice_content=@InvoiceContent, detail=@Detail,
                quantity=@Quantity, unit_price=@UnitPrice,
                exchange_rate=@ExchangeRate, prepaid=@Prepaid,
                reimburse_amount=@ReimburseAmount, contact_id=@ContactId,
                pay_method=@PayMethod, payer=@Payer,
                reimbursee=@Reimbursee, transfer_recipient=@TransferRecipient,
                doc_id=@DocId, batch_info=@BatchInfo,
                reimburse_status=@ReimburseStatus, is_asset=@IsAsset,
                collect_date=@CollectDate, notes=@Notes
            WHERE id=@Id
        ", new
        {
            expense.Sid, expense.EntryDate, expense.OccurDate,
            expense.CompanyId, expense.DeptId, expense.Cat1Id, expense.Cat2Id,
            expense.InvoiceContent, expense.Detail, expense.Quantity, expense.UnitPrice,
            expense.ExchangeRate, expense.Prepaid, expense.ReimburseAmount,
            expense.ContactId, expense.PayMethod, expense.Payer, expense.Reimbursee,
            expense.TransferRecipient, expense.DocId, expense.BatchInfo,
            expense.ReimburseStatus, expense.IsAsset, expense.CollectDate, expense.Notes,
            Id = id
        });
    }

    public void DeleteExpense(int id)
    {
        var conn = GetConnection();
        conn.Execute("DELETE FROM expenses WHERE id = @Id", new { Id = id });
    }

    // ─── Stats ──────────────────────────────────────────────────────

    public Stats GetStats()
    {
        var conn = GetConnection();
        var ym = DateTime.Now.ToString("yyyy-MM");

        var monthCount = conn.ExecuteScalar<int>(
            "SELECT COUNT(*) FROM expenses WHERE substr(occur_date,1,7) = @YM",
            new { YM = ym });

        var monthReimburse = conn.ExecuteScalar<double>(
            "SELECT COALESCE(SUM(reimburse_amount),0) FROM expenses WHERE substr(occur_date,1,7) = @YM",
            new { YM = ym });

        var pendingCount = conn.ExecuteScalar<int>(
            "SELECT COUNT(*) FROM expenses WHERE reimburse_status IN ('填单','签录')");

        var prepaidBalance = conn.ExecuteScalar<double>(
            "SELECT COALESCE(SUM(prepaid - reimburse_amount),0) FROM expenses WHERE prepaid > 0");

        return new Stats
        {
            MonthCount = monthCount,
            MonthReimburse = monthReimburse,
            PendingCount = pendingCount,
            PrepaidBalance = prepaidBalance
        };
    }

    // ─── Lookups ────────────────────────────────────────────────────

    public List<LookupItem> GetCompanies()
    {
        var conn = GetConnection();
        return conn.Query<LookupItem>("SELECT id AS Id, name AS Name FROM companies ORDER BY id").ToList();
    }

    public List<LookupItem> GetDepartments()
    {
        var conn = GetConnection();
        return conn.Query<LookupItem>("SELECT id AS Id, name AS Name FROM departments ORDER BY name").ToList();
    }

    public List<LookupItem> GetCategories(int parentId = 0)
    {
        var conn = GetConnection();
        return conn.Query<LookupItem>(
            "SELECT id AS Id, name AS Name FROM categories WHERE parent_id = @ParentId ORDER BY id",
            new { ParentId = parentId }).ToList();
    }

    public List<LookupItem> GetContacts()
    {
        var conn = GetConnection();
        return conn.Query<LookupItem>("SELECT id AS Id, short_name AS Name FROM contacts ORDER BY short_name").ToList();
    }

    // ─── Contact CRUD ─────────────────────────────────────────────

    public List<Contact> GetAllContacts()
    {
        var conn = GetConnection();
        return conn.Query<Contact>(
            "SELECT id AS Id, short_name AS ShortName, full_name AS FullName, phone AS Phone, notes AS Notes FROM contacts ORDER BY id").ToList();
    }

    public int CreateContact(Contact contact)
    {
        var conn = GetConnection();
        conn.Execute(@"
            INSERT INTO contacts (short_name, full_name, phone, notes)
            VALUES (@ShortName, @FullName, @Phone, @Notes);
        ", contact);
        return conn.ExecuteScalar<int>("SELECT last_insert_rowid()");
    }

    public void UpdateContact(int id, Contact contact)
    {
        var conn = GetConnection();
        conn.Execute(@"
            UPDATE contacts SET short_name=@ShortName, full_name=@FullName,
                phone=@Phone, notes=@Notes
            WHERE id=@Id
        ", new { contact.ShortName, contact.FullName, contact.Phone, contact.Notes, Id = id });
    }

    public void DeleteContact(int id)
    {
        var conn = GetConnection();
        conn.Execute("DELETE FROM contacts WHERE id = @Id", new { Id = id });
    }

    // ─── Category CRUD ────────────────────────────────────────────

    public List<Category> GetAllCategories()
    {
        var conn = GetConnection();
        return conn.Query<Category>(
            "SELECT id AS Id, parent_id AS ParentId, name AS Name FROM categories ORDER BY parent_id, id").ToList();
    }

    public int CreateCategory(Category category)
    {
        var conn = GetConnection();
        conn.Execute(@"
            INSERT INTO categories (parent_id, name) VALUES (@ParentId, @Name);
        ", category);
        return conn.ExecuteScalar<int>("SELECT last_insert_rowid()");
    }

    public void UpdateCategory(int id, Category category)
    {
        var conn = GetConnection();
        conn.Execute("UPDATE categories SET parent_id=@ParentId, name=@Name WHERE id=@Id",
            new { category.ParentId, category.Name, Id = id });
    }

    public void DeleteCategory(int id)
    {
        var conn = GetConnection();
        conn.Execute("DELETE FROM categories WHERE id = @Id", new { Id = id });
    }

    // ─── Name Resolution (for import) ──────────────────────────────

    public int ResolveCompanyId(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return 0;
        var conn = GetConnection();
        return conn.ExecuteScalar<int?>("SELECT id FROM companies WHERE name = @Name", new { Name = name }) ?? 0;
    }

    public int ResolveDeptId(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return 0;
        var conn = GetConnection();
        return conn.ExecuteScalar<int?>("SELECT id FROM departments WHERE name = @Name", new { Name = name }) ?? 0;
    }

    public int ResolveCategoryId(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return 0;
        var conn = GetConnection();
        return conn.ExecuteScalar<int?>("SELECT id FROM categories WHERE name = @Name", new { Name = name }) ?? 0;
    }

    public int ResolveContactId(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return 0;
        var conn = GetConnection();
        return conn.ExecuteScalar<int?>("SELECT id FROM contacts WHERE short_name = @Name", new { Name = name }) ?? 0;
    }

    public void Dispose()
    {
        (_connection as IDisposable)?.Dispose();
    }
}
