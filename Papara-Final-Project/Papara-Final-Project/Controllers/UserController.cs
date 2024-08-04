using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Papara_Final_Project.Models;
using Papara_Final_Project.Services;
using Papara_Final_Project.DTOs;
using FluentValidation;
using FluentValidation.Results;
using System.Linq;
using System.Threading.Tasks;

namespace Papara_Final_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IValidator<UserRegisterDTO> _registerValidator;
        private readonly IValidator<UserUpdateDTO> _updateValidator;

        public UserController(IUserService userService, IValidator<UserRegisterDTO> registerValidator, IValidator<UserUpdateDTO> updateValidator)
        {
            _userService = userService;
            _registerValidator = registerValidator;
            _updateValidator = updateValidator;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDTO model)
        {
            ValidationResult result = await _registerValidator.ValidateAsync(model);

            if (!result.IsValid)
            {
                var errors = result.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }).ToList();
                return BadRequest(errors);
            }

            var user = new User
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                Password = model.Password,
                Role = "User", // Normal kullanıcı olarak kaydediliyor
                WalletBalance = 0, // Default value
                PointsBalance = 0 // Default value
            };

            await _userService.Register(user);
            return Ok();
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var userDto = await _userService.Authenticate(model.Email, model.Password);

            if (userDto == null)
                return Unauthorized();

            return Ok(userDto);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("admin_register")]
        public async Task<IActionResult> AdminRegister([FromBody] UserRegisterDTO model)
        {
            ValidationResult result = await _registerValidator.ValidateAsync(model);

            if (!result.IsValid)
            {
                var errors = result.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }).ToList();
                return BadRequest(errors);
            }

            var admin = new User
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                Password = model.Password,
                Role = "Admin", // Admin olarak kaydediliyor
                WalletBalance = 0, // Default value
                PointsBalance = 0 // Default value
            };

            await _userService.Register(admin);
            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userService.GetUserById(id);

            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            await _userService.Delete(id);
            return Ok();
        }

        [Authorize]
        [HttpPut("update")]
        public async Task<IActionResult> Update([FromBody] UserUpdateDTO model)
        {
            ValidationResult result = await _updateValidator.ValidateAsync(model);

            if (!result.IsValid)
            {
                var errors = result.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }).ToList();
                return BadRequest(errors);
            }

            var user = new User
            {
                Id = model.Id,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                Password = model.Password,
                // Rol bilgisi güncellenmeyecek
            };

            await _userService.Update(user);
            return Ok();
        }
    }
}
