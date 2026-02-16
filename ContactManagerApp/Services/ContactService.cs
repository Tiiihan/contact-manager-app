using ContactManagerApp.Abstractions;
using ContactManagerApp.Data;
using ContactManagerApp.Dto;
using ContactManagerApp.Models;
using Microsoft.EntityFrameworkCore;

namespace ContactManagerApp.Services
{
	public class ContactService : IContactService
	{
		private readonly ContactDbContext _context;

		public ContactService(ContactDbContext context)
		{
			_context = context;
		}

		public async Task AddContactsAsync(IEnumerable<ContactDto> contacts)
		{
			var mappedContacts = contacts.Select(x => new Contact
			{
				Name = x.Name,
				DateOfBirth = x.DateOfBirth,
				Phone = x.Phone,
				Salary = x.Salary,
				Married = x.Married
			}).ToList();

			_context.Contacts.AddRange(mappedContacts);
			await _context.SaveChangesAsync();
		}

		public async Task<bool> DeleteContactAsync(Guid contactId)
		{
			var contact = await _context.Contacts.FindAsync(contactId);

			if (contact == null)
				return false;

			_context.Remove(contact);
			await _context.SaveChangesAsync();

			return true;
		}

		public async Task<List<Contact>> GetAllContactsAsync()
		{
			return await _context.Contacts.AsNoTracking().ToListAsync();
		}

		public async Task<bool> UpdateContactAsync(ContactDto request) {
			var contact = await _context.Contacts.FindAsync(request.Id);

			if (contact == null)
				return false;

			contact.Name = request.Name;
			contact.DateOfBirth = request.DateOfBirth;
			contact.Phone = request.Phone;
			contact.Salary = request.Salary;
			contact.Married = request.Married;

			await _context.SaveChangesAsync();
			return true;
		}
	}
}