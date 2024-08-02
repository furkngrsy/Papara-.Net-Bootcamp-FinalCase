namespace Papara_Final_Project.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public bool IsActive { get; set; }
        public decimal PointsPercentage { get; set; }
        public decimal MaxPoints { get; set; }

        public ICollection<ProductCategory> ProductCategories { get; set; }
    }
}
