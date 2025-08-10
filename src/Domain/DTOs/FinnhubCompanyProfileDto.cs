namespace Domain.DTOs;

/// <summary>Company profile basic information.</summary>
public class FinnhubCompanyProfileDto
{
    public string ticker { get; set; } = string.Empty;
    public string name { get; set; } = string.Empty;
    public string? currency { get; set; }
    public string? exchange { get; set; }
}
