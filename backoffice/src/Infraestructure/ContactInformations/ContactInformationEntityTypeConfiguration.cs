using DDDSample1.Domain.ContactInformations;
using DDDSample1.Domain.HospitalStaff;
using DDDSample1.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DDDSample1.Infrastructure.ContactInformations
{
	internal class ContactInformationEntityTypeConfiguration : IEntityTypeConfiguration<ContactInformation>
	{
		public void Configure(EntityTypeBuilder<ContactInformation> builder)
		{
			/* builder.Property(ci => ci.Id)
				.HasColumnType("VARCHAR(550)")
				.IsRequired(); */

			builder.HasKey(s => s.Id);

			builder.OwnsOne(s => s.Phone);
			builder.OwnsOne(s => s.Email);
		}
	}
}
