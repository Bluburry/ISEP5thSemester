using DDDSample1.Domain.OperationRoomTypes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal class OperationRoomTypeEntityTypeConfiguration : IEntityTypeConfiguration<OperationRoomType>
{
    public void Configure(EntityTypeBuilder<OperationRoomType> builder)
    {
        builder.HasKey(p => p.Id);
        
        builder.OwnsOne(p => p.Name);
        builder.OwnsOne(p => p.description);
    }
}
