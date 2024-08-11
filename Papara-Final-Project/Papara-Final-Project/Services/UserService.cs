﻿using Papara_Final_Project.Models;
using Papara_Final_Project.UnitOfWorks;
using Papara_Final_Project.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace Papara_Final_Project.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;

        public UserService(IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
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

        public async Task Register(User user)
        {
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            await _unitOfWork.Users.AddUser(user);
            await _unitOfWork.CompleteAsync();
        }

        public async Task Update(User user)
        {
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            await _unitOfWork.Users.UpdateUser(user);
            await _unitOfWork.CompleteAsync();
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
