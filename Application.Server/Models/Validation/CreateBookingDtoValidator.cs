using Application.Server.Models.DTOs;
using FluentValidation;

namespace Application.Server.Models.Validation
{
    public class CreateBookingDtoValidator : AbstractValidator<CreateBookingDto>
    {
       public CreateBookingDtoValidator() {
            RuleFor(dto => dto.CoworkingId).NotNull();
            RuleFor(dto => dto.Name).NotNull();
            RuleFor(dto => dto.Email).NotNull().EmailAddress();
            RuleFor(dto => dto.WorkspaceType).NotNull();
            RuleFor(dto => dto.Seats).NotNull().GreaterThan(0);
            RuleFor(dto => dto.StartDate).NotNull();
            RuleFor(dto => dto.EndDate).NotNull();
            RuleFor(dto => dto.StartTime).NotNull();
            RuleFor(dto => dto.EndTime).NotNull();
        }
    }
}
