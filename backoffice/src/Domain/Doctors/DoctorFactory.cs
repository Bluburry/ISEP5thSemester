using System;
using DDDSample1.Domain.Doctors;
using DDDSample1.Domain.HospitalStaff;

namespace DDDSample1.Domain.Doctors
{
    public class DoctorFactory
    {
        public Doctor CreateDoctor(Staff staff)
        {
            if (staff == null)
            {
                throw new ArgumentException("Specialization is required for creating a doctor.");
            }

            var doctor = new Doctor(staff)
            {
                Staff = staff,
            };

            return doctor;
        }
    }
}
