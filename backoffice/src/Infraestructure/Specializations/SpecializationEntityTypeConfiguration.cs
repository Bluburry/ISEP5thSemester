using DDDSample1.Domain.HospitalStaff;
using DDDSample1.Domain.Specializations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DDDSample1.Infrastructure.Specializations
{
	internal class SpecializationEntityTypeConfiguration : IEntityTypeConfiguration<Specialization>
	{

		public void Configure(EntityTypeBuilder<Specialization> builder)
		{
			builder.Property(sp => sp.SpecializationName)
				.HasColumnType("VARCHAR(255)")
				.IsRequired();

			builder.Property(sp => sp.SpecializationDescription)
				.HasColumnType("MEDIUMTEXT");

			builder.HasKey(b => b.Id);
		}
	}
}