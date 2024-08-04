using Papara_Final_Project.DTOs;
using Papara_Final_Project.Models;
using System.Threading.Tasks;

namespace Papara_Final_Project.Services
{
    public interface IUserService
    {
        Task<UserDTO> Authenticate(string email, string password);
        Task Register(User user);
        Task Update(User user);
        Task Delete(int id);
        Task<User> GetUserById(int id);
    }
}
