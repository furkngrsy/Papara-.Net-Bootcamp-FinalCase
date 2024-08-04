using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Papara_Final_Project.Models;
using Papara_Final_Project.Services;
using Papara_Final_Project.DTOs;

namespace Papara_Final_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register([FromBody] UserRegisterDTO model)
        {
            var user = new User
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                Password = model.Password,
                Role = "User", 
                WalletBalance = 0,
                PointsBalance = 0 
            };

            _userService.Register(user);
            return Ok();
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel model)
        {
            var userDto = _userService.Authenticate(model.Email, model.Password);

            if (userDto == null)
                return Unauthorized();

            return Ok(userDto);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("admin_register")]
        public IActionResult AdminRegister([FromBody] UserRegisterDTO model)
        {
            var admin = new User
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                Password = model.Password,
                Role = "Admin", 
                WalletBalance = 0, 
                PointsBalance = 0 
            };

            _userService.Register(admin);
            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("delete/{id}")]
        public IActionResult Delete(int id)
        {
            _userService.Delete(id);
            return Ok();
        }

        [Authorize]
        [HttpPut("update")]
        public IActionResult Update([FromBody] User model)
        {
            var user = new User
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                Password = model.Password,
                Role = model.Role
            };

            _userService.Update(user);
            return Ok();
        }
    }
}
