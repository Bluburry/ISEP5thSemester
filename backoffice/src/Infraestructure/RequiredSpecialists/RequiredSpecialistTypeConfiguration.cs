using DDDSample1.Domain.RequiredSpecialists;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DDDSample1.Infrastructure.RequiredSpecialists
{
	internal class RequiredSpecialistEntityTypeConfiguration : IEntityTypeConfiguration<RequiredSpecialist>
	{
		public void Configure(EntityTypeBuilder<RequiredSpecialist> builder)
		{
			builder.HasKey(rs => rs.Id);

			/*builder.HasOne(rs => rs.OperationType)
			.WithMany()
			.IsRequired();*/

			//builder.HasOne(rs => rs.Specialization).WithMany().HasForeignKey(rs => rs.SpecializationName).IsRequired();
			builder.OwnsOne(rs => rs.SpecialistCount);
		}
	}
}