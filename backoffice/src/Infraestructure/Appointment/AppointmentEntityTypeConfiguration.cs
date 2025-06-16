using DDDSample1.Domain.HospitalAppointment;
using DDDSample1.Domain.HospitalPatient;
using DDDSample1.Domain.HospitalStaff;
using DDDSample1.Domain.OperationRooms;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal class AppointmentEntityTypeConfiguration : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        builder.HasKey(p => p.Id);
        
        builder.HasOne(p => p.patient);

        builder.HasMany(app => app.designedStaff);


        builder.OwnsOne(app => app.dateAndTime);

        builder.HasOne(app => app.OperationRoom)
        .WithMany()
        .HasForeignKey(app => app.OpRoomId)
        .IsRequired(false);
        //builder.HasOne(app => app.reason).WithOne().IsRequired(false);
        //builder.HasOne(app => app.notes).WithOne().IsRequired(false);
        //builder.HasOne(app => app.diagnosis).WithOne().IsRequired(false);
        // Remember to add Cirurgy Information
    }
}
