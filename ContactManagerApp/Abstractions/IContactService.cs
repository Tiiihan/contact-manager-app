using ContactManagerApp.Dto;
using ContactManagerApp.Models;

namespace ContactManagerApp.Abstractions
{
	public interface IContactService
	{
		Task AddContactsAsync(IEnumerable<ContactDto> contacts);
		Task<bool> DeleteContactAsync(Guid contactId);
		Task<List<Contact>> GetAllContactsAsync();
		Task<bool> UpdateContactAsync(ContactDto request);
	}
}
