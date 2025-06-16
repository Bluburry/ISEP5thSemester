using DDDSample1.Domain.HospitalStaff;
using DDDSample1.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal class StaffEntityTypeConfiguration : IEntityTypeConfiguration<Staff>
{
    public void Configure(EntityTypeBuilder<Staff> builder)
    {
        builder.HasKey(s => s.Id);

        builder.HasOne(s => s.TheUser)
                .WithOne()
                .HasForeignKey<Staff>(s=>s.userId)
                .OnDelete(DeleteBehavior.Restrict);

        //builder.ComplexProperty(s => s.ContactInformation);
    

        builder.HasOne(s => s.theSpecialization)
            .WithMany()
            .HasForeignKey(sp => sp.specializationId)
            .IsRequired();

        builder.HasOne(s=>s.ContactInformation)
                .WithOne()
                .HasForeignKey<Staff>(s=>s.ContactInformationId);
    
        builder.OwnsOne(s => s.FullName);
        builder.OwnsOne(s => s.FirstName);
        builder.OwnsOne(s => s.LastName);

    }
}
