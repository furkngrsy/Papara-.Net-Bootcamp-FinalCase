using Papara_Final_Project.Models;
using Papara_Final_Project.UnitOfWorks;
using Papara_Final_Project.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using FluentValidation;
using FluentValidation.Results;

namespace Papara_Final_Project.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly IValidator<UserRegisterDTO> _registerValidator;
        private readonly IValidator<UserUpdateDTO> _updateValidator;

        public UserService(IUnitOfWork unitOfWork, IConfiguration configuration, IValidator<UserRegisterDTO> registerValidator, IValidator<UserUpdateDTO> updateValidator)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _registerValidator = registerValidator;
            _updateValidator = updateValidator;
        }

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"]
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public async Task<UserDTO> Authenticate(string email, string password)
        {
            var user = await _unitOfWork.Users.GetUserByEmail(email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
                return null;

            return new UserDTO
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Role = user.Role,
                WalletBalance = user.WalletBalance,
                PointsBalance = user.PointsBalance,
                Token = GenerateJwtToken(user)
            };
        }

        public async Task<ValidationResult> Register(UserRegisterDTO model)
        {
            var validationResult = await _registerValidator.ValidateAsync(model);
            if (!validationResult.IsValid)
            {
                return validationResult;
            }

            var user = new User
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(model.Password),
                Role = "User",
                WalletBalance = 0,
                PointsBalance = 0
            };

            await _unitOfWork.Users.AddUser(user);
            await _unitOfWork.CompleteAsync();
            return validationResult;
        }

        public async Task<ValidationResult> RegisterAdmin(UserRegisterDTO model)
        {
            var validationResult = await _registerValidator.ValidateAsync(model);
            if (!validationResult.IsValid)
            {
                return validationResult;
            }

            var admin = new User
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(model.Password),
                Role = "Admin",
                WalletBalance = 0,
                PointsBalance = 0
            };

            await _unitOfWork.Users.AddUser(admin);
            await _unitOfWork.CompleteAsync();
            return validationResult;
        }

        public async Task<ValidationResult> UpdateUser(UserUpdateDTO model, int userId)
        {
            var validationResult = await _updateValidator.ValidateAsync(model);
            if (!validationResult.IsValid)
            {
                return validationResult;
            }

            var existingUser = await _unitOfWork.Users.GetUserByEmail(model.Email);
            if (existingUser != null && existingUser.Id != userId)
            {
                validationResult.Errors.Add(new FluentValidation.Results.ValidationFailure("Email", "Email is already in use."));
                return validationResult;
            }

            var user = await _unitOfWork.Users.GetUserById(userId);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found");
            }

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Email = model.Email;

            if (!string.IsNullOrEmpty(model.Password))
            {
                user.Password = BCrypt.Net.BCrypt.HashPassword(model.Password);
            }

            await _unitOfWork.Users.UpdateUser(user);
            await _unitOfWork.CompleteAsync();
            return validationResult;
        }

        public async Task Delete(int id)
        {
            await _unitOfWork.Users.DeleteUser(id);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<User> GetUserById(int id)
        {
            return await _unitOfWork.Users.GetUserById(id);
        }

        public async Task<decimal> GetUserPoints(int userId)
        {
            var user = await _unitOfWork.Users.GetUserById(userId);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found");
            }

            return user.PointsBalance;
        }
    }
}
