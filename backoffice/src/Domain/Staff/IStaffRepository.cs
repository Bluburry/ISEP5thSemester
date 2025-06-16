using System.Collections.Generic;
using System.Threading.Tasks;
using DDDSample1.Domain.ContactInformations;
using DDDSample1.Domain.Shared;
using DDDSample1.Domain.Users;
using DDDSample1.DTO;

namespace DDDSample1.Domain.HospitalStaff
{
    public interface IStaffRepository : IRepository<Staff, LicenseNumber>
    {

        Task<Staff> GetStaffByContact(ContactInformation contact);

        IEnumerable<Staff> GetStaffList();

        Task<Staff> GetStaffByLicense(LicenseNumber num);
        Task<Staff> GetStaffByUser(Username name);
        
        Task<Staff> GetTemporaryVersion(Username name);

        IEnumerable<Staff> GetFilteredStaff(QueryDataDto queryData);

    }
}
