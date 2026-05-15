namespace Cashlane.Models;

public class Expense
{
    public int Id { get; set; }
    public string Sid { get; set; } = "";
    public string EntryDate { get; set; } = "";
    public string OccurDate { get; set; } = "";
    public int CompanyId { get; set; }
    public string CompanyName { get; set; } = "";
    public int DeptId { get; set; }
    public string DeptName { get; set; } = "";
    public int Cat1Id { get; set; }
    public string Cat1Name { get; set; } = "";
    public int Cat2Id { get; set; }
    public string Cat2Name { get; set; } = "";
    public string InvoiceContent { get; set; } = "";
    public string Detail { get; set; } = "";
    public double Quantity { get; set; } = 1;
    public double UnitPrice { get; set; }
    public double ExchangeRate { get; set; } = 1;
    public double Prepaid { get; set; }
    public double ReimburseAmount { get; set; }
    public int ContactId { get; set; }
    public string ContactName { get; set; } = "";
    public string PayMethod { get; set; } = "报销";
    public string Payer { get; set; } = "";
    public string Reimbursee { get; set; } = "";
    public string TransferRecipient { get; set; } = "";
    public string DocId { get; set; } = "";
    public string BatchInfo { get; set; } = "";
    public string ReimburseStatus { get; set; } = "填单";
    public int IsAsset { get; set; }
    public string CollectDate { get; set; } = "";
    public string Notes { get; set; } = "";
}
