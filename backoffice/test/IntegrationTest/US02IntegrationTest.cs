using DDDSample1.AppServices;
using DDDSample1.Controllers;
using DDDSample1.Domain.Shared;
using DDDSample1.Domain.Tokens;
using DDDSample1.Domain.Users;
using DDDSample1.Domain.ValueObjects;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DDDNetCore.test.IntegrationTest
{
    public class US02IntegrationTest
    {
        private readonly Mock<ITokenRepository> _mockTokenRepository;
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;

        private readonly TokenService _tokenSvc;
        private readonly PasswordActivationService _passSvc;
        private readonly UserService _userService;
        private ResetPasswordController _controller;

        private User user;

        public US02IntegrationTest()
        {
            _mockTokenRepository = new Mock<ITokenRepository>();
            _mockUserRepository = new Mock<IUserRepository>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();


            _tokenSvc = new TokenService(_mockUnitOfWork.Object, _mockUserRepository.Object, _mockTokenRepository.Object);
            _passSvc = new PasswordActivationService(_mockUnitOfWork.Object, _mockUserRepository.Object, _tokenSvc);
            _userService = new UserService(_mockUnitOfWork.Object, _mockUserRepository.Object, _tokenSvc);
            
            
            _controller = new ResetPasswordController(_passSvc, _tokenSvc, _userService);


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
            //Arrange
            Token token = new Token(
                    new TokenId(Guid.NewGuid()),
                    DateTime.Now.AddDays(1),
                    user,
                    TokenType.PASSWORD_RESET_TOKEN
            );

            string newPass = "!NewPassword21New";
            User newUser = user;

            newUser.Password = new Password(newPass);
            _mockTokenRepository.Setup(s => s.GetByIdAsync(It.IsAny<TokenId>()))
                .ReturnsAsync(token);

            _mockUserRepository.Setup(s => s.GetByIdAsync(It.IsAny<Username>()))
                .ReturnsAsync(user);

            _mockUserRepository.Setup(s => s.GetByIdAsync(It.IsAny<Username>()))
                .ReturnsAsync(newUser);

            _mockUserRepository.Setup(s => s.Update(It.IsAny<User>()))
                .Returns(newUser);

            _mockTokenRepository.Setup(s => s.Remove(It.IsAny<Token>()));

            // Act
            var result = await _controller.ResetPassword(newPass, token.Id.AsString());

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<UserDto>(okResult.Value);

            Assert.Equal(returnValue.ToString(), newUser.ToDto().ToString());
        }

        [Fact]
        public async Task ResetPassword_Failure_WithBadToken()
        {
            //Arrange
            Token token = new Token(
                new TokenId(Guid.NewGuid()),
                DateTime.Now.AddDays(1),
                user,
                TokenType.GENERAL_ACCESS
                );

            string newPass = "!NewPassword21New";
            var tokenDto = new TokenDto(token.Id.AsString(), TokenType.PASSWORD_RESET_TOKEN.ToString(), DateTime.Now.AddDays(2).ToString(), "12345");
            User newUser = user;
            newUser.Password = new Password(newPass);
            _mockTokenRepository.Setup(s => s.GetByIdAsync(It.IsAny<TokenId>()))
                .ReturnsAsync(token);

            _mockTokenRepository.Setup(s => s.Remove(It.IsAny<Token>()));


            // Act
            var result = await _controller.ResetPassword(newPass, token.Id.AsString());

            // Assert
            var okResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var returnValue = Assert.IsType<string>(okResult.Value);

            Assert.Equal("Invalid Token", returnValue);
        }

        [Fact]
        public async Task ResetPassword_Failure_WithDeactivatedUser()
        {
            user.Deactivate();
            //Arrange
            Token token = new Token(
                new TokenId(Guid.NewGuid()),
                DateTime.Now.AddDays(1),
                user,
                TokenType.PASSWORD_RESET_TOKEN
                );

            string newPass = "!NewPassword21New";
            var tokenDto = new TokenDto(token.Id.AsString(), TokenType.PASSWORD_RESET_TOKEN.ToString(), DateTime.Now.AddDays(2).ToString(), "12345");
            User newUser = user;
            newUser.Password = new Password(newPass);
            _mockTokenRepository.Setup(s => s.GetByIdAsync(It.IsAny<TokenId>()))
                .ReturnsAsync(token);

            _mockTokenRepository.Setup(s => s.Remove(It.IsAny<Token>()));
            _mockUserRepository.Setup(s => s.GetByIdAsync(It.IsAny<Username>()))
                .ReturnsAsync(user);

            // Act
            var result = await _controller.ResetPassword(newPass, token.Id.AsString());

            // Assert
            var okResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var returnValue = Assert.IsType<string>(okResult.Value);

            Assert.Equal("User Not Activated", returnValue);
        }

    }
}
