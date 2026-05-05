using FluentValidation;
using Riaya.Application.Features.TimeSlots.DTOs;

namespace Riaya.Application.Validators;

public class CreateTimeSlotRequestValidator : AbstractValidator<CreateTimeSlotRequest>
{
    public CreateTimeSlotRequestValidator()
    {
        RuleFor(x => x.StartAtUtc)
            .NotEmpty()
            .GreaterThan(DateTime.UtcNow).WithMessage("Start time must be in the future");

        RuleFor(x => x.EndAtUtc)
            .NotEmpty()
            .GreaterThan(x => x.StartAtUtc).WithMessage("End time must be after start time");
    }
}