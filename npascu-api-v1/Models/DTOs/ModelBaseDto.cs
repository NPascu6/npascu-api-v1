namespace npascu_api_v1.Models.DTOs
{
    public class ModelBaseDto
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; } 
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
