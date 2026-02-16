using ContactManagerApp.Dto;
using ContactManagerApp.Models;
using FluentValidation;

namespace ContactManagerApp.Validation
{
	public class ContactValidator : AbstractValidator<ContactDto>
	{
		public ContactValidator() {
			RuleFor(x => x.Name)
				.NotEmpty().WithMessage("Name is required")
				.MaximumLength(100).WithMessage("Name max 100 characters");

			RuleFor(x => x.DateOfBirth)
				.NotEmpty().WithMessage("Date of birth is required")
				.LessThan(DateTime.Today).WithMessage("Date of birth cannot be in the future");

			RuleFor(x => x.Phone)
				.NotEmpty().WithMessage("Phone is required")
				.Matches(@"^(?:\+380\s?|0)\d{2}\s?\d{3}\s?\d{4,5}$").WithMessage("Invalid phone format. Use +380XX XXX XXXX or 0XX XXX XXXX");

			RuleFor(x => x.Salary)
				.GreaterThanOrEqualTo(0).WithMessage("Salary cannot be negative");
		}
	}
}