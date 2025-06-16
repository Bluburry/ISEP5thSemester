using DDDSample1.Domain.OperationPhases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DDDSample1.Infrastructure.OperationPhases
{
	internal class OperationPhaseEntityTypeConfiguration : IEntityTypeConfiguration<OperationPhase>
	{
		public void Configure(EntityTypeBuilder<OperationPhase> builder)
		{
			builder.HasKey(op => op.Id);

			//builder.HasOne(op => op.theOperationType)
				//.WithMany()
				//.HasPrincipalKey()
				//.IsRequired();
				
			// builder.OwnsOne(op=> op.PhaseDuration);
		}
	}
}