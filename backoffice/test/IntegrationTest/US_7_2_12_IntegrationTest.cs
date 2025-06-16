using DDDSample1.Controllers;
using DDDSample1.Domain.Logs;
using DDDSample1.Domain.Shared;
using DDDSample1.Domain.Specializations;
using DDDSample1.Domain.Tokens;
using DDDSample1.Domain.Users;
using DDDSample1.Domain.ValueObjects;
using Moq;

namespace DDDNetCore.Test.IntegrationTest
{
	public class US_7_2_12_IntegrationTest
	{
		private readonly Mock<IUnitOfWork> _mockUnitOfWork;
		private readonly Mock<ILogRepository> _mockLogRepo;
		private readonly Mock<ISpecializationRepository> _mockSpecRepo;
		private readonly Mock<TokenService> _mockTokenService;
		private readonly SpecializationService _service;
		private readonly SpecializationController _ctrl;
		private readonly Specialization _spec;
		private readonly Specialization _spec2;
		private readonly List<Specialization> _specList;
		private readonly List<SpecializationDTO> _specListDto;
		private readonly Token _token;
		private readonly User _user;

		public US_7_2_12_IntegrationTest()
		{
			_mockUnitOfWork = new();
			_mockSpecRepo = new();
			_mockLogRepo = new();
			_mockTokenService = new();

			_user = new("no@nope.university.com", UserRole.ADMIN);

			_token = new(new TokenId("c185d517-d467-4ba5-a789-eb4f77a194b1"), DateTime.Now, _user, TokenType.ADMIN_AUTH_TOKEN);

			_specList = [];
			_specListDto = [];
			_spec = new Specialization("TestingSpecialization", "short description", "SpecCode123");
			_spec2 = new Specialization("SpecializationTesting", "short description", "321SpecCode");
			_specList.Add(_spec);
			_specList.Add(_spec2);
			_specListDto.Add(_spec.ToDTO());
			_specListDto.Add(_spec2.ToDTO());

			_mockTokenService.Setup(s => s.GetByIdAsync(It.IsAny<TokenId>()))
				.ReturnsAsync(_token.ToDto());

			_service = new SpecializationService(_mockUnitOfWork.Object, _mockSpecRepo.Object, _mockLogRepo.Object);
			_ctrl = new SpecializationController(_mockTokenService.Object, _service);
		}

		[Fact]
		public async Task ListSpecialization_Successful1()
		{
			_mockSpecRepo.Setup(s => s.GetByIdAsync(It.IsAny<SpecializationCode>())).ReturnsAsync(_spec);
			_mockSpecRepo.Setup(s => s.GetByName(It.IsAny<string>())).ReturnsAsync(_spec);

			var ret = await _ctrl.FilteredSearch(_spec.Id.AsString(), _spec.SpecializationName, _token.ToDto().TokenId);

			Assert.NotNull(ret);

			_mockSpecRepo.Verify(r => r.GetByName(It.IsAny<string>()), Times.Never);
			_mockSpecRepo.Verify(r => r.GetByIdAsync(It.IsAny<SpecializationCode>()), Times.Once);
			_mockTokenService.Verify(r => r.GetByIdAsync(It.IsAny<TokenId>()), Times.Once);
		}

		[Fact]
		public async Task ListSpecialization_Successful2()
		{
			_mockSpecRepo.Setup(s => s.GetByName(It.IsAny<string>())).ReturnsAsync(_spec);

			var ret = await _ctrl.FilteredSearch(null, _spec.SpecializationName, _token.ToDto().TokenId);

			Assert.NotNull(ret);

			_mockSpecRepo.Verify(r => r.GetByName(It.IsAny<string>()), Times.Once);
			_mockSpecRepo.Verify(r => r.GetByIdAsync(It.IsAny<SpecializationCode>()), Times.Never);
			_mockTokenService.Verify(r => r.GetByIdAsync(It.IsAny<TokenId>()), Times.Once);
		}

		[Fact]
		public async Task ListSpecialization_UnSuccessful()
		{
			_mockSpecRepo.Setup(s => s.GetByName(It.IsAny<string>())).ReturnsAsync(_spec);
			_mockSpecRepo.Setup(s => s.AddAsync(It.IsAny<Specialization>())).ReturnsAsync(_spec);


			var exception = await Assert.ThrowsAsync<ArgumentException>(async () =>
				await _ctrl.FilteredSearch(null, null, _token.ToDto().TokenId)
			);

			Assert.Equal("Can't filter specialization when both code and name are empty", exception.Message);


			_mockSpecRepo.Verify(r => r.GetByName(It.IsAny<string>()), Times.Never);
			_mockSpecRepo.Verify(r => r.GetByIdAsync(It.IsAny<SpecializationCode>()), Times.Never);
			_mockTokenService.Verify(r => r.GetByIdAsync(It.IsAny<TokenId>()), Times.Once);
		}

	}
}