using System.ComponentModel.DataAnnotations;

namespace npascu_api_v1.Data.Models
{
    public class Role
    {
        public int Id { get; set; }
        [MaxLength(20)] public required string Name { get; init; }

        public ICollection<UserRole> UserRoles { get; init; } = new List<UserRole>();
    }
}