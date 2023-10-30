namespace npascu_api_v1.Models.Entities
{
    public class User: ModelBase
    {
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public ICollection<UserItem> OwnedItems { get; set; } = new List<UserItem>();
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
