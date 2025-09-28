using Clinic.API.API.Dtos.InvoiceDtos;
using FluentValidation;

namespace Clinic.API.API.Validators.InvoiceValidators
{
    public class CreateInvoiceItemDtoValidator : AbstractValidator<CreateInvoiceItemDto>
    {
        public CreateInvoiceItemDtoValidator()
        {
            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Item description is required.")
                .MaximumLength(200).WithMessage("Description cannot exceed 200 characters.");

            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be at least 1.");

            RuleFor(x => x.UnitPrice)
                .GreaterThanOrEqualTo(0).WithMessage("Unit price cannot be negative.");
        }
    }
}
