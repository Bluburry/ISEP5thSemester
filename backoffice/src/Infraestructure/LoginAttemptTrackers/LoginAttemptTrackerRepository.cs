using DDDSample1.Domain.HospitalStaff;
using DDDSample1.Domain.LoginAttemptTrackers;
using DDDSample1.Domain.Specializations;
using DDDSample1.Domain.Users;
using DDDSample1.Infrastructure.Shared;

namespace DDDSample1.Infrastructure.LoginAttemptTrackers
{
    public class LoginAttemptTrackerRepository : BaseRepository<LoginAttemptTracker, Username>, ILoginAttemptTrackerRepository
    {
    
        public LoginAttemptTrackerRepository(HospitalDbContext context):base(context.loginAttemps)
        {
           
        }


    }
}