using DDDSample1.Controllers;
using DDDSample1.Domain.ContactInformations;
using DDDSample1.Domain.HospitalAppointment;
using DDDSample1.Domain.HospitalPatient;
using DDDSample1.Domain.Logs;
using DDDSample1.Domain.Shared;
using DDDSample1.Domain.Tokens;
using DDDSample1.Domain.Users;
using DDDSample1.Domain.ValueObjects;
using DDDSample1.DTO;
using Microsoft.AspNetCore.Mvc;
using Moq;
using AppServices;

namespace DDDNetCore.test.IntegrationTest
{
    public class US09IntegrationTest{
        private readonly Mock<IPatientRepository> _mockPatientRepo;
        private readonly Mock<ILogRepository> _mockLogRepo;
        private readonly Mock<IUserRepository> _mockUserRepo;
        private readonly Mock<ITokenRepository> _mockTokenRepo;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;

        private readonly PatientService _patSvc;
        private readonly TokenService _tokenSvc;
        private readonly LogService _logSvc;

        private readonly AppointmentService _apptSvc;
        private readonly PatientController controller;
        private readonly User user;

        public US09IntegrationTest() {
            _mockPatientRepo = new Mock<IPatientRepository>();
            _mockUserRepo = new Mock<IUserRepository>();
            _mockLogRepo = new Mock<ILogRepository>();
            _mockTokenRepo = new Mock<ITokenRepository>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();

            _tokenSvc = new TokenService(_mockUnitOfWork.Object,  _mockUserRepo.Object, _mockTokenRepo.Object); 
            _patSvc = new PatientService(_mockUnitOfWork.Object, _mockPatientRepo.Object, _mockUserRepo.Object, null, _tokenSvc, _apptSvc);
            _logSvc = new LogService(_mockUnitOfWork.Object, _mockLogRepo.Object);

            user = new User(
                "user@example.com",
                "!Password123",
                UserRole.PATIENT
                );
            controller = new PatientController(_patSvc, _tokenSvc, _logSvc);


        }


        [Fact]
        public async Task EditPatientProfileAdmin_Sucess_WithCorrectInputs()
        {
            EditPatientDto_Admin editData = new EditPatientDto_Admin();
            editData.FirstName = "Johnathaneius";
            editData.patientId="MRN12345";

            Token token = new Token(
                new TokenId(Guid.NewGuid()),
                DateTime.Now.AddDays(1),
                user,
                TokenType.ADMIN_AUTH_TOKEN
                );

            Patient patient =
                new Patient(
                    new MedicalRecordNumber("MRN12345"),
                    new FirstName("Johnny"),
                    new LastName("Test"),
                    new FullName("Johnny Test"),
                    Gender.MALE,
                    new DateOfBirth("2001-04-01"),
                    new ContactInformation(new EmailAddress("johnny@email.com"), new PhoneNumber("")),
                     new PhoneNumber(""),
                     new List<Appointment>(),
                     user
                );
            Patient newPatient = patient;
            newPatient.firstName = new FirstName("Johnathaneius");

            _mockTokenRepo.Setup(s => s.GetByIdAsync(It.IsAny<TokenId>()))
                .ReturnsAsync(token);

            _mockPatientRepo.Setup(s => s.GetByIdAsync(It.IsAny<MedicalRecordNumber>()))
                .ReturnsAsync(patient);

            _mockPatientRepo.Setup(s => s.Update(It.IsAny<Patient>()))
                .Returns(newPatient);

            LogsBuilder builder = new LogsBuilder();
            builder.WithDateAndTime()
                .WithID(patient.Id.ToString())
                .WithInformation(patient.ToString())
                .WithObjectType(ObjectLoggedType.PATIENT_UPDATE_ATTEMPT.ToString());
            Log log = builder.Build();

            _mockLogRepo.Setup(l => l.AddAsync(It.IsAny<Log>()))
                .ReturnsAsync(log);

            _mockUnitOfWork.Setup(u => u.CommitAsync());


           var result = await controller.EditPatientProfileAdmin(editData, token.Id.AsString());

           var okResult = Assert.IsType<OkObjectResult>(result.Result);
           var returnValue = Assert.IsType<PatientDto>(okResult.Value);

            Assert.Equal(newPatient.toDto().ToString(), returnValue.ToString());
            _mockLogRepo.Verify(s => s.AddAsync(It.IsAny<Log>()), Times.Once);
            _mockTokenRepo.Verify(s => s.GetByIdAsync(It.IsAny<TokenId>()), Times.Once);
            _mockPatientRepo.Verify(s => s.Update(It.IsAny<Patient>()), Times.Once);
        }

        [Fact]
        public async Task EditPatientProfile_Failure_AuthFail()
        {
            EditPatientDto_Admin editData = new EditPatientDto_Admin();
            editData.FirstName = "Johnathaneius";
            editData.patientId="MRN12345";

            Token token = new Token(
                new TokenId(Guid.NewGuid()),
                DateTime.Now.AddDays(1),
                user,
                TokenType.PATIENT_AUTH_TOKEN
                );


            _mockTokenRepo.Setup(s => s.GetByIdAsync(It.IsAny<TokenId>()))
                .ReturnsAsync(token);

            var result = await controller.EditPatientProfileAdmin(editData, token.Id.AsString());

            var okResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var returnValue = Assert.IsType<string>(okResult.Value);

            Assert.Equal("ACCESS TO RESOURCE DENIED.", returnValue);
        }

        [Fact]
        public async Task EditPatientProfileAdmin_Failure_WithIncorrectInputs()
        {

            //Arrange
            EditPatientDto_Admin editData = new EditPatientDto_Admin();
            editData.FirstName = "Johnathaneius";
            editData.Email = "newemail";
            editData.patientId="MRN12345";

            Token token = new Token(
                new TokenId(Guid.NewGuid()),
                DateTime.Now.AddDays(1),
                user,
                TokenType.ADMIN_AUTH_TOKEN
                );

            Patient patient =
                new Patient(
                    new MedicalRecordNumber("MRN12345"),
                    new FirstName("Johnny"),
                    new LastName("Test"),
                    new FullName("Johnny Test"),
                    Gender.MALE,
                    new DateOfBirth("2001-04-01"),
                    new ContactInformation(new EmailAddress("johnny@email.com"), new PhoneNumber("")),
                     new PhoneNumber(""),
                     new List<Appointment>(),
                     user
                    );
            Patient newPatient = patient;
            newPatient.firstName = new FirstName("Johnathaneius");

            _mockTokenRepo.Setup(s => s.GetByIdAsync(It.IsAny<TokenId>()))
                .ReturnsAsync(token);

            _mockPatientRepo.Setup(s => s.GetByIdAsync(It.IsAny<MedicalRecordNumber>()))
                .ReturnsAsync(patient);

            _mockPatientRepo.Setup(s => s.Update(It.IsAny<Patient>()))
                .Returns(newPatient);

            LogsBuilder builder = new LogsBuilder();
            builder.WithDateAndTime()
                .WithID(patient.Id.ToString())
                .WithInformation(patient.ToString())
                .WithObjectType(ObjectLoggedType.PATIENT_UPDATE_ATTEMPT.ToString());
            Log log = builder.Build();

            _mockLogRepo.Setup(l => l.AddAsync(It.IsAny<Log>()))
                .ReturnsAsync(log);

            _mockUnitOfWork.Setup(u => u.CommitAsync());

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await controller.EditPatientProfileAdmin(editData, token.Id.AsString());
            });

            Assert.Equal("Invalid email address format. (Parameter 'value')", exception.Message);
            Assert.Equal("value", exception.ParamName);


            //Assert
            _mockTokenRepo.Verify(s => s.GetByIdAsync(It.IsAny<TokenId>()), Times.Once);
            _mockPatientRepo.Verify(s => s.GetByUserIdAsync(It.IsAny<Username>()), Times.Never);
        }

    }
}
