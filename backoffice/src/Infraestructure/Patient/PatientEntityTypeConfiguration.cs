using DDDSample1.Domain.ContactInformations;
using DDDSample1.Domain.HospitalPatient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal class PatientEntityTypeConfiguration : IEntityTypeConfiguration<Patient>
{
    public void Configure(EntityTypeBuilder<Patient> builder)
    {
        builder.HasKey(p => p.Id);
        
        builder.HasOne(p => p.TheUser)
                .WithOne()
                .HasForeignKey<Patient>(s=>s.userId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

        builder
            .HasOne(p => p.ContactInformation)
            .WithOne() // or .WithMany() depending on your model
            .HasForeignKey<Patient>(c => c.ContactInformationId); // adjust the foreign key as needed


        //builder.OwnsMany(p => p.appointmentHistory);

        /*builder.HasOne(p=>p.ContactInformation)
                .WithOne()
                .HasForeignKey<Patient>(p=>p.ContactInformationId);*/

        builder.OwnsOne(p => p.dateOfBirth);
        builder.OwnsOne(p => p.fullName);
        builder.OwnsOne(p => p.firstName);
        builder.OwnsOne(p => p.lastName);
        builder.OwnsOne(p => p.emergencyContact);
    }
}
