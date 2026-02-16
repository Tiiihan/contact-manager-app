using ContactManagerApp.Abstractions;
using ContactManagerApp.Dto;
using ContactManagerApp.Models;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ContactManagerApp.Controllers
{
	public class HomeController : Controller
	{
		private readonly ICsvService _csvService;
		private readonly IContactService _contactService;
		private readonly IContactValidationService _validationService;

		public HomeController(ICsvService csvService, IContactService contactService, IContactValidationService validationService)
		{
			_csvService = csvService;
			_contactService = contactService;
			_validationService = validationService;
		}

		public async Task<IActionResult> Index()
		{
			var contacts = await _contactService.GetAllContactsAsync();

			return View(contacts);
		}

		[HttpPost]
		public async Task<IActionResult> Upload(IFormFile postedFile)
		{
			if (postedFile == null || postedFile.Length == 0)
				return BadRequest(new { success = false, message = "File is not uploaded"});

			if (!_csvService.CheckFileExtension(postedFile))
				return BadRequest(new { success = false, message = "Invalid file extension, choose .csv file" });

			var dataFromCsv = await _csvService.GetDataFromFileAsync(postedFile);

			if (!dataFromCsv.Any())
				return BadRequest(new { success = false, message = "File contains no data" });

			var errors = await _validationService.ValidateAsync(dataFromCsv);

			if (errors.Any())
			{
				return BadRequest(new { success = false, message = "Validation failed", errors = errors });
			}

			await _contactService.AddContactsAsync(dataFromCsv);

			return Ok(new { success = true, message = "Data uploaded successfully"});
		}

		[HttpDelete]
		public async Task<IActionResult> Delete([FromQuery] Guid contactId)
		{
			if (!await _contactService.DeleteContactAsync(contactId))
				return NotFound(new { success = false, message = $"Contact with id: {contactId} not found"});

			return Ok();
		}

		[HttpPut]
		public async Task<IActionResult> Update([FromBody] ContactDto request)
		{
			if (!ModelState.IsValid)
			{
				var errors = ModelState.Where(x => x.Value.Errors.Count > 0)
										.SelectMany(x => x.Value.Errors)
										.Select(x => x.ErrorMessage)
										.ToList();

				return BadRequest(new { success = false, message = "Validation failed", errors = errors });
			}

			if (request.Id == Guid.Empty)
				return BadRequest(new { success = false, message = "Id is required" });

			if (!await _contactService.UpdateContactAsync(request))
				return NotFound(new { success = false, message = $"Contact with id: {request.Id} not found" });

			return Ok();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}