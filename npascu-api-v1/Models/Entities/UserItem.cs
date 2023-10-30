namespace npascu_api_v1.Models.Entities
{
    public class UserItem: ModelBase
    {
        public int UserId { get; set; }
        public User? User { get; set; }
        public int ItemId { get; set; }
        public Item? Item { get; set; }
    }
}
