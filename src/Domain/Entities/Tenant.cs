using System;

namespace Domain.Entities;

public class Tenant
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public TenantStatus Status { get; set; } = TenantStatus.Active;
    public string Plan { get; set; } = "free";
    public string? StripeCustomerId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<ApiKey> ApiKeys { get; set; } = new List<ApiKey>();
}

public enum TenantStatus
{
    Active,
    Suspended
}
