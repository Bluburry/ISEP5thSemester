using DDDSample1.Controllers;
using DDDSample1.Domain.Specializations;
using DDDSample1.Domain.Tokens;
using DDDSample1.Domain.Users;
using DDDSample1.Domain.ValueObjects;
using Moq;

namespace DDDNetCore.test.ControllerTest
{
	public class SpecializationControllerTest
	{
		private readonly Mock<TokenService> _mockTokenService;
		private readonly Mock<SpecializationService> _mockService;
		private readonly SpecializationController _ctrl;
		private readonly Token _token;
		private readonly User _user;
		private readonly Token _tokenStaff;
		private readonly TokenDto _tokenDto;
		private readonly SpecializationDTO _specDTO;
		private readonly Specialization _spec;
		private readonly Specialization _spec2;
		private readonly List<SpecializationDTO> _specList;

		public SpecializationControllerTest()
		{
			_mockTokenService = new();
			_mockService = new();

			_user = new("no@nope.university.com", UserRole.ADMIN);

			_token = new(new TokenId("c185d517-d467-4ba5-a789-eb4f77a194b1"), DateTime.Now, _user, TokenType.ADMIN_AUTH_TOKEN);


			_specList = [];
			_spec = new Specialization("TestingSpecialization", "short description", "SpecCode123");
			_spec2 = new Specialization("SpecializationTesting", "short description", "321SpecCode");
			_specList.Add(_spec.ToDTO());
			_specList.Add(_spec2.ToDTO());

			_mockTokenService.Setup(s => s.GetByIdAsync(It.IsAny<TokenId>()))
				.ReturnsAsync(_token.ToDto());

			_ctrl = new SpecializationController(_mockTokenService.Object, _mockService.Object);
		}

		[Fact]
		public async Task FilteredSearch_Successful()
		{
			_mockService.Setup(s => s.FilteredGet(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(_spec.ToDTO());

			var list = await _ctrl.FilteredSearch("", _spec.SpecializationName, _token.ToDto().TokenId);

			Assert.NotNull(list);

			_mockTokenService.Verify(r => r.GetByIdAsync(It.IsAny<TokenId>()), Times.Once);
			_mockService.Verify(r => r.FilteredGet(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
		}

		[Fact]
		public async Task FilteredSearch_UnSuccessful()
		{
			User user = new("no@nope.university.com", UserRole.STAFF);

			Token token = new(new TokenId("c185d517-d467-4ba5-a789-eb4f77a194b1"), DateTime.Now, _user, TokenType.STAFF_AUTH_TOKEN);

			_mockTokenService.Setup(s => s.GetByIdAsync(It.IsAny<TokenId>()))
				.ReturnsAsync(token.ToDto());

			_mockService.Setup(s => s.FilteredGet(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(_spec.ToDTO());

			await _ctrl.FilteredSearch("", _spec.SpecializationName, token.ToDto().TokenId);

			_mockTokenService.Verify(r => r.GetByIdAsync(It.IsAny<TokenId>()), Times.Once);
			_mockService.Verify(r => r.FilteredGet(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
		}

		[Fact]
		public async Task DeleteSpecializations()
		{
			_mockService.Setup(s => s.DeleteSpecialization(It.IsAny<string>())).ReturnsAsync(_spec.ToDTO());

			var ret = await _ctrl.RemoveSpecialization(_spec.Id.AsString(), _token.ToDto().TokenId);

			Assert.NotNull(ret);

			_mockTokenService.Verify(r => r.GetByIdAsync(It.IsAny<TokenId>()), Times.Once);
			_mockService.Verify(r => r.DeleteSpecialization(It.IsAny<string>()), Times.Once);
		}

		[Fact]
		public async Task CreateSpecializations()
		{
			_mockService.Setup(s => s.CreateSpecialization(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(_spec.ToDTO());

			var ret = await _ctrl.CreateSpecialization(_spec.ToDTO(), _token.ToDto().TokenId);

			Assert.NotNull(ret);

			_mockTokenService.Verify(r => r.GetByIdAsync(It.IsAny<TokenId>()), Times.Once);
			_mockService.Verify(r => r.CreateSpecialization(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
		}
	}
}