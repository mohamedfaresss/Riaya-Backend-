using FluentValidation;
using Riaya.Application.Features.Bookings.DTOs;

namespace Riaya.Application.Validators;

public class CreateBookingRequestValidator : AbstractValidator<CreateBookingRequest>
{
    public CreateBookingRequestValidator()
    {
        RuleFor(x => x.TimeSlotId)
            .NotEmpty().WithMessage("TimeSlotId is required");

        RuleFor(x => x.Reason)
            .MaximumLength(500).WithMessage("Reason cannot exceed 500 characters");
    }
}