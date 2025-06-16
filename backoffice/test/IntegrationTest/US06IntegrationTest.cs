using DDDSample1.Domain.HospitalStaff;
using DDDSample1.Domain.Users;
using DDDSample1.DTO;
using DDDSample1.Domain.LoginAttemptTrackers;
using Moq;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using DDDSample1.Controllers;
using DDDSample1.Domain.Shared;
using DDDSample1.Domain.ValueObjects;
using DDDSample1.DTO.LoginAttemptTrackers;
using DDDSample1.Domain.Tokens;

namespace DDDNetCore.Test.IntegrationTest
{
    public class US06IntegrationTest
    {
        private readonly Mock<IUserRepository> _mockUserRepo;
        private readonly Mock<ITokenRepository> _mockTokenRepo;
        private readonly Mock<ILoginAttemptTrackerRepository> _mockLoginAttemptRepo;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<TokenService> _mockTokenSvc;
        private readonly Mock<UserService> _mockUserSvc;
        private readonly LoginService _loginService;
        private readonly LoginController _loginController;
        private readonly Mock<User> _mockUser;


        public US06IntegrationTest()
        {
            _mockUserRepo = new Mock<IUserRepository>();
            _mockTokenRepo = new Mock<ITokenRepository>();
            _mockLoginAttemptRepo = new Mock<ILoginAttemptTrackerRepository>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockTokenSvc = new Mock<TokenService>();
            _mockUserSvc = new Mock<UserService>();

            _mockUser = new Mock<User>();

            // Set up the mocked properties for the User
            _mockUser.Setup(u => u.ActivationStatus).Returns(ActivationStatus.ACTIVATED);
            _mockUser.Setup(u => u.EmailAddress).Returns(new EmailAddress("user@example.com"));
            _mockUser.Setup(u => u.Password).Returns(new Password("!Password123"));
            _mockUser.Setup(u => u.Role).Returns(UserRole.STAFF);


            // Pass the mocked dependencies to the LoginService
            _loginService = new LoginService(_mockUnitOfWork.Object, _mockUserRepo.Object, _mockLoginAttemptRepo.Object);
            _loginController = new LoginController(_mockTokenSvc.Object, _mockUserSvc.Object, _loginService);
        }

        [Fact]
        public async Task Login_WithMAXATTEMPTS_BLOCKS()
        {
            // Arrange

            // Arrange
            var username = "testuser@example.com";
            var password = "!1WrongPassword";
            var credentials = new LoginCredentialsDto(username, "!Ba12babababaa");

            _mockTokenSvc
                .Setup(x => x.CreateAuthToken(It.IsAny<LoginCredentialsDto>()))
                .ReturnsAsync(new TokenDto("token123", TokenType.ADMIN_AUTH_TOKEN.ToString(), "", "testuser@example.com"));

            // Mock user
            var mockUser = new User(username, password, UserRole.PATIENT);
            mockUser.Activate();
            _mockUserRepo.Setup(repo => repo.GetByIdAsync(It.IsAny<Username>())).ReturnsAsync(mockUser);

            // Mock login tracker with valid logins
            var loginTracker = LoginAttemptTrackerFactory.Create(mockUser.Id, 5);
            _mockLoginAttemptRepo.Setup(repo => repo.GetByIdAsync(It.IsAny<Username>())).ReturnsAsync(loginTracker);

            // Act
            var result = await _loginController.Login(credentials);
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<LoginOutputDto>(okResult.Value);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(LoginResult.AccountLocked.ToString(), returnValue.Result);

            // Verify unit of work commit was called
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task Login_WithInvalidCredentials_ReturnsFailure()
        {
            // Arrange
            var username = "testuser@example.com";
            var password = "!1WrongPassword";
            var credentials = new LoginCredentialsDto(username, "!Ba12babababaa");

            _mockTokenSvc
                .Setup(x => x.CreateAuthToken(It.IsAny<LoginCredentialsDto>()))
                .ReturnsAsync(new TokenDto("token123", TokenType.ADMIN_AUTH_TOKEN.ToString(), "", "testuser@example.com"));

            // Mock user
            var mockUser = new User(username, password, UserRole.PATIENT);
            mockUser.Activate();
            _mockUserRepo.Setup(repo => repo.GetByIdAsync(It.IsAny<Username>())).ReturnsAsync(mockUser);

            // Mock login tracker with valid logins
            var loginTracker = LoginAttemptTrackerFactory.Create(mockUser.Id, 3);
            _mockLoginAttemptRepo.Setup(repo => repo.GetByIdAsync(It.IsAny<Username>())).ReturnsAsync(loginTracker);

            // Act
            var result = await _loginController.Login(credentials);
            var okResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var returnValue = Assert.IsType<string>(okResult.Value);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Login Credentials do not match.", returnValue);

            // Verify unit of work commit was called
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task Login_AccountLogsIn()
        {
            // Arrange
            var username = "testuser@example.com";
            var password = "!1WrongPassword";
            var credentials = new LoginCredentialsDto(username, password);

            _mockTokenSvc
                .Setup(x => x.CreateAuthToken(It.IsAny<LoginCredentialsDto>()))
                .ReturnsAsync(new TokenDto("lb52JysSEupTAMaRzl9b/ALvhJ++Aft0hwI9WW1NPQq/1akfhmFcHNZ5Q/r76r7a4e6aWVOywPPOLiQRlJsqvwOv2GSNuYMIEOPPbx2P4Ualxobok8n0UYSwEi5GXBm5uc4eOq8i6nB+t6fSnyTcHsBCX2xUD80dLioA/4eBezTmIjAZf/aVt/1aqEO4ztJEb6ybS85prABB42k45mlO0zRZa3ABbXzGcdu4yEKMlLGRDleFqAf7amzDPJD2IPExoKbYw+82fjLgzmT2vyGgdxenIMKqhWDFnRh52GnVQZIUH4PhH9hdNNfZr+vfojvxma5OuB0aZ5dOh0Th370NMQ==", TokenType.ADMIN_AUTH_TOKEN.ToString(), "21-02-2027", "testuser@example.com"));

            // Mock user
            var mockUser = new User(username, password, UserRole.PATIENT);
            mockUser.Activate();
            _mockUserRepo.Setup(repo => repo.GetByIdAsync(It.IsAny<Username>())).ReturnsAsync(mockUser);

            // Mock login tracker with normal logins
            var loginTracker = LoginAttemptTrackerFactory.Create(mockUser.Id, 2);
            _mockLoginAttemptRepo.Setup(repo => repo.GetByIdAsync(It.IsAny<Username>())).ReturnsAsync(loginTracker);

            // Act
            var result = await _loginController.Login(credentials);
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<LoginOutputDto>(okResult.Value);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(LoginResult.Success.ToString(), returnValue.Result);

            // Verify unit of work commit was called
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
        }
    }
}
