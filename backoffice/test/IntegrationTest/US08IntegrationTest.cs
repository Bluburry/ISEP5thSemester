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
using DDDSample1.DTO.LoginAttemptTrackers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDDNetCore.test.IntegrationTest
{
    public class US08IntegrationTest
    {
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
        private User user;

        public US08IntegrationTest()
        {
            _mockPatientRepo = new Mock<IPatientRepository>();
            _mockUserRepo = new Mock<IUserRepository>();
            _mockLogRepo = new Mock<ILogRepository>();
            _mockTokenRepo = new Mock<ITokenRepository>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();

            _tokenSvc = new TokenService(_mockUnitOfWork.Object, _mockUserRepo.Object, _mockTokenRepo.Object);
            _patSvc = new PatientService(_mockUnitOfWork.Object, _mockPatientRepo.Object, _mockUserRepo.Object, null, _tokenSvc, _apptSvc);
            _logSvc = new LogService(_mockUnitOfWork.Object, _mockLogRepo.Object);

            
            controller = new PatientController(_patSvc, _tokenSvc, _logSvc);

            
        }

        [Fact]
        public async Task CreatePatient_Success_WithGoodValues()
        {
            //Arrange
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

            user = new User(
                "user@example.com",
                "!Password123",
                UserRole.PATIENT
                );
            string tokenId = "433e14b5-6c89-4dfb-b258-6e8f8d08a4d6";

            Patient patient =
                new Patient(
                    new MedicalRecordNumber("202410000001"),
                    new FirstName(examplePatient.firstName),
                    new LastName(examplePatient.lastName),
                    new FullName(examplePatient.fullName),
                    Gender.MALE,
                    new DateOfBirth(examplePatient.dateOfBirth),
                    new ContactInformation(new EmailAddress(examplePatient.email), new PhoneNumber(examplePatient.phone)),
                    new PhoneNumber(examplePatient.phone),
                    new List<Appointment>()
                    , null
                );

            Token token = new Token(
                new TokenId(tokenId),
                DateTime.Now.AddDays(1),
                user,
                TokenType.ADMIN_AUTH_TOKEN
                );

            _mockTokenRepo.Setup(s => s.GetByIdAsync(It.IsAny<TokenId>()))
                .ReturnsAsync(token);
            _mockPatientRepo.Setup(s => s.GetByUserIdAsync(It.IsAny<Username>()))
                .ReturnsAsync(patient);

            //Act
            var result = await controller.CreatePatientProfile(tokenId, examplePatient);


            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<PatientDto>(okResult.Value);
            returnValue.mrn = patient.Id.AsString();
            Assert.Equal(returnValue.ToString(), patient.toDto().ToString());
            _mockPatientRepo.Verify(s => s.AddAsync(It.IsAny<Patient>()), Times.Once);
            _mockUnitOfWork.Verify(s => s.CommitAsync(), Times.Once);

        }

        [Fact]
        public async Task CreatePatient_BadRequest_WithBadToken()
        {
            //Arrange
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

            user = new User(
                "user@example.com",
                "!Password123",
                UserRole.PATIENT
                );
            string tokenId = "433e14b5-6c89-4dfb-b258-6e8f8d08a4d6";


            Token token = new Token(
                new TokenId(tokenId),
                DateTime.Now.AddDays(1),
                user,
                TokenType.PASSWORD_RESET_TOKEN
                );

            _mockTokenRepo.Setup(s => s.GetByIdAsync(It.IsAny<TokenId>()))
                .ReturnsAsync(token);


            //Act
            var result = await controller.CreatePatientProfile(tokenId, examplePatient);


            //Assert
            var okResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var returnValue = Assert.IsType<string>(okResult.Value);

            Assert.Equal(returnValue, "ACCESS TO RESOURCE DENIED.");

        }


        [Fact]
        public async Task CreatePatient_Failure_WithBadValues()
        {
            //Arrange
            var examplePatient = new PatientRegistrationDto
            {
                firstName = "John",
                lastName = "Doe",
                fullName = "John Doe",
                gender = "MALE",
                dateOfBirth = "2004-32-32",
                email = "johndoe@example.com",
                phone = "+987654321",
                emergencyContact = "+987654321"
            };

            user = new User(
                "user@example.com",
                "!Password123",
                UserRole.PATIENT
                );
            string tokenId = "433e14b5-6c89-4dfb-b258-6e8f8d08a4d6";

            

            Token token = new Token(
                new TokenId(tokenId),
                DateTime.Now.AddDays(1),
                user,
                TokenType.ADMIN_AUTH_TOKEN
                );

            _mockTokenRepo.Setup(s => s.GetByIdAsync(It.IsAny<TokenId>()))
                .ReturnsAsync(token);


            //Act
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await controller.CreatePatientProfile(tokenId, examplePatient);
            });


        }
    }
}
