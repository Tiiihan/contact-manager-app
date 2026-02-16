
using ContactManagerApp.Dto;
using ContactManagerApp.Models;

namespace ContactManagerApp.Abstractions
{
	public interface ICsvService
	{
		bool CheckFileExtension(IFormFile postedFile);
		Task<IReadOnlyList<ContactDto>> GetDataFromFileAsync(IFormFile postedFile);
	}
}
