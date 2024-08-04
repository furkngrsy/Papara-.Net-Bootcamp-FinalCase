using Papara_Final_Project.DTOs;
using Papara_Final_Project.Models;

namespace Papara_Final_Project.Services
{
    public interface IUserService
    {
        UserDTO Authenticate(string email, string password); // Dönüş tipi UserDTO olarak güncellendi
        UserDTO Register(User user);
        void Update(User user);
        void Delete(int id);
    }
}
