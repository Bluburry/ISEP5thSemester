using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DDDSample1.Domain.HospitalPatient;
using DDDSample1.Domain.Shared;
using DDDSample1.Domain.Users;
using DDDSample1.Domain.ValueObjects;
using DDDSample1.DTO;
using DDDSample1.Infrastructure.Shared;
using Microsoft.EntityFrameworkCore;


namespace DDDSample1.Infrastructure.HospitalPatient
{
    public class PatientRepository : BaseRepository<Patient, MedicalRecordNumber>, IPatientRepository
    {
        private readonly HospitalDbContext _context;
        public PatientRepository(HospitalDbContext context) : base(context.Patients)
        {
            _context = context;
        }

        public IEnumerable<Patient> GetFilteredPatients(QueryDataDto queryData)
        {
            // Fetch the patients and include related data
            var patients = _context.Patients
                .Include(p => p.ContactInformation)
                .Include(p => p.appointmentHistory)
                .Include(p => p.TheUser)
                .AsEnumerable(); // Force client-side evaluation here

            // Filter the patients based on the provided query data
            patients = patients.Where(p => !p.IsAnonymized());

            if (!string.IsNullOrEmpty(queryData.Name))
            {
                string queryName = queryData.Name.ToLower(); // Convert query to lowercase
                patients = patients.Where(p => 
                    p.fullName.ToString().ToLower().Contains(queryName)); // Convert fullName to lowercase and check contains
            }

            if (!string.IsNullOrEmpty(queryData.Email))
            {
                patients = patients.Where(p => 
                    p.ContactInformation.Email.ToString().ToLower().Contains(queryData.Email.ToLower()));
            }

            if (!string.IsNullOrEmpty(queryData.PhoneNumber))
            {
                patients = patients.Where(p => 
                    p.ContactInformation.Phone.ToString().ToLower().Contains(queryData.PhoneNumber.ToLower()));
            }

            if (!string.IsNullOrEmpty(queryData.MedicalRecordNumber))
            {
                patients = patients.Where(p => 
                    p.Id.AsString().ToLower().Contains(queryData.MedicalRecordNumber.ToLower()));
            }

            if (!string.IsNullOrEmpty(queryData.Gender))
            {
                if (Enum.TryParse(typeof(Gender), queryData.Gender.ToUpper(), out var gender))
                {
                    patients = patients.Where(p => 
                        p.gender.Equals((Gender)gender));
                }
            }

            if (!string.IsNullOrEmpty(queryData.DateOfBirth))
            {
                patients = patients.Where(p =>
                    p.dateOfBirth.ToString().ToLower().Contains(queryData.DateOfBirth.ToLower()));
            }

            return patients.ToList(); // Convert to a list before returning
        }


        public async Task<Patient> GetByIdAsync(MedicalRecordNumber id)
            {
                return await _context.Patients
                    .Include(p => p.ContactInformation)
                    .Include(p => p.appointmentHistory)
                    .Include(p => p.TheUser)
                    .FirstOrDefaultAsync(p => p.Id.Equals(id));
            }

        public async Task<Patient> GetByUserIdAsync(Username userId)
        {
            return await _context.Patients
                .Include(p => p.ContactInformation)
                .Include(p => p.appointmentHistory)
                .Include(p => p.TheUser)
                .FirstOrDefaultAsync(p => p.userId.Equals(userId));
        }

        public async Task<Patient> GetByEmailAsync(EmailAddress emailAddress)
        {
            return await _context.Patients
                .Include(p => p.ContactInformation)
                .Include(p => p.appointmentHistory)
                .Include(p => p.TheUser)
                .FirstOrDefaultAsync(p => p.ContactInformation.Email.Value == emailAddress.Value);
        }

    }
}