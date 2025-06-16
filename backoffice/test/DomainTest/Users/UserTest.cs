using System;
using Xunit;
using Moq;
using DDDSample1.Domain.Users;
using DDDSample1.Domain.ValueObjects;
using DDDSample1.Domain.Shared;

namespace DDDSample1.Domain.Tests
{
    public class UserTests
    {
        private readonly Mock<User> _mockUser;

        public UserTests()
        {
            // Initialize the mock User object
            _mockUser = new Mock<User>();

            // Set up the mocked properties for the User
            _mockUser.Setup(u => u.ActivationStatus).Returns(ActivationStatus.DEACTIVATED);
            _mockUser.Setup(u => u.EmailAddress).Returns(new EmailAddress("user@example.com"));
            _mockUser.Setup(u => u.Password).Returns(new Password("!Password123"));
            _mockUser.Setup(u => u.Role).Returns(UserRole.STAFF);
        }

        [Fact]
        public void CreateUser_ShouldInitializePropertiesCorrectly()
        {
            // Arrange
            var user = _mockUser.Object;

            // Act & Assert
            Assert.Equal("user@example.com", user.EmailAddress.Value);
            Assert.Equal("!Password123", user.Password.Value);
            Assert.Equal(UserRole.STAFF, user.Role);
            Assert.Equal(ActivationStatus.DEACTIVATED, user.ActivationStatus);
        }

        [Fact]
        public void Activate_ShouldSetStatusToActivated()
        {
            // Arrange
            var user = _mockUser.Object;

            // Override the setup to simulate a deactivated state
            _mockUser.Setup(u => u.ActivationStatus).Returns(ActivationStatus.DEACTIVATED);

            // Act
            user.Activate();

            // Assert
            _mockUser.VerifySet(u => u.ActivationStatus = ActivationStatus.ACTIVATED);
        }

        [Fact]
        public void Activate_ShouldThrowException_WhenAlreadyActivated()
        {
            // Arrange
            var user = _mockUser.Object;

            // Set user to already activated
            _mockUser.Setup(u => u.ActivationStatus).Returns(ActivationStatus.ACTIVATED);

            // Act & Assert
            var exception = Assert.Throws<BusinessRuleValidationException>(() => user.Activate());
            Assert.Equal("The user is already activated.", exception.Message);
        }

        [Fact]
        public void Deactivate_ShouldSetStatusToDeactivated()
        {
            // Arrange
            var user = _mockUser.Object;

            // Set user to activated for testing the deactivation
            _mockUser.Setup(u => u.ActivationStatus).Returns(ActivationStatus.ACTIVATED);

            // Act
            user.Deactivate();

            // Assert
            _mockUser.VerifySet(u => u.ActivationStatus = ActivationStatus.DEACTIVATED);
        }

        [Fact]
        public void Deactivate_ShouldThrowException_WhenAlreadyDeactivated()
        {
            // Arrange
            var user = _mockUser.Object;

            // Set user to already deactivated
            _mockUser.Setup(u => u.ActivationStatus).Returns(ActivationStatus.DEACTIVATED);

            // Act & Assert
            var exception = Assert.Throws<BusinessRuleValidationException>(() => user.Deactivate());
            Assert.Equal("The user is already deactivated.", exception.Message);
        }

        [Fact]
        public void ChangePassword_ShouldChangePassword_WhenActivated()
        {
            // Arrange
            var user = _mockUser.Object;

            // Set user to activated
            _mockUser.Setup(u => u.ActivationStatus).Returns(ActivationStatus.ACTIVATED);

            var newPassword = "!NewPassword123";

            // Act
            user.ChangePassword(newPassword);

            // Assert
            _mockUser.VerifySet(u => u.Password = It.Is<Password>(p => p.Value == newPassword));
        }

        [Fact]
        public void ChangePassword_ShouldThrowException_WhenDeactivated()
        {
            // Arrange
            var user = _mockUser.Object;

            // Set user to deactivated
            _mockUser.Setup(u => u.ActivationStatus).Returns(ActivationStatus.DEACTIVATED);

            var newPassword = "NewPassword123";

            // Act & Assert
            var exception = Assert.Throws<BusinessRuleValidationException>(() => user.ChangePassword(newPassword));
            Assert.Equal("Cannot change password for a deactivated user.", exception.Message);
        }

        [Fact]
        public void ChangeRole_ShouldChangeRole_WhenActivated()
        {
            // Arrange
            var user = _mockUser.Object;

            // Set user to activated
            _mockUser.Setup(u => u.ActivationStatus).Returns(ActivationStatus.ACTIVATED);

            var newRole = UserRole.ADMIN;

            // Act
            user.ChangeRole(newRole);

            // Assert
            _mockUser.VerifySet(u => u.Role = newRole);
        }

        [Fact]
        public void ChangeRole_ShouldThrowException_WhenDeactivated()
        {
            // Arrange
            var user = _mockUser.Object;

            // Set user to deactivated
            _mockUser.Setup(u => u.ActivationStatus).Returns(ActivationStatus.DEACTIVATED);

            var newRole = UserRole.ADMIN;

            // Act & Assert
            var exception = Assert.Throws<BusinessRuleValidationException>(() => user.ChangeRole(newRole));
            Assert.Equal("Cannot change role for a deactivated user.", exception.Message);
        }

        [Fact]
        public void ToDto_ShouldReturnCorrectDto()
        {
            // Arrange
            var user = _mockUser.Object;

            // Act
            var userDto = user.ToDto();

            // Assert
            Assert.Equal("user@example.com", userDto.EmailAddress); // Check the EmailAddress (which is the Id in this case)
            Assert.Equal(UserRole.STAFF.ToString(), userDto.Role); // Verify the Role
            Assert.Equal(ActivationStatus.DEACTIVATED.ToString(), userDto.ActivationStatus); // Verify the ActivationStatus
        }

    }
}
