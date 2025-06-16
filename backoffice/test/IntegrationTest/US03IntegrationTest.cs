using DDDSample1.Controllers;
using DDDSample1.Domain.ContactInformations;
using DDDSample1.Domain.HospitalAppointment;
using DDDSample1.Domain.HospitalPatient;
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
    public class US03IntegrationTest
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IPatientRepository> _mockPatientRepo;
        private readonly Mock<IUserRepository> _mockUserRepo;
        private readonly Mock<ITokenRepository> _mockTokenRepo;

        private UserService _userSvc;
        private PatientService _patSvc;
        private TokenService _tokenSvc;

        private AppointmentService _apptSvc;
        private Patient patient;
        private User user;
        private UsersController controller;

        public US03IntegrationTest()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockPatientRepo = new Mock<IPatientRepository>();
            _mockUserRepo = new Mock<IUserRepository>();
            _mockTokenRepo = new Mock<ITokenRepository>();

            _userSvc = new UserService(_mockUnitOfWork.Object, _mockUserRepo.Object, null);
            _tokenSvc = new TokenService(_mockUnitOfWork.Object, _mockUserRepo.Object, _mockTokenRepo.Object);
            _patSvc = new PatientService(_mockUnitOfWork.Object, _mockPatientRepo.Object, _mockUserRepo.Object, null, _tokenSvc, _apptSvc);
            
            user = new User(
                "user@example.com",
                "!Password123",
                UserRole.PATIENT
                );
            patient =
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
                     null
                );

            controller = new UsersController(_userSvc, null, _patSvc, _tokenSvc);


        }

        [Fact]
        public async Task RegisterUserPatient_Success_WithProperParameters()
        {
            //Arrange
            _mockPatientRepo.Setup(s => s.GetByEmailAsync(It.IsAny<EmailAddress>()))
                .ReturnsAsync(patient);

            _mockPatientRepo.Setup(s => s.GetByIdAsync(It.IsAny<MedicalRecordNumber>()))
                .ReturnsAsync(patient);

            _mockUserRepo.Setup(s => s.AddAsync(It.IsAny<User>()))
                .ReturnsAsync(user);

            _mockUserRepo.Setup(s => s.GetByIdAsync(It.IsAny<Username>()))
                .ReturnsAsync(user);

            var mockTokenId = new TokenId(Guid.NewGuid());
            var expirationDate = DateTime.Now.AddDays(1);
            var tokenType = TokenType.PASSWORD_RESET_TOKEN;

            _mockTokenRepo.Setup(s => s.AddAsync(It.IsAny<Token>()))
                .ReturnsAsync(new Token(mockTokenId, expirationDate, user, tokenType));

            //Act
            var result = await controller.RegisterUserPatient(new LoginCredentialsDto(user.EmailAddress.ToString(), user.Password.ToString()));

            //Assert
            _mockPatientRepo.Verify(s => s.GetByEmailAsync(It.IsAny<EmailAddress>()), Times.Once);
            _mockUserRepo.Verify(s => s.AddAsync(It.IsAny<User>()), Times.Once);
            _mockTokenRepo.Verify(s => s.AddAsync(It.IsAny<Token>()), Times.Once);
        }

        [Fact]
        public async Task RegisterUserPatient_Failure_PatientProfileDoesNotExist()
        {
            //Arrange
            _mockPatientRepo.Setup(s => s.GetByEmailAsync(It.IsAny<EmailAddress>()))
                .ReturnsAsync((Patient)null);

            _mockPatientRepo.Setup(s => s.GetByIdAsync(It.IsAny<MedicalRecordNumber>()))
                .ReturnsAsync(patient);

            _mockUserRepo.Setup(s => s.AddAsync(It.IsAny<User>()))
                .ReturnsAsync(user);

            _mockUserRepo.Setup(s => s.GetByIdAsync(It.IsAny<Username>()))
                .ReturnsAsync(user);

            var mockTokenId = new TokenId(Guid.NewGuid());
            var expirationDate = DateTime.Now.AddDays(1);
            var tokenType = TokenType.PASSWORD_RESET_TOKEN;

            _mockTokenRepo.Setup(s => s.AddAsync(It.IsAny<Token>()))
                .ReturnsAsync(new Token(mockTokenId, expirationDate, user, tokenType));

            //Act
            var result = await controller.RegisterUserPatient(new LoginCredentialsDto(user.EmailAddress.ToString(), user.Password.ToString()));

            //Assert
            var resultAct = Assert.IsType<ActionResult<UserDto>>(result);
            var resultBadRequest = Assert.IsType<BadRequestObjectResult>(result.Result);

            var returnValue = Assert.IsType<string>(resultBadRequest.Value);
            Assert.Equal("No Available Patient Profile shares the same e-mail as the one you are trying to login.", returnValue);
        }

        [Fact]
        public async Task RegisterUserPatient_Failure_AlreadyAssociated()
        {
            //Arrange
            patient =
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

            _mockPatientRepo.Setup(s => s.GetByEmailAsync(It.IsAny<EmailAddress>()))
                .ReturnsAsync(patient);

            _mockPatientRepo.Setup(s => s.GetByIdAsync(It.IsAny<MedicalRecordNumber>()))
                .ReturnsAsync(patient);

            _mockUserRepo.Setup(s => s.AddAsync(It.IsAny<User>()))
                .ReturnsAsync(user);

            _mockUserRepo.Setup(s => s.GetByIdAsync(It.IsAny<Username>()))
                .ReturnsAsync(user);

            var mockTokenId = new TokenId(Guid.NewGuid());
            var expirationDate = DateTime.Now.AddDays(1);
            var tokenType = TokenType.PASSWORD_RESET_TOKEN;

            _mockTokenRepo.Setup(s => s.AddAsync(It.IsAny<Token>()))
                .ReturnsAsync(new Token(mockTokenId, expirationDate, user, tokenType));

            //Act
            var result = await controller.RegisterUserPatient(new LoginCredentialsDto(user.EmailAddress.ToString(), user.Password.ToString()));

            // Assert
            var resultAct = Assert.IsType<ActionResult<UserDto>>(result); // Check if it's a BadRequestObjectResult
            var resultBadRequest = Assert.IsType<BadRequestObjectResult>(result.Result); // Check if it's a BadRequestObjectResult

            //Assert
            var returnValue = Assert.IsType<string>(resultBadRequest.Value);       // Check if the content of BadRequest is a string
            Assert.Equal("The Patient Profile that shares the same email as the one you are trying to login, already has a user associated." +
                "\nTry logging in with your information.", returnValue); // Verify the content
          
            _mockPatientRepo.Verify(s => s.GetByEmailAsync(It.IsAny<EmailAddress>()), Times.Once);
            _mockUserRepo.Verify(s => s.AddAsync(It.IsAny<User>()), Times.Never);
            _mockTokenRepo.Verify(s => s.AddAsync(It.IsAny<Token>()), Times.Never);
        }
    }
}
