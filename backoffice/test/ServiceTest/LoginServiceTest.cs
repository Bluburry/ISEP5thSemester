using System;
using System.Threading.Tasks;
using Moq;
using Xunit;
using DDDSample1.Domain.HospitalStaff;
using DDDSample1.Domain.Users;
using DDDSample1.Domain.LoginAttemptTrackers;
using DDDSample1.Domain.ValueObjects;
using DDDSample1.DTO.LoginAttemptTrackers;
using DDDSample1.DTO;
using DDDSample1.Domain.Shared;

namespace DDDSample1.Domain.HospitalStaff.Tests
{
    public class LoginServiceTests
    {
        private readonly Mock<IUserRepository> _mockUserRepo;
        private readonly Mock<ILoginAttemptTrackerRepository> _mockLoginRepo;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly LoginService _loginService;

        public LoginServiceTests()
        {
            _mockUserRepo = new Mock<IUserRepository>();
            _mockLoginRepo = new Mock<ILoginAttemptTrackerRepository>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();

            _loginService = new LoginService(
                _mockUnitOfWork.Object, 
                _mockUserRepo.Object, 
                _mockLoginRepo.Object
            );
        }

        [Fact]
        public async Task Login_Success_ReturnsSuccessResult()
        {
            // Arrange
            var userId = new Username("testuser@example.com");
            var password = new Password("fjdsfjTest@123");
            var user = new User("testuser@example.com", "fjdsfjTest@123", UserRole.PATIENT);
            user.Activate();
            var credentials = new LoginCredentialsDto(userId.AsString(), password.ToString());

            _mockUserRepo.Setup(repo => repo.GetByIdAsync(It.IsAny<Username>())).ReturnsAsync(user);
            _mockLoginRepo.Setup(repo => repo.GetByIdAsync(It.IsAny<Username>())).ReturnsAsync((LoginAttemptTracker)null); // No login attempts yet

            // Act
            var result = await _loginService.Login(credentials);

            // Assert
            Assert.Equal(LoginResult.Success.ToString(), result.Result);
            _mockLoginRepo.Verify(repo => repo.AddAsync(It.IsAny<LoginAttemptTracker>()), Times.Once);
            _mockUnitOfWork.Verify(uow => uow.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task Login_Failure_ReturnsFailureResultAndIncrementsAttemptCounter()
        {
            // Arrange
            var userId = new Username("testuser@example.com");
            var user = new User("testuser@example.com", "!sjdfTest@123", UserRole.PATIENT);
            user.Activate();
            var credentials = new LoginCredentialsDto(userId.AsString(), "!1WrongPassword");

            var loginAttemptTracker = LoginAttemptTrackerFactory.Create(user.Id, 2); // Already 2 failed attempts

            _mockUserRepo.Setup(repo => repo.GetByIdAsync(It.IsAny<Username>())).ReturnsAsync(user);
            _mockLoginRepo.Setup(repo => repo.GetByIdAsync(It.IsAny<Username>())).ReturnsAsync(loginAttemptTracker);

            // Act
            var result = await _loginService.Login(credentials);

            // Assert
            Assert.Equal(LoginResult.Failure.ToString(), result.Result);
            _mockLoginRepo.Verify(repo => repo.Update(It.IsAny<LoginAttemptTracker>()), Times.Once);
            _mockUnitOfWork.Verify(uow => uow.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task Login_AccountLocked_AfterMaximumFailedAttempts()
        {
            // Arrange
            var userId = new Username("testuser@example.com");
            var user = new User("testuser@example.com", "Test@123", UserRole.PATIENT);
            var credentials = new LoginCredentialsDto(userId.AsString(), "WrongPassword");

            var loginAttemptTracker = LoginAttemptTrackerFactory.Create(user.Id, 5); // Maximum failed attempts reached

            _mockUserRepo.Setup(repo => repo.GetByIdAsync(It.IsAny<Username>())).ReturnsAsync(user);
            _mockLoginRepo.Setup(repo => repo.GetByIdAsync(It.IsAny<Username>())).ReturnsAsync(loginAttemptTracker);

            // Act
            var result = await _loginService.Login(credentials);

            // Assert
            Assert.Equal("DEACTIVATED", result.Result);
            _mockLoginRepo.Verify(repo => repo.Update(It.IsAny<LoginAttemptTracker>()), Times.Never);
            _mockUnitOfWork.Verify(uow => uow.CommitAsync(), Times.Never);
        }

        [Fact]
        public async Task Login_DeactivatedUser_ReturnsDeactivatedResult()
        {
            // Arrange
            var userId = new Username("deactivateduser@example.com");
            var user = new User("testuser@example.com", "Test@123", UserRole.PATIENT);
            var credentials = new LoginCredentialsDto(userId.AsString(), "Test@123");

            _mockUserRepo.Setup(repo => repo.GetByIdAsync(It.IsAny<Username>())).ReturnsAsync(user);

            // Act
            var result = await _loginService.Login(credentials);

            // Assert
            Assert.Equal(LoginResult.DEACTIVATED.ToString(), result.Result);
            _mockLoginRepo.Verify(repo => repo.GetByIdAsync(It.IsAny<Username>()), Times.Never); // Shouldn't check login attempts
            _mockUnitOfWork.Verify(uow => uow.CommitAsync(), Times.Never); // Shouldn't commit anything
        }

        [Fact]
        public async Task Login_UserNotFound_ThrowsException()
        {
            // Arrange
            var credentials = new LoginCredentialsDto("nonexistentuser@example.com", "Password");

            _mockUserRepo.Setup(repo => repo.GetByIdAsync(It.IsAny<Username>())).ReturnsAsync((User)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _loginService.Login(credentials));
            Assert.Contains("User not found", exception.Message);
        }
    }
}
