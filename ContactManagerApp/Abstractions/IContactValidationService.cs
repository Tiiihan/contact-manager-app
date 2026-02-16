using ContactManagerApp.Dto;

namespace ContactManagerApp.Abstractions
{
	public interface IContactValidationService
	{
		Task<List<string>> ValidateAsync(IEnumerable<ContactDto> contacts);
	}
}
