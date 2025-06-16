using DDDSample1.Domain.AssignedStaffs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DDDSample1.Infrastructure.AssignedStaffs
{
	internal class AssignedStaffEntityTypeConfiguration : IEntityTypeConfiguration<AssignedStaff>
	{
		public void Configure(EntityTypeBuilder<AssignedStaff> builder)
		{
			builder.HasKey(a => a.Id);
		}
	}
}