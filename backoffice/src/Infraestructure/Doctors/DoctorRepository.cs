using System.Linq;
using System.Threading.Tasks;
using DDDSample1.Domain.Doctors;
using DDDSample1.Domain.HospitalStaff;
using DDDSample1.Domain.Specializations;
using DDDSample1.Infrastructure.Shared;
using Microsoft.EntityFrameworkCore;

namespace DDDSample1.Infrastructure.Doctors
{
    public class DoctorRepository : BaseRepository<Doctor, DoctorId>, IDoctorRepository
    {
    
        HospitalDbContext _context;
        public DoctorRepository(HospitalDbContext context):base(context.Doctors)
        {  
           _context = context;
        }

		public async Task<Doctor> GetDoctorByLicenseNumber(LicenseNumber license)
		{
            IQueryable<Doctor> doctor = _context.Doctors
				.Where(d => d.Staff.Id.Equals(license))
                .Include(d => d.Staff)
                    .ThenInclude(s => s.theSpecialization);
                
            return await doctor.FirstOrDefaultAsync();
		}

		public async Task<Doctor> GetStaffByUser(DoctorId id)
        {
            IQueryable<Doctor> doctor = _context.Doctors.Where(d => d.Id.Equals(id))
                .Include(d => d.Staff)
                    .ThenInclude(s => s.theSpecialization);
                
            return await doctor.FirstOrDefaultAsync();
        }


    }
}