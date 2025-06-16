using System;
using Xunit;
using Moq;
using DDDSample1.Domain.Tokens;
using DDDSample1.Domain.Users;
using DDDSample1.Domain.ValueObjects;

namespace DDDSample1.Domain.Tests
{
    public class TokenTest
    {
        private readonly Mock<User> _mockUser;
        private readonly TokenId _tokenId;
        private readonly DateTime _expirationDate;
        private readonly TokenType _tokenType;

        public TokenTest()
        {
            // Initialize common test data
            _mockUser = new Mock<User>();

            _mockUser.Setup(u => u.ActivationStatus).Returns(ActivationStatus.DEACTIVATED);
            _mockUser.Setup(u => u.EmailAddress).Returns(new EmailAddress("user@example.com"));
            _mockUser.Setup(u => u.Password).Returns(new Password("!Password123"));
            _mockUser.Setup(u => u.Role).Returns(UserRole.STAFF);


            _tokenId = new TokenId(Guid.NewGuid());
            _expirationDate = DateTime.Now.AddHours(1); // Example expiration date
            _tokenType = TokenType.GENERAL_ACCESS; // Replace with a valid TokenType
        }

        [Fact]
        public void CreateToken_ShouldInitializeProperties()
        {
            // Arrange
            var user = _mockUser.Object; // Use your mocked user
            var token = new Token(_tokenId, _expirationDate, user, _tokenType);

            // Act
            // (No action needed for this test)

            // Assert
            Assert.NotNull(token);
            Assert.Equal(_tokenId, token.Id);
            Assert.Equal(_expirationDate, token.ExpirationDate.DateTime);
            Assert.Equal(user.Id, token.UserId);
            Assert.Equal(_tokenType, token.TokenValue);
        }

        [Fact]
        public void IsExpired_ShouldReturnTrue_WhenTokenIsExpired()
        {
            // Arrange
            var user = _mockUser.Object;

            try
            {
                var token = new Token(_tokenId, DateTime.Now.AddHours(-1), user, _tokenType);
            }catch (Exception e)
            {
                Assert.True(e.Message == "Expiration date cannot be in the past.");
            }

            
        }

        [Fact]
        public void MatchesTokenValue_ShouldReturnTrue_WhenTokenValueMatches()
        {
            // Arrange
            var user = _mockUser.Object;
            var token = new Token(_tokenId, _expirationDate, user, _tokenType);

            // Act
            var result = token.MatchesTokenValue(_tokenType.ToString());

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void ToDto_ShouldThrowInvalidOperationException_WhenIdIsNull()
        {
            // Arrange
            var user = _mockUser.Object;
            var token = new Token(null, _expirationDate, user, _tokenType); // Pass null ID

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => token.ToDto());
            Assert.Equal("Token ID cannot be null.", ex.Message);
        }

        // Add more tests as needed for other methods like MatchesTokenValue, IsExpired, etc.
    }
}
