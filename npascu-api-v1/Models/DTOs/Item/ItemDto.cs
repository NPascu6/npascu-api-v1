﻿namespace npascu_api_v1.Models.DTOs.Item
{
    public class ItemDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
    }
}
