using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DDDSample1.Domain.Shared;
using DDDSample1.Domain.Users;
using DDDSample1.DTO;

namespace DDDSample1.Domain.HospitalPatient
{
    public interface IPatientRepository : IRepository<Patient, MedicalRecordNumber>
    {
        public IEnumerable<Patient> GetFilteredPatients(QueryDataDto queryData);
        
        new public Task<Patient> GetByIdAsync(MedicalRecordNumber id);

        public Task<Patient> GetByUserIdAsync(Username userId);

        public Task<Patient> GetByEmailAsync(EmailAddress emailAddress);
    }
}
