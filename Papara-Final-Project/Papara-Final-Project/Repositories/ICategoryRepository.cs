using System.Collections.Generic;
using System.Threading.Tasks;
using Papara_Final_Project.Models;

public interface ICategoryRepository
{
    Task<List<Category>> GetCategoriesByIds(List<int> categoryIds);
}
