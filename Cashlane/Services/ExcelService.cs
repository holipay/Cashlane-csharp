using ClosedXML.Excel;
using Cashlane.Models;

namespace Cashlane.Services;

public class ExcelService
{
    // ─── Export ────────────────────────────────────────────────────

    public void ExportExpenses(string filePath, List<Expense> expenses)
    {
        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("费用记录");

        // Headers
        var headers = new[]
        {
            "单据ID", "填单日期", "发生日期", "公司", "部门",
            "一级科目", "二级科目", "发票内容", "费用明细",
            "数量", "单价", "汇率", "预付/代付", "报销费用",
            "供应商", "付款方式", "付款人", "报销人", "转账收款",
            "批次/张数", "报销状态", "领款日期", "备注"
        };

        for (int i = 0; i < headers.Length; i++)
        {
            var cell = ws.Cell(1, i + 1);
            cell.Value = headers[i];
            cell.Style.Font.Bold = true;
            cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#F0F0F0");
            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        }

        // Data rows
        for (int r = 0; r < expenses.Count; r++)
        {
            var e = expenses[r];
            int row = r + 2;
            ws.Cell(row, 1).Value = e.DocId;
            ws.Cell(row, 2).Value = e.EntryDate;
            ws.Cell(row, 3).Value = e.OccurDate;
            ws.Cell(row, 4).Value = e.CompanyName;
            ws.Cell(row, 5).Value = e.DeptName;
            ws.Cell(row, 6).Value = e.Cat1Name;
            ws.Cell(row, 7).Value = e.Cat2Name;
            ws.Cell(row, 8).Value = e.InvoiceContent;
            ws.Cell(row, 9).Value = e.Detail;
            ws.Cell(row, 10).Value = e.Quantity;
            ws.Cell(row, 11).Value = e.UnitPrice;
            ws.Cell(row, 12).Value = e.ExchangeRate;
            ws.Cell(row, 13).Value = e.Prepaid;
            ws.Cell(row, 14).Value = e.ReimburseAmount;
            ws.Cell(row, 15).Value = e.ContactName;
            ws.Cell(row, 16).Value = e.PayMethod;
            ws.Cell(row, 17).Value = e.Payer;
            ws.Cell(row, 18).Value = e.Reimbursee;
            ws.Cell(row, 19).Value = e.TransferRecipient;
            ws.Cell(row, 20).Value = e.BatchInfo;
            ws.Cell(row, 21).Value = e.ReimburseStatus;
            ws.Cell(row, 22).Value = e.CollectDate;
            ws.Cell(row, 23).Value = e.Notes;

            // Money columns number format
            for (int c = 10; c <= 14; c++)
                ws.Cell(row, c).Style.NumberFormat.Format = "#,##0.00";
        }

        // Auto-fit columns
        ws.Columns().AdjustToContents();

        // Freeze header row
        ws.SheetView.FreezeRows(1);

        workbook.SaveAs(filePath);
    }

    // ─── Import ───────────────────────────────────────────────────

    public List<Expense> ImportExpenses(string filePath)
    {
        using var workbook = new XLWorkbook(filePath);
        var ws = workbook.Worksheet(1);
        var rows = ws.RangeUsed()?.RowsUsed();
        if (rows == null) return new List<Expense>();

        // Build header map
        var headerRow = rows.First();
        var headerMap = new Dictionary<string, int>();
        int col = 1;
        foreach (var cell in headerRow.Cells())
        {
            var name = cell.GetString().Trim();
            if (!string.IsNullOrEmpty(name))
                headerMap[name] = col;
            col++;
        }

        var expenses = new List<Expense>();

        foreach (var row in rows.Skip(1))
        {
            var e = new Expense();

            e.DocId = GetString(row, headerMap, "单据ID");
            e.EntryDate = GetString(row, headerMap, "填单日期");
            e.OccurDate = GetString(row, headerMap, "发生日期");
            e.CompanyName = GetString(row, headerMap, "公司");
            e.DeptName = GetString(row, headerMap, "部门");
            e.Cat1Name = GetString(row, headerMap, "一级科目");
            e.Cat2Name = GetString(row, headerMap, "二级科目");
            e.InvoiceContent = GetString(row, headerMap, "发票内容");
            e.Detail = GetString(row, headerMap, "费用明细");
            e.Quantity = GetDouble(row, headerMap, "数量", 1);
            e.UnitPrice = GetDouble(row, headerMap, "单价");
            e.ExchangeRate = GetDouble(row, headerMap, "汇率", 1);
            e.Prepaid = GetDouble(row, headerMap, "预付/代付");
            e.ReimburseAmount = GetDouble(row, headerMap, "报销费用");
            e.ContactName = GetString(row, headerMap, "供应商");
            e.PayMethod = GetString(row, headerMap, "付款方式", "报销");
            e.Payer = GetString(row, headerMap, "付款人");
            e.Reimbursee = GetString(row, headerMap, "报销人");
            e.TransferRecipient = GetString(row, headerMap, "转账收款");
            e.BatchInfo = GetString(row, headerMap, "批次/张数");
            e.ReimburseStatus = GetString(row, headerMap, "报销状态", "填单");
            e.CollectDate = GetString(row, headerMap, "领款日期");
            e.Notes = GetString(row, headerMap, "备注");

            // Skip empty rows
            if (string.IsNullOrWhiteSpace(e.EntryDate) && string.IsNullOrWhiteSpace(e.InvoiceContent))
                continue;

            expenses.Add(e);
        }

        return expenses;
    }

    // ─── Helpers ───────────────────────────────────────────────────

    private static string GetString(IXLRangeRow row, Dictionary<string, int> map, string key, string defaultValue = "")
    {
        if (!map.TryGetValue(key, out int col)) return defaultValue;
        var cell = row.Cell(col);
        return cell.GetString().Trim();
    }

    private static double GetDouble(IXLRangeRow row, Dictionary<string, int> map, string key, double defaultValue = 0)
    {
        if (!map.TryGetValue(key, out int col)) return defaultValue;
        var cell = row.Cell(col);
        if (cell.TryGetValue(out double d)) return d;
        if (double.TryParse(cell.GetString(), out d)) return d;
        return defaultValue;
    }
}
