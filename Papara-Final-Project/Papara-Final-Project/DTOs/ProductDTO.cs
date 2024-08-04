public class ProductDTO
{
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public bool IsAvailable { get; set; }
    public int Stock { get; set; }
    public decimal RewardRate { get; set; } // Yüzdelik puan miktarı
    public decimal MaxReward { get; set; } // Maksimum puan miktarı
    public List<int> CategoryIds { get; set; }
}
