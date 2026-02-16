using ContactManagerApp.Models;
using Microsoft.EntityFrameworkCore;

namespace ContactManagerApp.Data
{
	public class ContactDbContext : DbContext
	{
		public ContactDbContext(DbContextOptions<ContactDbContext> options) : base(options) { }

		protected override void OnModelCreating(ModelBuilder modelBuilder) 
		{ 
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<Contact>(
				ec =>
				{
					ec.HasKey(c => c.Id);
					ec.Property(c => c.Name).IsRequired().HasColumnType("varchar(100)");
					ec.Property(c => c.Phone).IsRequired().HasColumnType("varchar(20)");
					ec.Property(c => c.Salary).IsRequired().HasColumnType("decimal(19,4)");
				});

		}

		public DbSet<Contact> Contacts { get; set; }
	}
}
