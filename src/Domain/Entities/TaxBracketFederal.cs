namespace Domain.Entities;

public class TaxBracketFederal
{
    public int Id { get; set; }
    public int Year { get; set; }
    public string BracketsJson { get; set; } = "[]"; // [{"up_to":50000,"rate":0.1},...]
}
