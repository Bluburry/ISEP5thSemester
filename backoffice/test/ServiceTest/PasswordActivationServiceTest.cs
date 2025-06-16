using DDDSample1.AppServices;
using DDDSample1.Controllers;
using DDDSample1.Domain.Shared;
using DDDSample1.Domain.Tokens;
using DDDSample1.Domain.Users;
using DDDSample1.Domain.ValueObjects;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDDNetCore.test.ServiceTest
{
    public class PasswordActivationServiceTest
    {

        private readonly Mock<TokenService> _mockTokenSvc;
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;

        private PasswordActivationService _service;

        private User user;

        public PasswordActivationServiceTest()
        {
            _mockTokenSvc = new Mock<TokenService>();
            _mockUserRepository = new Mock<IUserRepository>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();

            _service = new PasswordActivationService(_mockUnitOfWork.Object,_mockUserRepository.Object, _mockTokenSvc.Object);

            user = new User(
                "user@example.com",
                "!Password123",
                UserRole.PATIENT
                );
        }

        [Fact]
        public async Task ActivatePassword_Success_WithGoodValues()
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

            _mockTokenSvc.Setup(s => s.GetByIdAsync(It.IsAny<TokenId>()))
                .ReturnsAsync(token.ToDto());

            UserDto userDto = await _service.ActivatePassword(newPass, token.Id.AsString());

            Assert.Equal(newUser.ToDto().ToString(), userDto.ToString());

        }

        [Fact]
        public async Task ResetPassword_Success_WithGoodValues()
        {
            user.Activate();
            string newPass = "!NewPassword21New";
            User newUser = user;
            newUser.Password = new Password(newPass);

            _mockUserRepository.Setup(s => s.GetByIdAsync(It.IsAny<Username>()))
                .ReturnsAsync(user);

            _mockUserRepository.Setup(s => s.Update(It.IsAny<User>()))
                .Returns(newUser);

            UserDto userDto = await _service.ResetPassword(newPass, user.ToDto());

            Assert.Equal(newUser.ToDto().ToString(), userDto.ToString());

        }
    }
}
