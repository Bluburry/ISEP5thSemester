using DDDSample1.AppServices;
using DDDSample1.Controllers;
using DDDSample1.Domain.Shared;
using DDDSample1.Domain.Tokens;
using DDDSample1.Domain.Users;
using DDDSample1.Domain.ValueObjects;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DDDNetCore.test.ControllerTest
{

    public class PasswordActivationControllerTest
    {

        private readonly Mock<PasswordActivationService> _mockPasswordService;
        private readonly Mock<TokenService> _mockTokenService;
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;        
        private PasswordActivationController _controller;
        private PasswordActivationService _service;

        private User user;
         public PasswordActivationControllerTest()
        {
            _mockTokenService = new Mock<TokenService>();
            _mockPasswordService = new Mock<PasswordActivationService>();
            _mockUserRepository = new Mock<IUserRepository>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();

            _service = new PasswordActivationService(_mockUnitOfWork.Object,_mockUserRepository.Object, _mockTokenService.Object);
            _controller = new PasswordActivationController(_service, _mockTokenService.Object);


            user = new User(
                "user@example.com",
                "!Password123",
                UserRole.PATIENT
                );
        }

        [Fact]
        public void ActivatePassword_Success_WithGoodValues()
        {
            string newPass = "!NewPassword21New";
            User newUser = user;
            newUser.Password = new Password(newPass);

            //Arrange
            Token token = new Token(
                    new TokenId(Guid.NewGuid()),
                    DateTime.Now.AddDays(1),
                    user,
                    TokenType.VERIFICATION_TOKEN
            );

            _mockUserRepository.Setup(s => s.GetByIdAsync(It.IsAny<Username>()))
                .ReturnsAsync(user);

            _mockUserRepository.Setup(s => s.Update(It.IsAny<User>()))
                .Returns(newUser);

            _mockTokenService.Setup(s => s.GetByIdAsync(It.IsAny<TokenId>()))
                .ReturnsAsync(token.ToDto());

            var result = _controller.ActivatePassword(newPass, token.Id.AsString());

            var okResult = Assert.IsType<ActionResult<UserDto>>(result.Result);
            var returnValue = Assert.IsType<UserDto>(okResult.Value);

            Assert.Equal(newUser.ToDto().ToString(), returnValue.ToString());
        }

    }
}