using ContactManagerApp.Abstractions;
using ContactManagerApp.Dto;
using FluentValidation;

namespace ContactManagerApp.Services
{
	public class ContactValidationService : IContactValidationService
	{
		private readonly IValidator<ContactDto> _validator;

		public ContactValidationService(IValidator<ContactDto> validator)
		{
			_validator = validator;
		}

		public async Task<List<string>> ValidateAsync(IEnumerable<ContactDto> contacts) 
		{
			var errors = new List<string>();
			int row = 1;

			foreach (var contact in contacts)
			{
				var result = await _validator.ValidateAsync(contact);

				if (!result.IsValid)
				{
					errors.AddRange(result.Errors.Select(e => $"Row {row}: {e.ErrorMessage}"));
				}
				row++;
			}
			return errors;
		}
	}
}
