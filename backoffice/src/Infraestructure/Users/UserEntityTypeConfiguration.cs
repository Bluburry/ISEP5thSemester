using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DDDSample1.Domain.Users;
using DDDSample1.Domain.HospitalStaff;

namespace DDDSample1.Infrastructure.Users
{
    internal class UserEntityTypeConfiguration : IEntityTypeConfiguration<User>
    {

        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(u => u.Id);

            /*builder.HasOne(s => s.associatedStaff)
                .WithOne(u => u.TheUser)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);*/

            builder.OwnsOne(u => u.Password);
            builder.OwnsOne(u => u.EmailAddress);
            
        
        }

    }
}