using System;
using System.Threading.Tasks;
using Moq;
using Xunit;
using DDDSample1.Domain.HospitalPatient;
using DDDSample1.Domain.Tokens;
using DDDSample1.DTO;
using DDDSample1.Domain.ValueObjects;
using DDDSample1.Domain.HospitalAppointment;
using DDDSample1.Domain.Shared;
using DDDSample1.Domain.Users;
using DDDSample1.Domain.ContactInformations;
using Humanizer;

namespace DDDNetCore.test.ServiceTest
{
    public class PatientServiceTest
    {
        private readonly Mock<IPatientRepository> _mockPatientRepo;
        private readonly Mock<IAppointmentRepository> _mockAppRepo;
        private readonly Mock<TokenService> _mockTokenService;
        private readonly Mock<AppointmentService> _mockApptService;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly PatientService _service;


        public PatientServiceTest()
        {
            _mockPatientRepo = new Mock<IPatientRepository>();
            _mockAppRepo = new Mock<IAppointmentRepository>();
            _mockTokenService = new Mock<TokenService>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockApptService = new Mock<AppointmentService>();

            _service = new PatientService(
                _mockUnitOfWork.Object,
                _mockPatientRepo.Object,
                null, // User repository not needed here
                _mockAppRepo.Object,
                _mockTokenService.Object,
                _mockApptService.Object
            );
        }

        [Fact]
        public async Task EditPatientProfilePatient_ThrowsException_WhenPatientNotFound()
        {
            // Arrange
            var tokenDto = new TokenDto("user123", "PATIENT_AUTH_TOKEN", DateTime.Now.ToString(), "");
            _mockPatientRepo
                .Setup(r => r.GetByUserIdAsync(It.IsAny<Username>()))
                .ReturnsAsync((Patient)null);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _service.EditPatientProfilePatient(new EditPatientDto_Patient(), tokenDto));
        }

        [Fact]
        public async Task EditPatientProfilePatient_SavesChanges_WhenDataIsUpdated()
        {
            // Arrange
            var tokenDto = new TokenDto("user123", "PATIENT_AUTH_TOKEN", DateTime.Now.ToString(), "user123@domain.com");
            var patient = new Patient();

            // Instantiate value objects
            var firstName = new FirstName("John");
            var lastName = new LastName("Doe");
            var fullName = new FullName("John Doe");
            var gender = Gender.MALE;
            var dateOfBirth = new DateOfBirth("1990-01-01");

            // Contact Information
            var email = new EmailAddress("johndoe@example.com");
            var phone = new PhoneNumber("1234567890");
            var contactInfo = new ContactInformation(email, phone);

            // Emergency Contact
            var emergencyContact = new PhoneNumber("0987654321");

            // Medical Conditions
            string clinicalDetails = "Hypertension";

            var userId = new Username("deactivateduser@example.com");
            var user = new User("testuser@example.com", "Test@123", UserRole.PATIENT);


            // Create Patient instance
            patient = new Patient(new MedicalRecordNumber("MRN123"),firstName,lastName,fullName,gender,dateOfBirth,contactInfo,emergencyContact, new List<Appointment>(), user);

            _mockPatientRepo.Setup(r => r.GetByUserIdAsync(It.IsAny<Username>()))
                            .ReturnsAsync(patient);

            Patient newPatient = patient;
            newPatient.firstName = new FirstName("NewFirst");


            _mockPatientRepo.Setup(r => r.Update(It.IsAny<Patient>()))
                            .Returns(newPatient);


            _mockUnitOfWork.Setup(u => u.CommitAsync());

            var editDto = new EditPatientDto_Patient
            {
                FirstName = "NewFirst",
            };

            // Act
            var result = await _service.EditPatientProfilePatient(editDto, tokenDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("NewFirst", result.firstName);
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Never);
        }

        [Fact]
        public async Task EditPatientProfilePatient_SendsEmailNotification_WhenEmailChanged()
        {
            // Arrange
            var tokenDto = new TokenDto("user123", "PATIENT_AUTH_TOKEN", DateTime.Now.ToString(), "user123@domain.com");
            var patient = new Patient();

            // Instantiate value objects
            var firstName = new FirstName("John");
            var lastName = new LastName("Doe");
            var fullName = new FullName("John Doe");
            var gender = Gender.MALE;
            var dateOfBirth = new DateOfBirth("1990-01-01");

            // Contact Information
            var email = new EmailAddress("johndoe@example.com");
            var phone = new PhoneNumber("1234567890");
            var contactInfo = new ContactInformation(email, phone);

            // Emergency Contact
            var emergencyContact = new PhoneNumber("0987654321");

            // Medical Conditions
            string clinicalDetails = "Hypertension";

            var userId = new Username("deactivateduser@example.com");
            var user = new User("testuser@example.com", "Test@123", UserRole.PATIENT);


            // Create Patient instance
            patient = new Patient(new MedicalRecordNumber("MRN123"), firstName, lastName, fullName, gender, dateOfBirth, contactInfo, emergencyContact, new List<Appointment>(), user);

            _mockPatientRepo.Setup(r => r.GetByUserIdAsync(It.IsAny<Username>()))
                            .ReturnsAsync(patient);

            Patient newPatient = patient;
            newPatient.firstName = new FirstName("NewFirst");


            _mockPatientRepo.Setup(r => r.Update(It.IsAny<Patient>()))
                            .Returns(newPatient);

            _mockTokenService.Setup(t => t.GenerateUpdateConfirmationToken(It.IsAny<string>()))
                    .ReturnsAsync(new TokenDto());


            _mockUnitOfWork.Setup(u => u.CommitAsync());

            var editDto = new EditPatientDto_Patient
            {
                FirstName = "NewFirst",
                Email = "newMail@domain.com"
            };

            // Act
            await _service.EditPatientProfilePatient(editDto, tokenDto);


            // Assert
            _mockPatientRepo.Verify(r => r.Update(It.IsAny<Patient>()), Times.Never);
            _mockTokenService.Verify(r => r.GenerateUpdateConfirmationToken(It.IsAny<string>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Never);
        }


        [Fact]
        public async Task RegisterPatient_Registers_WithValidPatient()
        {

            var examplePatient = new PatientRegistrationDto
            {
                firstName = "John",
                lastName = "Doe",
                fullName = "John Doe",
                gender = "MALE",
                dateOfBirth = "1990-08-15",
                email = "johndoe@example.com",
                phone = "+987654321",
                emergencyContact = "+987654321"
            };
            PatientBuilder builder = new PatientBuilder();
            Patient patient =
            builder.WithFirstName(examplePatient.firstName)
                .WithLastName(examplePatient.lastName)
                .WithFullName(examplePatient.fullName)
                .WithGender(examplePatient.gender)
                .WithDateOfBirth(examplePatient.dateOfBirth)
            .WithContactInformation(examplePatient.phone, examplePatient.email)
                .WithEmergencyContactNumber(examplePatient.emergencyContact)
                .WithMedicalRecordNumber()
                .Build();

            

            _mockPatientRepo.Setup(r => r.AddAsync(It.IsAny<Patient>()))
                            .ReturnsAsync(patient);

            PatientDto dto = await _service.registerPatient(examplePatient);
            dto.mrn = patient.Id.AsString();
            Assert.Equal(dto.ToString(), patient.toDto().ToString());
            _mockPatientRepo.Verify(s => s.AddAsync(It.IsAny<Patient>()), Times.Once);
            _mockUnitOfWork.Verify(s => s.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task CheckPatientExists_Success_WithPatientExists()
        {
            // Arrange
            var tokenDto = new TokenDto("user123", "PATIENT_AUTH_TOKEN", DateTime.Now.ToString(), "user123@domain.com");
            var patient = new Patient();

            // Instantiate value objects
            var firstName = new FirstName("John");
            var lastName = new LastName("Doe");
            var fullName = new FullName("John Doe");
            var gender = Gender.MALE;
            var dateOfBirth = new DateOfBirth("1990-01-01");

            // Contact Information
            var email = new EmailAddress("johndoe@example.com");
            var phone = new PhoneNumber("1234567890");
            var contactInfo = new ContactInformation(email, phone);

            // Emergency Contact
            var emergencyContact = new PhoneNumber("0987654321");

            // Medical Conditions
            string clinicalDetails = "Hypertension";

            var userId = new Username("deactivateduser@example.com");
            var user = new User("testuser@example.com", "Test@123", UserRole.PATIENT);


            // Create Patient instance
            patient = new Patient(new MedicalRecordNumber("MRN123"), firstName, lastName, fullName, gender, dateOfBirth, contactInfo, emergencyContact, new List<Appointment>(), user);

            _mockPatientRepo.Setup(s => s.GetByEmailAsync(It.IsAny<EmailAddress>()))
                .ReturnsAsync(patient);

            var result = _service.checkIfPatientProfileExists(patient.ContactInformation.Email.ToString());

            Assert.NotNull(result);

        }

    }
}
