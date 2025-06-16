using DDDSample1.Domain.Doctors;
using DDDSample1.Domain.HospitalStaff;
using DDDSample1.Domain.Specializations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DDDSample1.Infrastructure.Doctors
{
    internal class DoctorEntityTypeConfiguration : IEntityTypeConfiguration<Doctor>
    {

        public void Configure(EntityTypeBuilder<Doctor> builder)
        {
            builder.HasKey(b => b.Id);
            /*builder
                .HasOne(s=>s.theStaff)
                .WithOne()
                .HasForeignKey<Doctor>(d=> d.staffId)
                .IsRequired();
            */
        }
    }
}