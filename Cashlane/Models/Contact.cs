namespace Cashlane.Models;

public class Contact
{
    public int Id { get; set; }
    public string ShortName { get; set; } = "";
    public string FullName { get; set; } = "";
    public string Phone { get; set; } = "";
    public string Notes { get; set; } = "";
}
