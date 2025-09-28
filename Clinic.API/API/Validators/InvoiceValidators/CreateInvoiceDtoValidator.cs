using Clinic.API.API.Dtos.InvoiceDtos;
using FluentValidation;

namespace Clinic.API.API.Validators.InvoiceValidators
{
    public class CreateInvoiceDtoValidator : AbstractValidator<CreateInvoiceDto>
    {
        public CreateInvoiceDtoValidator()
        {
            RuleFor(x => x.PatientId)
                .NotEmpty().WithMessage("Patient ID is required.");

            RuleFor(x => x.AppointmentId)
                .NotEmpty().WithMessage("Appointment ID is required.");

            RuleFor(x => x.Items)
                .NotEmpty().WithMessage("Invoice must have at least one line item.");

            RuleForEach(x => x.Items).SetValidator(new CreateInvoiceItemDtoValidator());
        }
    }

   

   
}
