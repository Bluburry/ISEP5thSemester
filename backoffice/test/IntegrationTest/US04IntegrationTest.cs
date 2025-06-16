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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDDNetCore.test.IntegrationTest
{
    public class US04IntegrationTest{
        private readonly Mock<IPatientRepository> _mockPatientRepo;
        private readonly Mock<ILogRepository> _mockLogRepo;
        private readonly Mock<IUserRepository> _mockUserRepo;
        private readonly Mock<ITokenRepository> _mockTokenRepo;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;

        private readonly PatientService _patSvc;
        private readonly TokenService _tokenSvc;
        private readonly AppointmentService _apptSvc;
        private readonly LogService _logSvc;

        private readonly PatientController controller;
        private readonly User user;

        public US04IntegrationTest() {
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
        public async Task EditProfile_Sucess_WithCorrectInputs()
        {
            EditPatientDto_Patient editData = new EditPatientDto_Patient();
            editData.FirstName = "Johnathaneius";

            string tokenId = "433e14b5-6c89-4dfb-b258-6e8f8d08a4d6";

            Token token = new Token(
                new TokenId(tokenId),
                DateTime.Now.AddDays(1),
                user,
                TokenType.PATIENT_AUTH_TOKEN
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
            
            _mockTokenRepo.Setup(s => s.AddAsync(It.IsAny<Token>()))
                .ReturnsAsync(token);

            _mockPatientRepo.Setup(s => s.GetByUserIdAsync(It.IsAny<Username>()))
                .ReturnsAsync(patient);

            _mockPatientRepo.Setup(s => s.Update(It.IsAny<Patient>()))
                .Returns(newPatient);

            _mockUserRepo.Setup(s=> s.GetByIdAsync(It.IsAny<Username>()))
            .ReturnsAsync(user);

            LogsBuilder builder = new LogsBuilder();
            builder.WithDateAndTime()
                .WithID(patient.Id.ToString())
                .WithInformation(patient.ToString())
                .WithObjectType(ObjectLoggedType.PATIENT_UPDATE_ATTEMPT.ToString());
            Log log = builder.Build();

            _mockLogRepo.Setup(l => l.AddAsync(It.IsAny<Log>()))
                .ReturnsAsync(log);

            _mockUnitOfWork.Setup(u => u.CommitAsync());


           var result = await controller.EditPatientProfilePatient(editData, tokenId);

           var okResult = Assert.IsType<OkObjectResult>(result.Result);

            _mockLogRepo.Verify(s => s.AddAsync(It.IsAny<Log>()), Times.Once);
            _mockTokenRepo.Verify(s => s.GetByIdAsync(It.IsAny<TokenId>()), Times.Once);
            _mockPatientRepo.Verify(s => s.Update(It.IsAny<Patient>()), Times.Never);
            _mockPatientRepo.Verify(s => s.GetByUserIdAsync(It.IsAny<Username>()), Times.AtLeastOnce);



        }

        [Fact]
        public async Task EditProfile_AuthFail()
        {
            EditPatientDto_Patient editData = new EditPatientDto_Patient();
            editData.FirstName = "Johnathaneius";

            string tokenId = "433e14b5-6c89-4dfb-b258-6e8f8d08a4d6";

            Token token = new Token(
                new TokenId(tokenId),
                DateTime.Now.AddDays(1),
                user,
                TokenType.GENERAL_ACCESS
                );


            _mockTokenRepo.Setup(s => s.GetByIdAsync(It.IsAny<TokenId>()))
                .ReturnsAsync(token);

            var result = await controller.EditPatientProfilePatient(editData, tokenId);

            var okResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var returnValue = Assert.IsType<string>(okResult.Value);

            Assert.Equal(returnValue, "ACCESS TO RESOURCE DENIED.");


        }

        [Fact]
        public async Task EditPatientProfilePatient_SendsEmail_WithSensitiveInformation()
        {

            //Arrange

            EditPatientDto_Patient editData = new EditPatientDto_Patient();
            editData.FirstName = "Johnathaneius";

            string tokenId = "433e14b5-6c89-4dfb-b258-6e8f8d08a4d6";

            Token token = new Token(
                new TokenId(tokenId),
                DateTime.Now.AddDays(1),
                user,
                TokenType.PATIENT_AUTH_TOKEN
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
            
            _mockTokenRepo.Setup(s => s.AddAsync(It.IsAny<Token>()))
                .ReturnsAsync(token);

            _mockPatientRepo.Setup(s => s.GetByUserIdAsync(It.IsAny<Username>()))
                .ReturnsAsync(patient);

            _mockPatientRepo.Setup(s => s.Update(It.IsAny<Patient>()))
                .Returns(newPatient);

            _mockUserRepo.Setup(s=> s.GetByIdAsync(It.IsAny<Username>()))
            .ReturnsAsync(user);

            LogsBuilder builder = new LogsBuilder();
            builder.WithDateAndTime()
                .WithID(patient.Id.ToString())
                .WithInformation(patient.ToString())
                .WithObjectType(ObjectLoggedType.PATIENT_UPDATE_ATTEMPT.ToString());
            Log log = builder.Build();

            _mockLogRepo.Setup(l => l.AddAsync(It.IsAny<Log>()))
                .ReturnsAsync(log);

            _mockUnitOfWork.Setup(u => u.CommitAsync());


           var result = await controller.EditPatientProfilePatient(editData, tokenId);

           var okResult = Assert.IsType<OkObjectResult>(result.Result);

            _mockLogRepo.Verify(s => s.AddAsync(It.IsAny<Log>()), Times.Once);
            _mockTokenRepo.Verify(s => s.GetByIdAsync(It.IsAny<TokenId>()), Times.Once);
            _mockPatientRepo.Verify(s => s.Update(It.IsAny<Patient>()), Times.Never);
            _mockPatientRepo.Verify(s => s.GetByUserIdAsync(It.IsAny<Username>()), Times.AtLeastOnce);





        }

    }
}
