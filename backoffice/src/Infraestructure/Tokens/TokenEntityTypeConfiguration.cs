using DDDSample1.Domain.HospitalStaff;
using DDDSample1.Domain.Tokens;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DDDSample1.Infrastructure.Tokens
{
    internal class TokenEntityTypeConfiguration : IEntityTypeConfiguration<Token>
    {

        public void Configure(EntityTypeBuilder<Token> builder)
        {
            builder.HasKey(b => b.Id);



            builder.HasOne(s => s.TheUser)
                    .WithMany()
                    .HasForeignKey(s=>s.UserId)
                    .OnDelete(DeleteBehavior.Restrict);


            builder.OwnsOne(t => t.ExpirationDate);
           

        
        }
    }
}