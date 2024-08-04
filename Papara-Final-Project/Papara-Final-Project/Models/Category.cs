using Papara_Final_Project.Models;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Url { get; set; }
    public string Tag { get; set; }
    public ICollection<ProductMatchCategory> ProductMatchCategories { get; set; }
}
