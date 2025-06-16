using DDDSample1.Domain.OperationRequests;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal class OperationRequestEntityTypeConfiguration : IEntityTypeConfiguration<OperationRequest>
{
	public void Configure(EntityTypeBuilder<OperationRequest> builder)
	{
		builder.HasKey(or => or.Id);

		//builder.HasOne(or => or.OperationType).WithMany().HasForeignKey(ot => ot.)
	}
}