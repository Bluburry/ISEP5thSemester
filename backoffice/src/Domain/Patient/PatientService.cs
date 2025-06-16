using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppServices;
using DDDSample1.Domain.HospitalAppointment;
using DDDSample1.Domain.Shared;
using DDDSample1.Domain.Tokens;
using DDDSample1.Domain.Users;
using DDDSample1.Domain.ValueObjects;
using DDDSample1.DTO;

namespace DDDSample1.Domain.HospitalPatient
{
    public class PatientService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPatientRepository _repo;
        private readonly IUserRepository _userRepo;
        private readonly IAppointmentRepository _appRepo;
        private readonly TokenService _tokenService;
        private readonly AppointmentService _appSvc;

        public PatientService(IUnitOfWork unitOfWork, IPatientRepository repo, IUserRepository userRepo, IAppointmentRepository appRepo, TokenService tokenService, AppointmentService appSvc)
        {
            this._unitOfWork = unitOfWork;
            this._repo = repo;
            this._userRepo = userRepo;
            this._appRepo = appRepo;
            this._tokenService = tokenService;
            this._appSvc = appSvc;
        }

        public PatientService() { }

        public async Task<List<Patient>> GetAllAsync()
        {
            var list = await this._repo.GetAllAsync();

            return list;
        }

        public virtual async Task<Patient> GetPatientByIdAsync(string id)
        {
            var patient = await this._repo.GetByIdAsync(new MedicalRecordNumber(id));

            return patient == null ? null : patient;
        }

        public virtual async Task<PatientDto> GetByIdAsync(string id)
        {
            var patient = await this._repo.GetByIdAsync(new MedicalRecordNumber(id));

            return patient == null ? null : patient.toDto();
        }

        public async virtual Task<PatientDto> GetByUserIdAsync(string id)
        {
            var patient = await this._repo.GetByUserIdAsync(new Username(id));

            return patient == null ? null : patient.toDto();
        }

        public async Task<bool> DeletePatientProfile(MedicalRecordNumber id)
        {
            var patient = await this._repo.GetByIdAsync(id);

            this._repo.Remove(patient);
            await this._unitOfWork.CommitAsync();

            return true;
        }

        public virtual async Task<PatientDto> checkIfPatientProfileExists(string email)
        {
            var patient = await _repo.GetByEmailAsync(new EmailAddress(email));
            if (patient == null)
            {
                return null;
            }
            else
            {
                return patient.toDto();
            }
        }

        public virtual async void addUserToPatientProfile(string patientMRN, string userId)
        {
            var user = await _userRepo.GetByIdAsync(new Username(userId));
            var patient = await _repo.GetByIdAsync(new MedicalRecordNumber(patientMRN));
            patient.AddUser(user.Id, user);
            _repo.Update(patient);
            await _unitOfWork.CommitAsync();
        }

        public async Task<PatientDto> registerPatient(PatientRegistrationDto dto)
        {
            PatientBuilder builder = new PatientBuilder();

            Patient patient =
            builder.WithFirstName(dto.firstName)
                .WithLastName(dto.lastName)
                .WithFullName(dto.fullName)
                .WithGender(dto.gender)
                .WithDateOfBirth(dto.dateOfBirth)
                .WithContactInformation(dto.phone, dto.email)
                .WithEmergencyContactNumber(dto.emergencyContact)
                .WithMedicalRecordNumber()
                .Build();

            if (patient == null)
            {
                PatientDto errorPatient = new PatientDto();

                errorPatient.email = "Patient is null";
                return errorPatient;
            }

            await this._repo.AddAsync(patient);
            await this._unitOfWork.CommitAsync();

            return patient.toDto();
        }

        public IEnumerable<PatientDto> GetFilteredPatients(QueryDataDto queryData)
        {
            IEnumerable<Patient> patients = _repo.GetFilteredPatients(queryData);

            return patients.Select(patient => patient.toDto()).ToList();
        }

        public virtual async Task<PatientDto> EditPatientProfileAdmin(EditPatientDto_Admin editData)
        {
            Patient patient = await _repo.GetByIdAsync(new MedicalRecordNumber(editData.patientId));

            // Update fields if they are not null
            if (!string.IsNullOrEmpty(editData.FirstName))
            {
                patient.firstName = new FirstName(editData.FirstName);
            }

            if (!string.IsNullOrEmpty(editData.LastName))
            {
                patient.lastName = new LastName(editData.LastName);
            }

            if (!string.IsNullOrEmpty(editData.Fullname))
            {
                patient.fullName = new FullName(editData.Fullname);
            }

            if (!string.IsNullOrEmpty(editData.Email))
            {
                patient.ContactInformation.Email = new EmailAddress(editData.Email);
            }

            if (!string.IsNullOrEmpty(editData.Phone))
            {
                patient.ContactInformation.Phone = new PhoneNumber(editData.Phone);
            }

            if (editData.MedicalHistory != null)
            {
                Appointment appointment = await this._appRepo.GetByIdAsync(new AppointmentID(editData.MedicalHistory.id));
                if (appointment == null)
                {
                    await this._appSvc.registerAppointment(editData.MedicalHistory);
                }
                else
                {
                    await this._appSvc.updateAppointment(editData.MedicalHistory, appointment);
                }
            }

            if (!string.IsNullOrEmpty(editData.DateOfBirth))
            {
                patient.dateOfBirth = new DateOfBirth(editData.DateOfBirth);
            }

            // Save the updated patient back to the repository
            Patient retPatient = _repo.Update(patient);
            await _unitOfWork.CommitAsync();

            // Return the updated patient as a DTO
            return retPatient.toDto();
        }

        public virtual async Task<PatientDto> EditPatientProfilePatient(EditPatientDto_Patient editData, TokenDto tokenDto)
        {
            Patient patient = await _repo.GetByUserIdAsync(new Username(tokenDto.UserId));

            if (patient == null)
            {
                throw new ArgumentNullException(nameof(patient), "Patient associated with User not found.");
            }

            if (patient.ContactInformation.Email.ToString() != editData.Email)
            {
                PatientDto patientDto = new PatientDto(patient.Id.AsString(), patient.userId.AsString(), editData);
                TokenDto confirmationToken = await this._tokenService.GenerateUpdateConfirmationToken(tokenDto.UserId);
                EmailService.sendPatientUpdateNotification(tokenDto.UserId, confirmationToken, patientDto);
                return patientDto;
            }

            // Update fields if they are not null
            if (patient.firstName.ToString() != editData.FirstName)
            {
                patient.firstName = new FirstName(editData.FirstName);
            }

            if (patient.lastName.ToString() != editData.LastName)
            {
                patient.lastName = new LastName(editData.LastName);
            }

            if (patient.fullName.ToString() != editData.Fullname)
            {
                patient.fullName = new FullName(editData.Fullname);
            }

            if (patient.ContactInformation.Email.ToString() != editData.Email)
            {
                patient.ContactInformation.Email = new EmailAddress(editData.Email);
            }

            if (patient.ContactInformation.Phone.ToString() != editData.Phone)
            {
                patient.ContactInformation.Phone = new PhoneNumber(editData.Phone);
            }

            if (editData.MedicalHistory != null && editData.MedicalHistory.id != null)
            {
                Appointment appointment = await this._appRepo.GetByIdAsync(new AppointmentID(editData.MedicalHistory.id));
                if (appointment == null)
                {
                    await this._appSvc.registerAppointment(editData.MedicalHistory);
                }
                else
                {
                    await this._appSvc.updateAppointment(editData.MedicalHistory, appointment);
                }
            }

            if (patient.dateOfBirth.ToString() != editData.DateOfBirth)
            {
                patient.dateOfBirth = new DateOfBirth(editData.DateOfBirth);
            }


            if (patient.gender.ToString() != editData.Gender)
            {
                if (editData.Gender.Equals("MALE")) { patient.gender = Gender.MALE; }
                else if (editData.Gender.Equals("FEMALE")) { patient.gender = Gender.FEMALE; }
                else if (editData.Gender.Equals("OTHER")) { patient.gender = Gender.OTHER; }
                else if (editData.Gender.Equals("NONSPECIFIED")) { patient.gender = Gender.NONSPECIFIED; }
                else { throw new ArgumentException("Gender must be valid."); }
            }

            if (patient.emergencyContact.ToString() != editData.EmergencyContact)
            {
                patient.emergencyContact = new PhoneNumber(editData.EmergencyContact);
            }

            // Save the updated patient back to the repository
            Patient retPatient = _repo.Update(patient);
            await _unitOfWork.CommitAsync();

            // Return the updated patient as a DTO
            return retPatient.toDto();
        }

    }
}
