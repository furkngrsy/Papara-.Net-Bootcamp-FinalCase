using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Papara_Final_Project.Models;
using Papara_Final_Project.Repositories;
using Papara_Final_Project.DTOs;

namespace Papara_Final_Project.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public UserService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public UserDTO Authenticate(string email, string password)
        {
            var user = _userRepository.GetUserByEmail(email);
            if (user == null || user.Password != password)
                return null;

            // JWT token oluşturma işlemi
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"]
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return new UserDTO
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Role = user.Role,
                WalletBalance = user.WalletBalance,
                PointsBalance = user.PointsBalance,
                Token = tokenString
            };
        }

        public UserDTO Register(User user)
        {
            _userRepository.AddUser(user);
            return Authenticate(user.Email, user.Password); // Kullanıcı kaydından sonra token oluşturup döndürmek için Authenticate metodunu çağırıyoruz
        }


        public void Update(User user)
        {
            _userRepository.UpdateUser(user);
        }

        public void Delete(int id)
        {
            _userRepository.DeleteUser(id);
        }
    }
}
