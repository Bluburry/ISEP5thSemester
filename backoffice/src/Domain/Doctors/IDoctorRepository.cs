using System.Threading.Tasks;
using DDDSample1.Domain.HospitalStaff;
using DDDSample1.Domain.Shared;

namespace DDDSample1.Domain.Doctors
{
    public interface IDoctorRepository : IRepository<Doctor, DoctorId>
    {
		Task<Doctor> GetDoctorByLicenseNumber(LicenseNumber license);

    }
}
