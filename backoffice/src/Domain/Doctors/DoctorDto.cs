using System;
using DDDSample1.Domain.HospitalStaff;

namespace DDDSample1.Domain.Doctors
{
    public class DoctorDto
    {
        public LicenseNumber LicenseNumber { get; set; }
        public DoctorId DoctorId { get; set; }

        public DoctorDto(LicenseNumber licenseNumber, DoctorId doctorId)
        {
            LicenseNumber = licenseNumber;
            DoctorId = doctorId;
        }
    }
}
