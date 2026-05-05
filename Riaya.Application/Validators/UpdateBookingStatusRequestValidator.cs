using FluentValidation;
using Riaya.Application.Features.Bookings.DTOs;
using Riaya.Domain.Enums;

namespace Riaya.Application.Validators;

public class UpdateBookingStatusValidator : AbstractValidator<UpdateBookingStatusDto>
{
    public UpdateBookingStatusValidator()
    {
        RuleFor(x => x.NewStatus)
            .IsInEnum()
            .WithMessage("Invalid booking status.")
            .NotEqual(BookingStatus.Pending)
            .WithMessage("Cannot manually set a booking back to Pending.");
    }
}
