using Papara_Final_Project.DTOs;
using FluentValidation.Results;
using Papara_Final_Project.Models;

namespace Papara_Final_Project.Services
{
    public interface IUserService
    {
        Task<UserDTO> Authenticate(string email, string password);
        Task<ValidationResult> Register(UserRegisterDTO user);
        Task<ValidationResult> RegisterAdmin(UserRegisterDTO user);
        Task<ValidationResult> UpdateUser(UserUpdateDTO user, int userId);
        Task Delete(int id);
        Task<User> GetUserById(int id);
        Task<decimal> GetUserPoints(int userId);
    }
}
