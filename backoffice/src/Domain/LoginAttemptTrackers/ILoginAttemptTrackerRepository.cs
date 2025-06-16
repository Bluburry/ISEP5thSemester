using DDDSample1.Domain.LoginAttemptTrackers;
using DDDSample1.Domain.Shared;
using DDDSample1.Domain.Users;

namespace DDDSample1.Domain.HospitalStaff
{
    public interface ILoginAttemptTrackerRepository : IRepository<LoginAttemptTracker, Username>
    {

        
    }
}
