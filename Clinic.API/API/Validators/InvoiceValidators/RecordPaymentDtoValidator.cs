using Clinic.API.API.Dtos.InvoiceDtos;
using FluentValidation;

namespace Clinic.API.API.Validators.InvoiceValidators
{
    public class RecordPaymentDtoValidator : AbstractValidator<RecordPaymentDto>
    {
        public RecordPaymentDtoValidator()
        {
            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("Payment amount must be greater than zero.");
        }
    }
}
