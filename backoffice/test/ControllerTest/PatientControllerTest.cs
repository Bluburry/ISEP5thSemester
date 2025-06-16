using DDDSample1.Controllers;
using DDDSample1.Domain.HospitalPatient;
using DDDSample1.Domain.Logs;
using DDDSample1.Domain.Tokens;
using DDDSample1.DTO;
using DDDSample1.DTO.LoginAttemptTrackers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDDNetCore.test.ControllerTest
{
    public class PatientControllerTest
    {
        private readonly Mock<PatientService> _mockPatientSvc;
        private readonly Mock<TokenService> _mockTokenSvc;
        private readonly Mock<LogService> _mockLogSvc;
        PatientController _controller;

        public PatientControllerTest()
        {
            _mockTokenSvc = new Mock<TokenService>();
            _mockPatientSvc = new Mock<PatientService>();
            _mockLogSvc = new Mock<LogService>();

            _controller = new PatientController(_mockPatientSvc.Object, _mockTokenSvc.Object, _mockLogSvc.Object);
        }

        [Fact]
        public async Task EditPatientProfilePatient_DoesNotAuth_WithBadToken(){

            //Arrange
            _mockTokenSvc
                    .Setup(s => s.GetByIdAsync(It.IsAny<TokenId>()))
                    .ReturnsAsync(new TokenDto(
                        "433e14b5-6c89-4dfb-b258-6e8f8d08a4d6",
                        TokenType.GENERAL_ACCESS.ToString(),
                        DateTime.Now.AddDays(1).ToString(),
                        ""));

            //Act
            var result = await _controller.EditPatientProfilePatient(null, Guid.NewGuid().ToString());

            //Assert
            var okResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var returnValue = Assert.IsType<string>(okResult.Value);

            Assert.Equal(returnValue, "ACCESS TO RESOURCE DENIED.");
        }

        [Fact]
        public async Task EditPatientProfileAdmin_Failure_WithBadToken(){

            //Arrange
            _mockTokenSvc
                    .Setup(s => s.GetByIdAsync(It.IsAny<TokenId>()))
                    .ReturnsAsync(new TokenDto(Guid.NewGuid().ToString(),
                        TokenType.PATIENT_AUTH_TOKEN.ToString(),
                        DateTime.Now.AddDays(1).ToString(),
                        ""));
                        
            //Act
            var result = await _controller.EditPatientProfileAdmin(null, Guid.NewGuid().ToString());

            //Assert
            var okResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var returnValue = Assert.IsType<string>(okResult.Value);

            Assert.Equal("ACCESS TO RESOURCE DENIED.", returnValue);
        }

        [Fact]
        public async Task EditPatientProfileAdmin_Sucess_WithCorrectInputs(){

            //Arrange
            EditPatientDto_Admin patientInfo = new EditPatientDto_Admin();
            patientInfo.FirstName = "Rogerio Levi";
            
            _mockTokenSvc
                    .Setup(s => s.GetByIdAsync(It.IsAny<TokenId>()))
                    .ReturnsAsync(new TokenDto(
                        "433e14b5-6c89-4dfb-b258-6e8f8d08a4d6",
                        TokenType.ADMIN_AUTH_TOKEN.ToString(),
                        DateTime.Now.AddDays(1).ToString(),
                        ""));

            _mockPatientSvc.Setup(s => s.EditPatientProfileAdmin(It.IsAny<EditPatientDto_Admin>()))
                           .ReturnsAsync(new PatientDto());
            _mockPatientSvc.Setup(s=> s.GetByIdAsync(It.IsAny<string>())).ReturnsAsync(new PatientDto());

            //Act
            var result = await _controller.EditPatientProfileAdmin(patientInfo, Guid.NewGuid().ToString());

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<PatientDto>(okResult.Value);
        }

        [Fact]
        public async Task EditPatientProfilePatient_SendsEmail_WithoutEmailChange()
        {
            //Arrange
            EditPatientDto_Patient patientInfo = new EditPatientDto_Patient();
            patientInfo.Fullname = "John Cool";
         

            PatientDto patDto = new PatientDto();
            patDto.fullName = "John Cool";
            patDto.email = "CoolMail@domain.com";

            _mockTokenSvc
                    .Setup(s => s.GetByIdAsync(It.IsAny<TokenId>()))
                    .ReturnsAsync(new TokenDto(
                        "433e14b5-6c89-4dfb-b258-6e8f8d08a4d6",
                        TokenType.PATIENT_AUTH_TOKEN.ToString(),
                        DateTime.Now.AddDays(1).ToString(),
                        ""));

            _mockLogSvc
                .Setup(s => s.LogPatientEditing(It.IsAny<PatientDto>()));

            _mockPatientSvc
                .Setup(s => s.EditPatientProfilePatient(It.IsAny<EditPatientDto_Patient>(), It.IsAny<TokenDto>()))
                .ReturnsAsync(patDto);

            _mockPatientSvc
                .Setup(s => s.GetByUserIdAsync(It.IsAny<string>()))
                .ReturnsAsync(patDto);

            //Act
            var result = await _controller.EditPatientProfilePatient(patientInfo, Guid.NewGuid().ToString());

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<PatientDto>(okResult.Value);

            _mockPatientSvc.Verify(l => l.EditPatientProfilePatient(It.IsAny<EditPatientDto_Patient>(), It.IsAny<TokenDto>()), Times.Once);
            _mockLogSvc.Verify(l => l.LogPatientEditing(It.IsAny<PatientDto>()), Times.Once);

            Assert.Equal(returnValue, patDto);
        }


        [Fact]
        public async Task DeleteSelfPatientProfile_Failure_BadAuthentication()
        {
            TokenDto tokenDto =
                new TokenDto(
                    Guid.NewGuid().ToString(),
                    TokenType.GENERAL_ACCESS.ToString(),
                    DateTime.Now.AddDays(1).ToString(),
                    Guid.NewGuid().ToString());

            _mockTokenSvc.Setup(s => s.GetByIdAsync(It.IsAny<TokenId>()))
                .ReturnsAsync(tokenDto);


            var result = await _controller.DeleteSelfPatientProfile(Guid.NewGuid().ToString());


            var okResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var returnValue = Assert.IsType<string>(okResult.Value);

            Assert.Equal("ACCESS TO RESOURCE DENIED.", returnValue);

        }

        [Fact]
        public async Task DeletePatientProfile_Failure_BadAuthentication()
        {
            TokenDto tokenDto =
                new TokenDto(
                    Guid.NewGuid().ToString(),
                    TokenType.PATIENT_AUTH_TOKEN.ToString(),
                    DateTime.Now.AddDays(1).ToString(),
                    Guid.NewGuid().ToString());

            _mockTokenSvc.Setup(s => s.GetByIdAsync(It.IsAny<TokenId>()))
                .ReturnsAsync(tokenDto);


            var result = await _controller.DeletePatientProfile("MRN1234",Guid.NewGuid().ToString());


            var okResult = Assert.IsType<BadRequestObjectResult>(result);
            var returnValue = Assert.IsType<string>(okResult.Value);

            Assert.Equal("ACCESS TO RESOURCE DENIED.", returnValue);

        }

        [Fact]
        public async Task DeletePatientProfile_Successa_GoodValues()
        {
            TokenDto tokenDto =
                new TokenDto(
                    Guid.NewGuid().ToString(),
                    TokenType.ADMIN_AUTH_TOKEN.ToString(),
                    DateTime.Now.AddDays(1).ToString(),
                    Guid.NewGuid().ToString());

            _mockTokenSvc.Setup(s => s.GetByIdAsync(It.IsAny<TokenId>()))
                .ReturnsAsync(tokenDto);

            _mockPatientSvc.Setup(s=> s.GetByIdAsync(It.IsAny<string>())).ReturnsAsync(new PatientDto());

            var result = await _controller.DeletePatientProfile("MRN1234",Guid.NewGuid().ToString());


            var okResult = Assert.IsType<OkResult>(result);
            var returnValue = Assert.IsType<OkResult>(okResult);

            _mockLogSvc.Verify(l => l.LogPatientDeletion(It.IsAny<PatientDto>()), Times.Once);
        }

        [Fact]
        public async Task DeleteSelfPatientProfile_Successa_GoodValues()
        {
            TokenDto tokenDto =
                new TokenDto(
                    Guid.NewGuid().ToString(),
                    TokenType.PATIENT_AUTH_TOKEN.ToString(),
                    DateTime.Now.AddDays(1).ToString(),
                    Guid.NewGuid().ToString());

            _mockTokenSvc.Setup(s => s.GetByIdAsync(It.IsAny<TokenId>()))
                .ReturnsAsync(tokenDto);
            
            _mockPatientSvc.Setup(s => s.GetByUserIdAsync(It.IsAny<string>()))
                .ReturnsAsync(new PatientDto());

            _mockLogSvc.Setup(s => s.LogPatientDeletion(It.IsAny<PatientDto>()))
                .ReturnsAsync(new LogDto());


            var result = await _controller.DeleteSelfPatientProfile(Guid.NewGuid().ToString());


            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            // Assert the value is not null
            Assert.NotNull(okResult.Value);

            // Use reflection to get the "message" property value
            var returnValue = okResult.Value.GetType().GetProperty("message")?.GetValue(okResult.Value, null);

            // Assert the "message" property value
            Assert.NotNull(returnValue);
            Assert.Equal("Patient Deletion requires Email Confirmation. Check your inbox to confirm the process.", returnValue.ToString());

        }

        [Fact]
        public async Task ConfirmPatientDeletion_Failure_BadAuthentication()
        {
            TokenDto tokenDto =
                new TokenDto(
                    Guid.NewGuid().ToString(),
                    TokenType.GENERAL_ACCESS.ToString(),
                    DateTime.Now.AddDays(1).ToString(),
                    Guid.NewGuid().ToString());

            _mockTokenSvc.Setup(s => s.GetByIdAsync(It.IsAny<TokenId>()))
                .ReturnsAsync(tokenDto);


            var result = await _controller.ConfirmPatientDeletion(Guid.NewGuid().ToString());


            var okResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var returnValue = Assert.IsType<string>(okResult.Value);

            Assert.Equal("ACCESS TO RESOURCE DENIED.", returnValue);

        }

        [Fact]
        public async Task ConfirmPatientDeletion_Success_GoodValues()
        {
            TokenDto tokenDto =
                new TokenDto(
                    Guid.NewGuid().ToString(),
                    TokenType.DELETION_TOKEN.ToString(),
                    DateTime.Now.AddDays(1).ToString(),
                    Guid.NewGuid().ToString());

            _mockTokenSvc.Setup(s => s.GetByIdAsync(It.IsAny<TokenId>()))
                .ReturnsAsync(tokenDto);

            _mockPatientSvc.Setup(s => s.GetByUserIdAsync(It.IsAny<string>()))
                .ReturnsAsync(new PatientDto());

            _mockLogSvc.Setup(s => s.LogPatientDeletion(It.IsAny<PatientDto>()))
                .ReturnsAsync(new LogDto());


            var result = await _controller.ConfirmPatientDeletion(Guid.NewGuid().ToString());


            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            // Assert the value is not null
            Assert.NotNull(okResult.Value);

            // Use reflection to get the "message" property value
            var returnValue = okResult.Value.GetType().GetProperty("message")?.GetValue(okResult.Value, null);

            // Assert the "message" property value
            Assert.NotNull(returnValue);
            Assert.Equal("Confirmation Accepted. Patient Deletion is schedule to happen within the GRPD Parameters.", returnValue.ToString());
        }

        [Fact]
        public async Task CreatePatientProfile_Failure_BadAuth()
        {

            //Arrange
            PatientRegistrationDto examplePatient = new PatientRegistrationDto
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

            TokenDto tokenDto =
                new TokenDto(
                    Guid.NewGuid().ToString(),
                    TokenType.GENERAL_ACCESS.ToString(), //WRONG TYPE
                    DateTime.Now.AddDays(1).ToString(),
                    Guid.NewGuid().ToString());
            _mockTokenSvc.Setup(s => s.GetByIdAsync(It.IsAny<TokenId>()))
                .ReturnsAsync(tokenDto);

            //Act
            var result = await _controller.CreatePatientProfile(tokenDto.TokenId, examplePatient);

            //Assert
            var okResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var returnValue = Assert.IsType<string>(okResult.Value);

            Assert.Equal("ACCESS TO RESOURCE DENIED.", returnValue);

        }

    }
}
