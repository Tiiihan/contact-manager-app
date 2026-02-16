using CsvHelper.Configuration.Attributes;

namespace ContactManagerApp.Dto
{
	public class ContactDto
	{
		[Ignore]
		public Guid Id { get; set; }

		[Name("Name")]
		public string Name { get; set; }

		[Name("Date of birth")]
		public DateTime DateOfBirth { get; set; }

		[Name("Phone")]
		public string Phone { get; set; }

		[Name("Salary")]
		public decimal Salary { get; set; }

		[Name("Married")]
		public bool Married { get; set; }
	}
}
