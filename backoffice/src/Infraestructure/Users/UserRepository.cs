using DDDSample1.Domain.Users;
using DDDSample1.Infrastructure.Shared;

namespace DDDSample1.Infrastructure.Users
{
    public class UserRepository : BaseRepository<User, Username>, IUserRepository
    {
    
        public UserRepository(HospitalDbContext context):base(context.Users)
        {
           
        }


    }
}