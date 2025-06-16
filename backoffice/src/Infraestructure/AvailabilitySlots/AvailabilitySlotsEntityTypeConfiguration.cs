using DDDSample1.Domain.AvailabilitySlots;
using DDDSample1.Domain.HospitalStaff;
using DDDSample1.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DDDSample1.Infrastructure.AvailabilitySlots{
    internal class AvailabilitySlotsEntityTypeConfiguration : IEntityTypeConfiguration<AvailabilitySlot>
    {
        public void Configure(EntityTypeBuilder<AvailabilitySlot> builder)
        {

            builder.HasKey(s => s.Id);

            builder.HasOne(s => s.theStaff)
            .WithMany()
            .HasForeignKey(s=>s.StaffId)
            .IsRequired(false);

            builder.HasOne(s => s.opRoom)
            .WithMany()
            .HasForeignKey(s=>s.roomId)
            .IsRequired(false);
            
        }
    }
}
