using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using DDDSample1.Controllers;
using DDDSample1.Domain.Tokens;
using DDDSample1.Domain.Users;
using DDDSample1.AppServices;
using DDDSample1.DTO;
using DDDSample1.DTO.LoginAttemptTrackers;
using DDDSample1.Domain.HospitalStaff;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using DDDSample1.Domain.ValueObjects;

namespace DDDSample1.Tests.Controllers
{
    public class LoginControllerTests
    {
        private readonly Mock<PasswordActivationService> _mockPasswordService;
        private readonly Mock<TokenService> _mockTokenService;
        private readonly Mock<UserService> _mockUserService;
        private readonly Mock<LoginService> _mockLoginService;
        private readonly LoginController _controller;
        private readonly Mock<User> _mockUser;

        public LoginControllerTests()
        {
            _mockPasswordService = new Mock<PasswordActivationService>();
            _mockTokenService = new Mock<TokenService>();
            _mockUserService = new Mock<UserService>();
            _mockLoginService = new Mock<LoginService>();

            _mockUser = new Mock<User>();

            // Set up the mocked properties for the User
            _mockUser.Setup(u => u.ActivationStatus).Returns(ActivationStatus.DEACTIVATED);
            _mockUser.Setup(u => u.EmailAddress).Returns(new EmailAddress("user@example.com"));
            _mockUser.Setup(u => u.Password).Returns(new Password("!Password123"));
            _mockUser.Setup(u => u.Role).Returns(UserRole.STAFF);

            _controller = new LoginController(
                _mockTokenService.Object,
                _mockUserService.Object,
                _mockLoginService.Object
            );
        }

        [Fact]
        public async Task Login_ReturnsOk_WithValidCredentials()
        {
            // Arrange
            var loginCredentialsDto = new LoginCredentialsDto("testuser", "!CoolPassword22");

            var loginOutputDto = new LoginOutputDto
            {
                Result = LoginResult.Success.ToString(),
                Token = "lb52JysSEupTAMaRzl9b/ALvhJ++Aft0hwI9WW1NPQq/1akfhmFcHNZ5Q/r76r7a4e6aWVOywPPOLiQRlJsqvwOv2GSNuYMIEOPPbx2P4Ualxobok8n0UYSwEi5GXBm5uc4eOq8i6nB+t6fSnyTcHsBCX2xUD80dLioA/4eBezTmIjAZf/aVt/1aqEO4ztJEb6ybS85prABB42k45mlO0zRZa3ABbXzGcdu4yEKMlLGRDleFqAf7amzDPJD2IPExoKbYw+82fjLgzmT2vyGgdxenIMKqhWDFnRh52GnVQZIUH4PhH9hdNNfZr+vfojvxma5OuB0aZ5dOh0Th370NMQ=="
            };

            var tokenDto = new TokenDto("lb52JysSEupTAMaRzl9b/ALvhJ++Aft0hwI9WW1NPQq/1akfhmFcHNZ5Q/r76r7a4e6aWVOywPPOLiQRlJsqvwOv2GSNuYMIEOPPbx2P4Ualxobok8n0UYSwEi5GXBm5uc4eOq8i6nB+t6fSnyTcHsBCX2xUD80dLioA/4eBezTmIjAZf/aVt/1aqEO4ztJEb6ybS85prABB42k45mlO0zRZa3ABbXzGcdu4yEKMlLGRDleFqAf7amzDPJD2IPExoKbYw+82fjLgzmT2vyGgdxenIMKqhWDFnRh52GnVQZIUH4PhH9hdNNfZr+vfojvxma5OuB0aZ5dOh0Th370NMQ==", TokenType.PATIENT_AUTH_TOKEN.ToString(), DateTime.Now.AddDays(2).ToString(), "12345");

            _mockLoginService
                .Setup(x => x.Login(It.IsAny<LoginCredentialsDto>()))
                .ReturnsAsync(loginOutputDto);

            _mockTokenService
                .Setup(x => x.CreateAuthToken(It.IsAny<LoginCredentialsDto>()))
                .ReturnsAsync(tokenDto);

            // Act
            var result = await _controller.Login(loginCredentialsDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<LoginOutputDto>(okResult.Value);
            Assert.Equal("lb52JysSEupTAMaRzl9b/ALvhJ++Aft0hwI9WW1NPQq/1akfhmFcHNZ5Q/r76r7a4e6aWVOywPPOLiQRlJsqvwOv2GSNuYMIEOPPbx2P4Ualxobok8n0UYSwEi5GXBm5uc4eOq8i6nB+t6fSnyTcHsBCX2xUD80dLioA/4eBezTmIjAZf/aVt/1aqEO4ztJEb6ybS85prABB42k45mlO0zRZa3ABbXzGcdu4yEKMlLGRDleFqAf7amzDPJD2IPExoKbYw+82fjLgzmT2vyGgdxenIMKqhWDFnRh52GnVQZIUH4PhH9hdNNfZr+vfojvxma5OuB0aZ5dOh0Th370NMQ==", returnValue.Token);
            Assert.Equal(LoginResult.Success.ToString(), returnValue.Result);
        }


        [Fact]
        public async Task LoginIAM_ReturnsOk_WithValidAuthentication()
        {
            // Arrange
            var claimsIdentity = new ClaimsIdentity(new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "testuser"),
                new Claim(ClaimTypes.Email, "testuser@example.com")
            }, "cookie");

            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            var authResult = AuthenticateResult.Success(new AuthenticationTicket(claimsPrincipal, CookieAuthenticationDefaults.AuthenticationScheme));

            // Mock the IAuthenticationService
            var mockAuthService = new Mock<IAuthenticationService>();
            mockAuthService
                .Setup(x => x.AuthenticateAsync(It.IsAny<HttpContext>(), CookieAuthenticationDefaults.AuthenticationScheme))
                .ReturnsAsync(authResult);

            // Create a mock service provider and add IAuthenticationService to it
            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock
                .Setup(x => x.GetService(typeof(IAuthenticationService)))
                .Returns(mockAuthService.Object);

            // Mock the HttpContext
            var mockHttpContext = new Mock<HttpContext>();
            mockHttpContext
                .Setup(x => x.RequestServices)
                .Returns(serviceProviderMock.Object);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext.Object
            };

            var loginCredentialsDto = new LoginCredentialsDto("testuser", "password123");

            _mockLoginService
                .Setup(x => x.TreatAuthenticateResult(It.IsAny<string[]>()))
                .Returns(loginCredentialsDto);

            _mockLoginService
                .Setup(x => x.Login(It.IsAny<LoginCredentialsDto>()))
                .ReturnsAsync(new LoginOutputDto { Result = "SUCCESS", Token = "token123" });

            _mockTokenService
                .Setup(x => x.CreateAuthToken(It.IsAny<LoginCredentialsDto>()))
                .ReturnsAsync(new TokenDto("token123", "", "", ""));

            // Act
            var result = await _controller.LoginIAMResponse();

            // Assert
            var redirectResult = Assert.IsType<RedirectResult>(result); // Ensure it's a RedirectResult
            Assert.StartsWith("http://localhost:4200", redirectResult.Url); // Check the base URL

            // Extract the token from the query string
            var uri = new Uri(redirectResult.Url);
            var query = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(uri.Query);
            Assert.True(query.ContainsKey("token")); // Ensure token exists
            Assert.Equal("token123", query["token"]); // Verify the token value
        }


        

    }
}
