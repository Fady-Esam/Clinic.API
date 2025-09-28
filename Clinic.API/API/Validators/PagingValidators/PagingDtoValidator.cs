using Clinic.API.API.Dtos.PagingDtos;
using FluentValidation;

namespace Clinic.API.API.Validators.PagingValidators
{
    public class PagingDtoValidator : AbstractValidator<PagingDto>
    {
        public PagingDtoValidator()
        {
            RuleFor(x => x.Page)
                .GreaterThan(0).WithMessage("Page must be greater than 0."); // ✅ ensures no zero/negative page

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 100).WithMessage("PageSize must be between 1 and 100.");
            // ✅ avoid over-fetching / denial of service risk

            RuleFor(x => x.Search)
                .MaximumLength(200).WithMessage("Search query must not exceed 200 characters.")
                .When(x => !string.IsNullOrEmpty(x.Search)); // ✅ optional but safe
        }
    }
}
