using FluentValidation;
using Papara_Final_Project.DTOs;
using Papara_Final_Project.Repositories;
using System.Threading;
using System.Threading.Tasks;

namespace Papara_Final_Project.Validations
{
    public class UserUpdateValidator : AbstractValidator<UserUpdateDTO>
    {
        private readonly IUserRepository _userRepository;

        public UserUpdateValidator(IUserRepository userRepository)
        {
            _userRepository = userRepository;

            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.")
                .MustAsync((user, email, cancellationToken) => EmailNotInUse(user.Id, email, cancellationToken)).WithMessage("Email is already in use.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
                .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
                .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
                .Matches("[0-9]").WithMessage("Password must contain at least one number.")
                .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character.");
        }

        private async Task<bool> EmailNotInUse(int userId, string email, CancellationToken cancellationToken)
        {
            var existingUser = await _userRepository.GetUserByEmail(email);
            return existingUser == null || existingUser.Id == userId;
        }
    }
}
