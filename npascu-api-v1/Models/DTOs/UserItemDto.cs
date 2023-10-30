namespace npascu_api_v1.Models.DTOs
{
    public class UserItemDto: ModelBaseDto
    {
        public int UserId { get; set; }
        public UserDto? User { get; set; }
        public int ItemId { get; set; }
        public ItemDto? Item { get; set; }
    }
}
