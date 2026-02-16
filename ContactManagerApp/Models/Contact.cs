namespace ContactManagerApp.Models
{
	public class Contact
	{
		public Guid Id { get; set; }

		public string Name { get; set; }

		public DateTime DateOfBirth { get; set; }

		public bool Married { get; set; }

		public string Phone { get; set; }

		public decimal Salary { get; set; }
	}
}
