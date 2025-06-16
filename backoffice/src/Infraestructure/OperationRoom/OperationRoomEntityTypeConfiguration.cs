using DDDSample1.Domain.OperationRooms;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal class OperationRoomEntityTypeConfiguration : IEntityTypeConfiguration<OperationRoom>
{
    public void Configure(EntityTypeBuilder<OperationRoom> builder)
    {
        builder.HasKey(p => p.Id);
        
        builder.HasOne(p => p.OperationRoomType)
        .WithMany()
        .HasForeignKey(p => p.OperationRoomTypeId)
        .OnDelete(DeleteBehavior.Restrict)
        .IsRequired(true);

        
    }
}
