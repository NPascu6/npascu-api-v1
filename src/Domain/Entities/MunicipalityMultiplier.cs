namespace Domain.Entities;

public class MunicipalityMultiplier
{
    public int Id { get; set; }
    public string Canton { get; set; } = string.Empty;
    public string MunicipalityCode { get; set; } = string.Empty;
    public int Year { get; set; }
    public decimal Multiplier { get; set; }
}
