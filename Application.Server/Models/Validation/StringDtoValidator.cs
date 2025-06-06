using Application.Server.Models.DTOs;
using FluentValidation;

namespace Application.Server.Models.Validation
{
    public class StringDtoValidator:AbstractValidator<StringDto>
    {
       public StringDtoValidator() {
            RuleFor(stringDto => stringDto.Value).NotNull();
        }
    }
}
