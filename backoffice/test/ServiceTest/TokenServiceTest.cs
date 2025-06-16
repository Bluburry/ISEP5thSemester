using DDDSample1.Domain.Shared;
using DDDSample1.Domain.Tokens;
using DDDSample1.Domain.Users;
using DDDSample1.Domain.ValueObjects;
using Moq;
using Org.BouncyCastle.Tls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDDNetCore.test.ServiceTest
{
    

    public class TokenServiceTest
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IUserRepository> _mockUserRepo;
        private readonly Mock<ITokenRepository> _mockTokenRepo;
        private readonly TokenService _tokenSvc;
        private readonly User user;


        public TokenServiceTest()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockTokenRepo = new Mock<ITokenRepository>();
            _mockUserRepo = new Mock<IUserRepository>();


            _tokenSvc = new TokenService(_mockUnitOfWork.Object, _mockUserRepo.Object, _mockTokenRepo.Object);

            user = new User(
                "user@example.com",
                "!Password123",
                UserRole.PATIENT
            );
            user.Activate();

        }

        [Fact]
        public async Task GenerateDeletionConfirmationToken_Success_WithGoodValues()
        {

            //Arrange
            Token token = new Token(
                    new TokenId(Guid.NewGuid()),
                    DateTime.Now.AddDays(1),
                    user,
                    TokenType.PATIENT_AUTH_TOKEN
            );


            _mockUserRepo.Setup(s => s.GetByIdAsync(It.IsAny<Username>()))
                .ReturnsAsync(user);
            _mockTokenRepo.Setup(s => s.AddAsync(It.IsAny<Token>()))
                .ReturnsAsync(token);
            _mockUnitOfWork.Setup(s => s.CommitAsync());

            //Act
            var result = await _tokenSvc.GenerateDeletionConfirmationToken(token.Id.ToString());
            
            //Assert
            Assert.NotNull(result);

            _mockTokenRepo.Verify(s => s.AddAsync(It.IsAny<Token>()), Times.Once);
            _mockUnitOfWork.Verify(s => s.CommitAsync(), Times.Once);

            Assert.Equal(result.ToString(), token.ToDto().ToString());

        }

        [Fact]
        public async Task GenerateDeletionConfirmationToken_Failure_WithBadValues()
        {
            //Arrange
            Token token = new Token(
                    new TokenId(Guid.NewGuid()),
                    DateTime.Now.AddDays(1),
                    user,
                    TokenType.PATIENT_AUTH_TOKEN
            );

            
            _mockUserRepo.Setup(s => s.GetByIdAsync(It.IsAny<Username>()))
                .ReturnsAsync((User)null);


            //Act
            //Assert
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _tokenSvc.GenerateDeletionConfirmationToken(token.Id.ToString());
            });
        }

    }
}
