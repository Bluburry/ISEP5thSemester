using DDDSample1.Domain.HospitalStaff;
using DDDSample1.Domain.Users;
using DDDSample1.DTO;
using Moq;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using DDDSample1.Controllers;
using DDDSample1.Domain.Shared;
using DDDSample1.Domain.ValueObjects;
using DDDSample1.DTO.LoginAttemptTrackers;
using DDDSample1.Domain.Tokens;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace DDDNetCore.Test.IntegrationTest
{
    public class US07IntegrationTest
    {
        private readonly Mock<IUserRepository> _mockUserRepo;
        private readonly Mock<ILoginAttemptTrackerRepository> _mockLoginAttemptRepo;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<TokenService> _mockTokenSvc;
        private readonly Mock<UserService> _mockUserSvc;
        private readonly LoginService _loginService;
        private readonly LoginController _loginController;
        private readonly Mock<User> _mockUser;
        private readonly Mock<HttpContext> _mockHTTP;
        private readonly Mock<IAuthenticationService> _mockAuthService;


        public US07IntegrationTest()
        {
            _mockUserRepo = new Mock<IUserRepository>();
            _mockLoginAttemptRepo = new Mock<ILoginAttemptTrackerRepository>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockTokenSvc = new Mock<TokenService>();
            _mockUserSvc = new Mock<UserService>();
            _mockHTTP = new Mock<HttpContext>();
            _mockAuthService = new Mock<IAuthenticationService>();

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
        public async Task LoginIAM_Sucess_AccountLogsIn()
        {
            // Arrange
            var username = "testuser@example.com";
            var password = "IAM-104687050750708666891";
            var credentials = new LoginCredentialsDto(username, password);

            _mockTokenSvc
                .Setup(x => x.CreateAuthToken(It.IsAny<LoginCredentialsDto>()))
                .ReturnsAsync(new TokenDto("token123", "", "", "testuser@example.com"));

            var authResult = AuthenticateResult.Success(new AuthenticationTicket(
                new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, "104687050750708666891"),
                    new Claim(ClaimTypes.Name, "Test User"),
                    new Claim(ClaimTypes.GivenName, "User"),
                    new Claim(ClaimTypes.Surname, "Test"),
                    new Claim(ClaimTypes.Email, "testuser@example.com")
                }, CookieAuthenticationDefaults.AuthenticationScheme)),
                new AuthenticationProperties(),
                CookieAuthenticationDefaults.AuthenticationScheme));

            // Set up the AuthenticateAsync method to return the mock result
            _mockAuthService.Setup(x => x.AuthenticateAsync(
                It.IsAny<HttpContext>(),
                CookieAuthenticationDefaults.AuthenticationScheme))
                .ReturnsAsync(authResult);

            // Set up the HttpContext to return the mock AuthenticationService
            var mockServiceProvider = new Mock<IServiceProvider>();
            mockServiceProvider.Setup(x => x.GetService(typeof(IAuthenticationService)))
                .Returns(_mockAuthService.Object);
            _mockHTTP.Setup(x => x.RequestServices)
                .Returns(mockServiceProvider.Object);

            // Assign the mock HttpContext to the controller context
            _loginController.ControllerContext = new ControllerContext
            {
                HttpContext = _mockHTTP.Object
            };

            // Mock user
            var mockUser = new User(username, password, UserRole.PATIENT);
            mockUser.Activate();
            _mockUserRepo.Setup(repo => repo.GetByIdAsync(It.IsAny<Username>())).ReturnsAsync(mockUser);

            // Act
            var result = await _loginController.LoginIAMResponse();

            // Assert
            var redirectResult = Assert.IsType<RedirectResult>(result); // Ensure it's a RedirectResult
            Assert.StartsWith("http://localhost:4200", redirectResult.Url); // Check the base URL

            // Extract the token from the query string
            var uri = new Uri(redirectResult.Url);
            var query = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(uri.Query);
            Assert.True(query.ContainsKey("token")); // Ensure token exists
            Assert.Equal("token123", query["token"]); // Verify the token value

            // Verify unit of work commit was called
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task LoginIAM_Failure_WithInvalidPassword()
        {
            // Arrange
            var username = "testuser@example.com";
            var password = "IAM-104687050750708666000";
            var credentials = new LoginCredentialsDto(username, password);

            _mockTokenSvc
                .Setup(x => x.CreateAuthToken(It.IsAny<LoginCredentialsDto>()))
                .ReturnsAsync(new TokenDto("token123", "", "", "testuser@example.com"));

            var authResult = AuthenticateResult.Success(new AuthenticationTicket(
                new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, "104687050750708666891"),
                    new Claim(ClaimTypes.Name, "Test User"),
                    new Claim(ClaimTypes.GivenName, "User"),
                    new Claim(ClaimTypes.Surname, "Test"),
                    new Claim(ClaimTypes.Email, "testuser@example.com")
                }, CookieAuthenticationDefaults.AuthenticationScheme)),
                new AuthenticationProperties(),
                CookieAuthenticationDefaults.AuthenticationScheme));

            // Set up the AuthenticateAsync method to return the mock result
            _mockAuthService.Setup(x => x.AuthenticateAsync(
                It.IsAny<HttpContext>(),
                CookieAuthenticationDefaults.AuthenticationScheme))
                .ReturnsAsync(authResult);

            // Set up the HttpContext to return the mock AuthenticationService
            var mockServiceProvider = new Mock<IServiceProvider>();
            mockServiceProvider.Setup(x => x.GetService(typeof(IAuthenticationService)))
                .Returns(_mockAuthService.Object);
            _mockHTTP.Setup(x => x.RequestServices)
                .Returns(mockServiceProvider.Object);

            // Assign the mock HttpContext to the controller context
            _loginController.ControllerContext = new ControllerContext
            {
                HttpContext = _mockHTTP.Object
            };

            // Mock user
            var mockUser = new User(username, password, UserRole.PATIENT);
            mockUser.Activate();
            _mockUserRepo.Setup(repo => repo.GetByIdAsync(It.IsAny<Username>())).ReturnsAsync(mockUser);

            // Act
            var result = await _loginController.LoginIAMResponse();

            // Assert
            var redirectResult = Assert.IsType<RedirectResult>(result); // Ensure it's a RedirectResult
            Assert.StartsWith("http://localhost:4200", redirectResult.Url); // Check the base URL

            // Extract the token from the query string
            var uri = new Uri(redirectResult.Url);
            var query = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(uri.Query);
            Assert.True(query.ContainsKey("token")); // Ensure token exists
            Assert.Equal("token123", query["token"]); // Verify the token value

            // Verify unit of work commit was called
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task LoginIAM_Failure_WithInvalidEmail()
        {
            // Arrange
            var username = "random@email.com";
            var password = "IAM-104687050750708666891";
            var credentials = new LoginCredentialsDto(username, password);

            _mockTokenSvc
                .Setup(x => x.CreateAuthToken(It.IsAny<LoginCredentialsDto>()))
                .ReturnsAsync(new TokenDto("token123", "", "", "random@email.com"));

            var authResult = AuthenticateResult.Success(new AuthenticationTicket(
                new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, "104687050750708666891"),
                    new Claim(ClaimTypes.Name, "Test User"),
                    new Claim(ClaimTypes.GivenName, "User"),
                    new Claim(ClaimTypes.Surname, "Test"),
                    new Claim(ClaimTypes.Email, "testuser@example.com")
                }, CookieAuthenticationDefaults.AuthenticationScheme)),
                new AuthenticationProperties(),
                CookieAuthenticationDefaults.AuthenticationScheme));

            // Set up the AuthenticateAsync method to return the mock result
            _mockAuthService.Setup(x => x.AuthenticateAsync(
                It.IsAny<HttpContext>(),
                CookieAuthenticationDefaults.AuthenticationScheme))
                .ReturnsAsync(authResult);

            // Set up the HttpContext to return the mock AuthenticationService
            var mockServiceProvider = new Mock<IServiceProvider>();
            mockServiceProvider.Setup(x => x.GetService(typeof(IAuthenticationService)))
                .Returns(_mockAuthService.Object);
            _mockHTTP.Setup(x => x.RequestServices)
                .Returns(mockServiceProvider.Object);

            // Assign the mock HttpContext to the controller context
            _loginController.ControllerContext = new ControllerContext
            {
                HttpContext = _mockHTTP.Object
            };

            // Mock user
            var mockUser = new User(username, password, UserRole.PATIENT);

            /// Act
            var result = await _loginController.LoginIAMResponse();

            // Assert
            var redirectResult = Assert.IsType<RedirectResult>(result); // Ensure it's a RedirectResult
            Assert.StartsWith("http://localhost:4200", redirectResult.Url); // Check the base URL

            // Extract the token from the query string
            var uri = new Uri(redirectResult.Url);
            var query = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(uri.Query);
            Assert.True(query.ContainsKey("error")); // Ensure token exists
            Assert.Equal("user-not-found", query["error"]); // Verify the token value
            // Verify unit of work commit was never called
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Never);
        }

    }
}
