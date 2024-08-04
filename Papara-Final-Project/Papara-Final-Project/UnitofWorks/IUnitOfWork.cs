using Papara_Final_Project.Repositories;

namespace Papara_Final_Project.UnitOfWorks
{
    public interface IUnitOfWork : IDisposable
    {
        IProductRepository Products { get; }
        IUserRepository Users { get; }
        ICategoryRepository Categories { get; }
        Task<int> CompleteAsync();
    }
}
