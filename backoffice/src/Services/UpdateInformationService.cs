using System;
using System.Threading.Tasks;
using DDDSample1.Domain.HospitalAppointment;
using DDDSample1.Domain.HospitalPatient;
using DDDSample1.Domain.Logs;
using DDDSample1.Domain.Shared;
using DDDSample1.Domain.Tokens;
using DDDSample1.Domain.Users;
using DDDSample1.Domain.ValueObjects;

namespace DDDSample1.AppServices
{
    public class UpdateInformationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRepository _userRepo;
        private readonly ITokenRepository _tokenRepo;
        private readonly ILogRepository _logRepo;
        private readonly IAppointmentRepository _appRepo;
        private readonly IPatientRepository _patRepo;
        private readonly AppointmentService _apptSvc;

        public UpdateInformationService(IUnitOfWork unitOfWork, IUserRepository userRepo, ITokenRepository tokenRepo, ILogRepository logRepository, IAppointmentRepository appRepo, IPatientRepository patRepo, AppointmentService apptSvc)
        {
            this._unitOfWork = unitOfWork;
            this._tokenRepo = tokenRepo;
            this._userRepo = userRepo;
            this._logRepo = logRepository;
            this._appRepo = appRepo;
            this._patRepo = patRepo;
            this._apptSvc = apptSvc;
        }


        public async Task<PatientDto> updatePatientInformation(string tokenId){

            var retrievedToken = await _tokenRepo.GetByIdAsync(new TokenId(tokenId));

            if (retrievedToken == null)
            {
                throw new Exception("Token not found" + tokenId);
            }

            if (retrievedToken.TokenValue != TokenType.UPDATE_CONFIRMATION)
            {
                throw new Exception("Token type does not match operation - " + retrievedToken.TokenValue + " - " + TokenType.UPDATE_CONFIRMATION);
            }

            var currentUser = await _userRepo.GetByIdAsync(retrievedToken.UserId);

            if (currentUser == null)
            {
                throw new Exception("User not found");
            }

            var log = await _logRepo.GetLastUpdateLogByLoggedIdAsync(retrievedToken.UserId.AsString());
            PatientDto patientDto = new PatientDto(log.LoggedInformation);

            if (patientDto!=null){
                Patient patient = await _patRepo.GetByIdAsync(new MedicalRecordNumber(patientDto.mrn));

                if (patient.firstName.ToString() != patientDto.firstName)
                {
                    patient.firstName = new FirstName(patientDto.firstName); 
                }

                if (patient.lastName.ToString() != patientDto.lastName)
                {
                    patient.lastName = new LastName(patientDto.lastName); 
                }

                if (patient.fullName.ToString() != patientDto.fullName)
                {
                    patient.fullName = new FullName(patientDto.fullName);
                }

                if (patient.ContactInformation.Email.ToString() != patientDto.email)
                {
                    patient.ContactInformation.Email = new EmailAddress(patientDto.email);
                }

                if (patient.ContactInformation.Phone.ToString() != patientDto.phone)
                {
                    patient.ContactInformation.Phone = new PhoneNumber(patientDto.phone);
                }

                if (!patientDto.appointmentHistory.Equals(patient.appointmentHistory))
                {
                    foreach (var apt in patientDto.appointmentHistory){
                    Appointment appointment = await this._appRepo.GetByIdAsync(new AppointmentID(apt.id));
                    if (appointment == null){
                        await this._apptSvc.registerAppointment(apt);
                    } else {
                        await this._apptSvc.updateAppointment(apt, appointment);
                    }}
                }

                if (patient.dateOfBirth.ToString() != patientDto.dateOfBirth)
                {
                    patient.dateOfBirth = new DateOfBirth(patientDto.dateOfBirth);
                }
                if (patient.gender.ToString() != patientDto.gender){
                    if (patientDto.gender == "MALE"){patient.gender = Gender.MALE;}
                    if (patientDto.gender == "FEMALE"){patient.gender = Gender.FEMALE;}
                    if (patientDto.gender == "OTHER"){patient.gender = Gender.OTHER;}
                    if (patientDto.gender == "NONSPECIFIED"){patient.gender = Gender.NONSPECIFIED;}
                }
                if (patient.emergencyContact.ToString() != patientDto.emergencyContact ){
                    patient.emergencyContact = new PhoneNumber(patientDto.emergencyContact);
                }

                // Save the updated patient back to the repository
                Patient retPatient = _patRepo.Update(patient);
                await _unitOfWork.CommitAsync();
                return retPatient.toDto();
            }
            throw new Exception("No Update Request Found for this User.");
        }
    }

}