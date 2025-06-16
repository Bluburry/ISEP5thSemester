using DDDSample1.Domain.HospitalStaff;
using DDDSample1.Domain.LoginAttemptTrackers;
using DDDSample1.Domain.Specializations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DDDSample1.Infrastructure.LoginAttemptTrackers
{
    internal class LoginAttemptTrackerEntityTypeConfiguration : IEntityTypeConfiguration<LoginAttemptTracker>
    {

        public void Configure(EntityTypeBuilder<LoginAttemptTracker> builder)
        {
            builder.HasKey(l => l.Id);

            builder.OwnsOne(l => l.AttemptCounter);
            
            

        
        }
    }
}