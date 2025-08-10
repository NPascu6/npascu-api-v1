namespace Domain.Entities;

public class CantonRuleSet
{
    public int Id { get; set; }
    public string Canton { get; set; } = string.Empty; // e.g., ZH
    public int Year { get; set; }
    public string JsonRules { get; set; } = "{}";
}
