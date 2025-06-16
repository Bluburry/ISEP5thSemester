using DDDSample1.Domain.Shared;
using DDDSample1.Domain.Tokens;
using DDDSample1.Domain.Users;
using DDDSample1.Domain.ValueObjects;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDDNetCore.test.ServiceTest
{
    public class UserServiceTest
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IUserRepository> _mockUserRepo;
        private readonly Mock<TokenService> _mockTokenSvc;
        private UserService _userService;
        private readonly User user;
        public UserServiceTest() {

            _mockTokenSvc = new Mock<TokenService>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockUserRepo = new Mock<IUserRepository>();

            _userService = new UserService(_mockUnitOfWork.Object, _mockUserRepo.Object, _mockTokenSvc.Object);

            user = new User(
                "user@example.com",
                "!Password123",
                UserRole.PATIENT
                );
        }

        [Fact]
        public async Task RegisterBackofficeUserAsync_Success_WithGoodValues()
        {
            //Arrange
            _mockUserRepo.Setup(s => s.AddAsync(It.IsAny<User>()))
                .ReturnsAsync(user);

            // Act
            var result = await _userService.RegisterBackofficeUserAsync("user@example.com");

            // Assert
            var okResult = Assert.IsType<UserDto>(result);

            Assert.Equal(okResult.ToString(), user.ToDto().ToString());
            _mockUnitOfWork.Verify(s => s.CommitAsync(), Times.Once);
            _mockUserRepo.Verify(s => s.AddAsync(It.IsAny<User>()), Times.Once);
                
           
        }
    }
}
