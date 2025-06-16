using DDDSample1.Controllers;
using DDDSample1.Domain.ContactInformations;
using DDDSample1.Domain.HospitalAppointment;
using DDDSample1.Domain.HospitalPatient;
using DDDSample1.Domain.HospitalStaff;
using DDDSample1.Domain.Shared;
using DDDSample1.Domain.Tokens;
using DDDSample1.Domain.Users;
using DDDSample1.Domain.ValueObjects;
using DDDSample1.DTO;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DDDNetCore.test.ControllerTest
{
    public class UserControllerTest
    {
        private readonly Mock<UserService> _mockUserService;
        private readonly Mock<LoginService> _mockLoginService;
        private readonly Mock<PatientService> _mockPatientService;
        private readonly Mock<TokenService> _mockTokenService;
        private Patient patient;
        private User user;
        private UsersController controller;


        public UserControllerTest()
        {

            _mockUserService = new Mock<UserService>();
            _mockLoginService = new Mock<LoginService>();
            _mockPatientService = new Mock<PatientService>();
            _mockTokenService = new Mock<TokenService>();

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

            controller = new UsersController(_mockUserService.Object, _mockLoginService.Object, _mockPatientService.Object, _mockTokenService.Object);

        }


        [Fact]
        public async Task RegisterUserPatient_Success_WithProperParameters()
        {
            _mockPatientService.Setup(s => s.checkIfPatientProfileExists(It.IsAny<string>()))
                .ReturnsAsync(patient.toDto());
            _mockUserService.Setup(s => s.AddWithPasswordAsync(It.IsAny<LoginCredentialsDto>()))
                .ReturnsAsync(user.ToDto());
            _mockTokenService.Setup(s => s.GeneratePasswordValidationTokenAsync(It.IsAny<UserDto>()))
                .ReturnsAsync(new TokenDto("fill", "fill", "fill", "fill"));
            var loginCredentialsDto = new LoginCredentialsDto(user.EmailAddress.ToString(), user.Password.ToString());

            var result = controller.RegisterUserPatient(loginCredentialsDto);

            _mockPatientService.Verify(s => s.checkIfPatientProfileExists(It.IsAny<string>()), Times.Once);
            _mockUserService.Verify(s => s.AddWithPasswordAsync(It.IsAny<LoginCredentialsDto>()), Times.Once);
            _mockTokenService.Verify(s => s.GeneratePasswordValidationTokenAsync(It.IsAny<UserDto>()), Times.Once);

        }

        [Fact]
        public async Task RegisterUserPatient_Failure_PatientProfileDoesNotExist()
        {
            _mockPatientService.Setup(s => s.checkIfPatientProfileExists(It.IsAny<string>()))
                .ReturnsAsync((PatientDto)null);
            
            var loginCredentialsDto = new LoginCredentialsDto(user.EmailAddress.ToString(), user.Password.ToString());

            // Act
            var result = await controller.RegisterUserPatient(loginCredentialsDto);

            // Assert
            var resultAct = Assert.IsType<ActionResult<UserDto>>(result); // Check if it's a BadRequestObjectResult
            var resultBadRequest = Assert.IsType<BadRequestObjectResult>(result.Result); // Check if it's a BadRequestObjectResult

            var returnValue = Assert.IsType<string>(resultBadRequest.Value);       // Check if the content of BadRequest is a string
            Assert.Equal("No Available Patient Profile shares the same e-mail as the one you are trying to login.", returnValue); // Verify the content


            _mockPatientService.Verify(s => s.checkIfPatientProfileExists(It.IsAny<string>()), Times.Once);
            _mockUserService.Verify(s => s.AddWithPasswordAsync(It.IsAny<LoginCredentialsDto>()), Times.Never);
            _mockTokenService.Verify(s => s.GeneratePasswordValidationTokenAsync(It.IsAny<UserDto>()), Times.Never);

        }

        [Fact]
        public async Task RegisterUserPatient_Failure_AlreadyAssociated()
        {
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
            _mockPatientService.Setup(s => s.checkIfPatientProfileExists(It.IsAny<string>()))
                .ReturnsAsync(patient.toDto());
            _mockUserService.Setup(s => s.AddWithPasswordAsync(It.IsAny<LoginCredentialsDto>()))
                .ReturnsAsync(user.ToDto());
            _mockTokenService.Setup(s => s.GeneratePasswordValidationTokenAsync(It.IsAny<UserDto>()))
                .ReturnsAsync(new TokenDto("fill", "fill", "fill", "fill"));

            var loginCredentialsDto = new LoginCredentialsDto(user.EmailAddress.ToString(), user.Password.ToString());

            // Act
            var result = await controller.RegisterUserPatient(loginCredentialsDto);

            // Assert
            var resultAct = Assert.IsType<ActionResult<UserDto>>(result); // Check if it's a BadRequestObjectResult
            var resultBadRequest = Assert.IsType<BadRequestObjectResult>(result.Result); // Check if it's a BadRequestObjectResult

            var returnValue = Assert.IsType<string>(resultBadRequest.Value);       // Check if the content of BadRequest is a string
            Assert.Equal("The Patient Profile that shares the same email as the one you are trying to login, already has a user associated." +
                "\nTry logging in with your information.", returnValue); // Verify the content


            _mockPatientService.Verify(s => s.checkIfPatientProfileExists(It.IsAny<string>()), Times.Once);
            _mockUserService.Verify(s => s.AddWithPasswordAsync(It.IsAny<LoginCredentialsDto>()), Times.Never);
            _mockTokenService.Verify(s => s.GeneratePasswordValidationTokenAsync(It.IsAny<UserDto>()), Times.Never);

        }
    }
}
