namespace Domain.Entities;

public class TaxBracketCantonal
{
    public int Id { get; set; }
    public string Canton { get; set; } = string.Empty;
    public int Year { get; set; }
    public string BracketsJson { get; set; } = "[]";
}
