namespace Papara_Final_Project.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string Tags { get; set; }

        public ICollection<ProductCategory> ProductCategories { get; set; }
    }
}
