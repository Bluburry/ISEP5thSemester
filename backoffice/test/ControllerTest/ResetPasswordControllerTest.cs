using DDDSample1.AppServices;
using DDDSample1.Controllers;
using DDDSample1.Domain.Tokens;
using DDDSample1.Domain.Users;
using DDDSample1.Domain.ValueObjects;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DDDNetCore.test.ControllerTest
{

    public class ResetPasswordControllerTest
    {

        private readonly Mock<UserService> _mockUserSvc;
        private readonly Mock<TokenService> _mockTokenSvc;
        private readonly Mock<PasswordActivationService> _mockPassSvc;
        private readonly ResetPasswordController _controller;
        private User user;
         public ResetPasswordControllerTest()
        {
            _mockTokenSvc = new Mock<TokenService>();
            _mockUserSvc = new Mock<UserService>();
            _mockPassSvc = new Mock<PasswordActivationService>();

            _controller = new ResetPasswordController(_mockPassSvc.Object, _mockTokenSvc.Object, _mockUserSvc.Object);

            user = new User(
                "user@example.com",
                "!Password123",
                UserRole.PATIENT
                );
            user.Activate();
        }

        [Fact]
        public async Task ResetPassword_Success_WithGoodValues()
        {
            string newPass = "!NewPassword21New";
            var tokenDto = new TokenDto((new TokenId(Guid.NewGuid())).AsString(), TokenType.PASSWORD_RESET_TOKEN.ToString(), DateTime.Now.AddDays(2).ToString(), "12345");
            User newUser = user;
            newUser.Password = new Password(newPass);
            _mockTokenSvc.Setup(s => s.GetByIdAsync(It.IsAny<TokenId>()))
                .ReturnsAsync(tokenDto);
            _mockUserSvc.Setup(s => s.GetByIdAsync(It.IsAny<Username>()))
                .ReturnsAsync(user.ToDto());

            _mockTokenSvc.Setup(s => s.RemoveToken(It.IsAny<string>()));

            _mockPassSvc.Setup(s => s.ResetPassword(It.IsAny<string>(), It.IsAny<UserDto>()))
                .ReturnsAsync(newUser.ToDto());

            // Act
            var result = await _controller.ResetPassword(newPass, tokenDto.TokenId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<UserDto>(okResult.Value);

            Assert.Equal(returnValue.ToString(), newUser.ToDto().ToString());

        }

        [Fact]
        public async Task ResetPassword_Aunauthorized_WithBadToken()
        {
            string newPass = "!NewPassword21New";
            var tokenDto = new TokenDto((new TokenId(Guid.NewGuid())).AsString(), TokenType.GENERAL_ACCESS.ToString(), DateTime.Now.AddDays(2).ToString(), "12345");
            User newUser = user;
            newUser.Password = new Password(newPass);
            _mockTokenSvc.Setup(s => s.GetByIdAsync(It.IsAny<TokenId>()))
                .ReturnsAsync(tokenDto);


            // Act
            var result = await _controller.ResetPassword(newPass, tokenDto.TokenId);

            // Assert
            var okResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var returnValue = Assert.IsType<string>(okResult.Value);

            Assert.Equal("Invalid Token", returnValue);
        }

    }
}