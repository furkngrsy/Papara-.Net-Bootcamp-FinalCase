using Papara_Final_Project.Models;
using System.Threading.Tasks;

namespace Papara_Final_Project.Repositories
{
    public interface IUserRepository
    {
        Task<bool> EmailExists(string email);
        Task AddUser(User user);
        Task<User> GetUserByEmail(string email);
        Task UpdateUser(User user);
        Task DeleteUser(int id);
        Task<User> GetUserById(int id);
    }
}
