using ContactManagerApp.Abstractions;
using ContactManagerApp.Dto;
using ContactManagerApp.Exceptions;
using ContactManagerApp.Models;
using CsvHelper;
using CsvHelper.TypeConversion;
using System.Globalization;

namespace ContactManagerApp.Services
{
	public class CsvService : ICsvService
	{
		private const string FILE_EXTENSION = ".csv";

		public bool CheckFileExtension(IFormFile postedFile)
		{
			var extension = Path.GetExtension(postedFile.FileName);
			return string.Equals(extension, FILE_EXTENSION, StringComparison.OrdinalIgnoreCase);
		}

		public async Task<IReadOnlyList<ContactDto>> GetDataFromFileAsync(IFormFile postedFile)
		{
			using var reader = new StreamReader(postedFile.OpenReadStream());
			using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

			try
			{
				var records = new List<ContactDto>();
				await foreach (var record in csv.GetRecordsAsync<ContactDto>())
				{
					records.Add(record);
				}
				return records;
			}
			catch (HeaderValidationException ex)
			{
				var missingHeaders = string.Join(", ", ex.InvalidHeaders.SelectMany(h => h.Names));
				throw new CsvValidationException($"File is missing required columns: {missingHeaders}");
			}
			catch (TypeConverterException ex)
			{
				var row = ex.Context?.Parser?.Row ?? 0;
				var column = ex.MemberMapData?.Member?.Name ?? "Unknown column";
				var value = ex.Text;

				throw new CsvValidationException($"Error at row {row}: Value '{value}' is invalid for column '{column}'.");
			}
			catch (CsvHelperException)
			{
				throw new CsvValidationException("The CSV file structure is invalid or corrupted.");
			}
		}
	}
}