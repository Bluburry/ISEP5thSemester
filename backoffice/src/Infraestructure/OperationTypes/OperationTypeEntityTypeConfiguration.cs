using DDDSample1.Domain.OperationTypes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Org.BouncyCastle.Asn1.Esf;

internal class OperationTypeEntityTypeConfiguration : IEntityTypeConfiguration<OperationType>
{
	public void Configure(EntityTypeBuilder<OperationType> builder)
	{
		builder.HasKey(ot =>  ot.Id );
		// https://learn.microsoft.com/en-us/ef/core/modeling/indexes?tabs=data-annotations#index-uniqueness
		//builder.HasIndex(ot => new { ot.OperationTypeName, ot.VersionNumber }).IsUnique();

	
		// builder.HasOne(o => o.RequiredSpecialists).WithMany().HasForeignKey(o => o.Id).IsRequired();
		

		builder.OwnsOne(ot => ot.OperationTypeName);
		builder.OwnsOne(ot => ot.EstimatedDuration);
		builder.OwnsOne(ot => ot.OperationTypeEndDate);
		builder.OwnsOne(ot => ot.OperationTypeStartDate);
		// builder.OwnsOne(ot => ot.VersionNumber);
	}
}