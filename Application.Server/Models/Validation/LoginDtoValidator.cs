using Application.Server.Models.DTOs;
using FluentValidation;

namespace Application.Server.Models.Validation
{
    public class LoginDtoValidator : AbstractValidator<LoginDto>
    {
        public LoginDtoValidator()
        {
            RuleFor(loginDto => loginDto.Email).NotNull();
        }
    }
}
