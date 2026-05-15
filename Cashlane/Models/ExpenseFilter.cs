namespace Cashlane.Models;

public class ExpenseFilter
{
    public int? CompanyId { get; set; }
    public int? DeptId { get; set; }
    public int? Cat1Id { get; set; }
    public int? Cat2Id { get; set; }
    public string? Status { get; set; }
    public string? Method { get; set; }
    public string? DateFrom { get; set; }
    public string? DateTo { get; set; }
    public string? Search { get; set; }
}
