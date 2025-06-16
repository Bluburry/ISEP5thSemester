using System;
using System.Threading.Tasks;
using DDDSample1.Domain.Doctors;
using DDDSample1.Domain.HospitalStaff;
using DDDSample1.Domain.Shared;

namespace DDDSample1.Domain.Doctors
{
    public class DoctorService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDoctorRepository _doctorRepository;

        private readonly IStaffRepository _staffRepo;

        public DoctorService(IDoctorRepository doctorRepository, IUnitOfWork unitOfWork, IStaffRepository staffRepo)
        {
            _doctorRepository = doctorRepository;
            _unitOfWork = unitOfWork;
            _staffRepo = staffRepo;
        }

        public async Task<DoctorDto> RegisterDoctorAsync(StaffDto dto)
        {
            Staff assStaff = await _staffRepo.GetByIdAsync(new LicenseNumber(dto.LicenseNumber));
            if (assStaff == null)
            {
                throw new ArgumentException("Invalid doctor registration details.");
            }

            DoctorFactory factory = new DoctorFactory();

            Doctor doctor = await _doctorRepository.AddAsync(factory.CreateDoctor(assStaff));
            await _unitOfWork.CommitAsync();

            return doctor.ToDto();
        }
    }
}
