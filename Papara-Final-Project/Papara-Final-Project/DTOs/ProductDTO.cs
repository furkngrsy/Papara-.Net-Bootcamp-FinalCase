﻿namespace Papara_Final_Project.DTOs
{
    public class ProductDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public bool IsAvailable { get; set; }
        public int Stock { get; set; }
        public decimal RewardRate { get; set; }
        public decimal MaxReward { get; set; }
        public List<int> CategoryIds { get; set; }
    }
}
