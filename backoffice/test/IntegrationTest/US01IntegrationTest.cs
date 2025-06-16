using DDDSample1.Controllers;
using DDDSample1.Domain.Shared;
using DDDSample1.Domain.Tokens;
using DDDSample1.Domain.Users;
using DDDSample1.Domain.ValueObjects;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DDDNetCore.test.IntegrationTest
{
    public class US01IntegrationTest
    {
        private readonly Mock<TokenService> _mockTokenSvc;
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;

        private readonly UserService _userService;
        private RegisterBackofficeController _controller;

        private User user;

        public US01IntegrationTest()
        {
            _mockTokenSvc = new Mock<TokenService>();
            _mockUserRepository = new Mock<IUserRepository>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();

            _userService = new UserService(_mockUnitOfWork.Object, _mockUserRepository.Object, _mockTokenSvc.Object);
            _controller = new RegisterBackofficeController(_userService, _mockTokenSvc.Object);

            user = new User(
                "someEmail@domain.com",
                "!Password123",
                UserRole.PATIENT
                );
        }

        [Fact]
        public async Task RegisterBackoffice_Success_WithGoodCredentials()
        {
            //Arrange
            Token token = new Token(
                    new TokenId(Guid.NewGuid()),
                    DateTime.Now.AddDays(1),
                    user,
                    TokenType.ADMIN_AUTH_TOKEN
            );

            //Can't be tested, but necessary to send the e-mail 
            Token tokenPass = new Token(
                    new TokenId(Guid.NewGuid()),
                    DateTime.Now.AddDays(1),
                    user,
                    TokenType.PASSWORD_RESET_TOKEN
            );

            _mockTokenSvc.Setup(s => s.GetByIdAsync(It.IsAny<TokenId>()))
                .ReturnsAsync(token.ToDto());

            _mockTokenSvc.Setup(s => s.GeneratePasswordValidationTokenAsync(It.IsAny<UserDto>()))
                .ReturnsAsync(tokenPass.ToDto());

            _mockUserRepository.Setup(s=>s.AddAsync(It.IsAny<User>()))
                .ReturnsAsync(user);

            // Act
            var result = await _controller.RegisterBackoffice("someEmail@domain.com", token.Id.AsString());

            // Assert
            var okResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnValue = Assert.IsType<UserDto>(okResult.Value);

            Assert.Equal("someEmail@domain.com", returnValue.EmailAddress);

            _mockTokenSvc.Verify(s => s.GetByIdAsync(It.IsAny<TokenId>()), Times.Once);
            _mockTokenSvc.Verify(s => s.GeneratePasswordValidationTokenAsync(It.IsAny<UserDto>()), Times.Once);
            _mockUserRepository.Verify(s => s.AddAsync(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task RegisterBackoffice_Failure_WithWrongToken()
        {
            //Arrange
            Token token = new Token(
                    new TokenId(Guid.NewGuid()),
                    DateTime.Now.AddDays(1),
                    user,
                    TokenType.PATIENT_AUTH_TOKEN
            );

            //Can't be tested, but necessary to send the e-mail 
            Token tokenPass = new Token(
                    new TokenId(Guid.NewGuid()),
                    DateTime.Now.AddDays(1),
                    user,
                    TokenType.PASSWORD_RESET_TOKEN
            );

            _mockTokenSvc.Setup(s => s.GetByIdAsync(It.IsAny<TokenId>()))
                .ReturnsAsync(token.ToDto());

            _mockTokenSvc.Setup(s => s.GeneratePasswordValidationTokenAsync(It.IsAny<UserDto>()))
                .ReturnsAsync(tokenPass.ToDto());

            _mockUserRepository.Setup(s=>s.AddAsync(It.IsAny<User>()))
                .ReturnsAsync(user);

            // Act
            var result = await _controller.RegisterBackoffice("someEmail@domain.com", token.Id.AsString());

            // Assert
            var okResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var returnValue = Assert.IsType<string>(okResult.Value);

            _mockTokenSvc.Verify(s => s.GetByIdAsync(It.IsAny<TokenId>()), Times.Once);
            _mockTokenSvc.Verify(s => s.GeneratePasswordValidationTokenAsync(It.IsAny<UserDto>()), Times.Never);
            _mockUserRepository.Verify(s => s.AddAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task RegisterBackoffice_Failure_WithoutEmail()
        {
            //Arrange
            Token token = new Token(
                    new TokenId(Guid.NewGuid()),
                    DateTime.Now.AddDays(1),
                    user,
                    TokenType.PATIENT_AUTH_TOKEN
            );

            //Can't be tested, but necessary to send the e-mail 
            Token tokenPass = new Token(
                    new TokenId(Guid.NewGuid()),
                    DateTime.Now.AddDays(1),
                    user,
                    TokenType.PASSWORD_RESET_TOKEN
            );

            _mockTokenSvc.Setup(s => s.GetByIdAsync(It.IsAny<TokenId>()))
                .ReturnsAsync(token.ToDto());

            _mockTokenSvc.Setup(s => s.GeneratePasswordValidationTokenAsync(It.IsAny<UserDto>()))
                .ReturnsAsync(tokenPass.ToDto());

            _mockUserRepository.Setup(s=>s.AddAsync(It.IsAny<User>()))
                .ReturnsAsync(user);

            // Act
            var result = await _controller.RegisterBackoffice(null, token.Id.AsString());

            // Assert
            var okResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var returnValue = Assert.IsType<string>(okResult.Value);

            _mockTokenSvc.Verify(s => s.GetByIdAsync(It.IsAny<TokenId>()), Times.Once);
            _mockTokenSvc.Verify(s => s.GeneratePasswordValidationTokenAsync(It.IsAny<UserDto>()), Times.Never);
            _mockUserRepository.Verify(s => s.AddAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task RegisterBackoffice_Failure_WithEmailIncorrect()
        {
            //Arrange
            Token token = new Token(
                    new TokenId(Guid.NewGuid()),
                    DateTime.Now.AddDays(1),
                    user,
                    TokenType.ADMIN_AUTH_TOKEN
            );

            //Can't be tested, but necessary to send the e-mail 
            Token tokenPass = new Token(
                    new TokenId(Guid.NewGuid()),
                    DateTime.Now.AddDays(1),
                    user,
                    TokenType.PASSWORD_RESET_TOKEN
            );

            _mockTokenSvc.Setup(s => s.GetByIdAsync(It.IsAny<TokenId>()))
                .ReturnsAsync(token.ToDto());

            _mockTokenSvc.Setup(s => s.GeneratePasswordValidationTokenAsync(It.IsAny<UserDto>()))
                .ReturnsAsync(tokenPass.ToDto());

            _mockUserRepository.Setup(s=>s.AddAsync(It.IsAny<User>()))
                .ReturnsAsync(user);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _controller.RegisterBackoffice("notAnEmail", token.Id.AsString());
            });

            Assert.Equal("Invalid email address format. (Parameter 'value')", exception.Message);
            Assert.Equal("value", exception.ParamName);

            _mockTokenSvc.Verify(s => s.GetByIdAsync(It.IsAny<TokenId>()), Times.Once);
            _mockTokenSvc.Verify(s => s.GeneratePasswordValidationTokenAsync(It.IsAny<UserDto>()), Times.Never);
            _mockUserRepository.Verify(s => s.AddAsync(It.IsAny<User>()), Times.Never);
        }
    }
}