using DDDSample1.Controllers;
using DDDSample1.Domain.ContactInformations;
using DDDSample1.Domain.HospitalAppointment;
using DDDSample1.Domain.HospitalPatient;
using DDDSample1.Domain.Logs;
using DDDSample1.Domain.Shared;
using DDDSample1.Domain.Tokens;
using DDDSample1.Domain.Users;
using DDDSample1.Domain.ValueObjects;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDDNetCore.test.IntegrationTest
{
    public class US05IntegrationTest
    {
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
        private User user;

        public US05IntegrationTest()
        {
            _mockPatientRepo = new Mock<IPatientRepository>();
            _mockUserRepo = new Mock<IUserRepository>();
            _mockLogRepo = new Mock<ILogRepository>();
            _mockTokenRepo = new Mock<ITokenRepository>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();

            _tokenSvc = new TokenService(_mockUnitOfWork.Object, _mockUserRepo.Object, _mockTokenRepo.Object);
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
        public async Task US05_Success_WithGoodValues()
        {
            //Arrange
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

            LogsBuilder builder = new LogsBuilder();
            builder.WithDateAndTime()
                .WithID(patient.Id.ToString())
                .WithInformation(patient.ToString())
                .WithObjectType(ObjectLoggedType.PATIENT_UPDATE_ATTEMPT.ToString());
            Log log = builder.Build();

            Token token = new Token(
                    new TokenId(Guid.NewGuid()),
                    DateTime.Now.AddDays(1),
                    user,
                    TokenType.DELETION_TOKEN
            );

            _mockTokenRepo.Setup(s => s.GetByIdAsync(It.IsAny<TokenId>()))
                .ReturnsAsync(token);
            _mockPatientRepo.Setup(s => s.GetByUserIdAsync(It.IsAny<Username>()))
                .ReturnsAsync(patient);

            _mockLogRepo.Setup(s => s.AddAsync(It.IsAny<Log>()))
                .ReturnsAsync(log);


            _mockUnitOfWork.Setup(s => s.CommitAsync());

            //Act
            var result = await controller.ConfirmPatientDeletion(token.Id.AsString());

            //Assert

            _mockTokenRepo.Verify(s => s.GetByIdAsync(It.IsAny<TokenId>()), Times.Once);
            _mockPatientRepo.Verify(s => s.GetByUserIdAsync(It.IsAny<Username>()), Times.Once);
            _mockLogRepo.Verify(s => s.AddAsync(It.IsAny<Log>()), Times.Once);
            _mockUnitOfWork.Verify(s => s.CommitAsync(), Times.Once);


            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            // Assert the value is not null
            Assert.NotNull(okResult.Value);

            // Use reflection to get the "message" property value
            var returnValue = okResult.Value.GetType().GetProperty("message")?.GetValue(okResult.Value, null);

            // Assert the "message" property value
            Assert.NotNull(returnValue);
            Assert.Equal("Confirmation Accepted. Patient Deletion is schedule to happen within the GRPD Parameters.", returnValue);
        }


        [Fact]
        public async Task US05_Failure_WithBadAuth()
        {
            //Arrange


            Token token = new Token(
                    new TokenId(Guid.NewGuid()),
                    DateTime.Now.AddDays(1),
                    user,
                    TokenType.GENERAL_ACCESS
            );

            _mockTokenRepo.Setup(s => s.GetByIdAsync(It.IsAny<TokenId>()))
                .ReturnsAsync(token);



            //Act
            var result = await controller.ConfirmPatientDeletion(token.Id.AsString());

            //Assert
            _mockTokenRepo.Verify(s => s.GetByIdAsync(It.IsAny<TokenId>()), Times.Once);

            var okResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var returnValue = Assert.IsType<string>(okResult.Value);

            Assert.Equal("ACCESS TO RESOURCE DENIED.", returnValue);
        }


        [Fact]
        public async Task US05_Exception_WithBadMrn()
        {
            //Arrange
            Patient patient =
                new Patient(
                    new MedicalRecordNumber("bingobango"),
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

            LogsBuilder builder = new LogsBuilder();
            builder.WithDateAndTime()
                .WithID(patient.Id.ToString())
                .WithInformation(patient.ToString())
                .WithObjectType(ObjectLoggedType.PATIENT_UPDATE_ATTEMPT.ToString());
            Log log = builder.Build();

            Token token = new Token(
                    new TokenId(Guid.NewGuid()),
                    DateTime.Now.AddDays(1),
                    user,
                    TokenType.DELETION_TOKEN
            );

            _mockTokenRepo.Setup(s => s.GetByIdAsync(It.IsAny<TokenId>()))
                .ReturnsAsync(token);
            _mockPatientRepo.Setup(s => s.GetByUserIdAsync(It.IsAny<Username>()))
                .ReturnsAsync((Patient) null);

            _mockLogRepo.Setup(s => s.AddAsync(It.IsAny<Log>()))
                .ReturnsAsync(log);


            _mockUnitOfWork.Setup(s => s.CommitAsync());

            //Act
            //Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await controller.ConfirmPatientDeletion(token.Id.AsString());
            });



        }
    }
}
