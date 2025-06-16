using DDDSample1.Controllers;
using DDDSample1.Domain.Logs;
using DDDSample1.Domain.Shared;
using DDDSample1.Domain.Specializations;
using DDDSample1.Domain.Tokens;
using DDDSample1.Domain.Users;
using DDDSample1.Domain.ValueObjects;
using Microsoft.CodeAnalysis;
using Moq;

namespace DDDNetCore.Test.IntegrationTest
{
	public class US_7_2_13_IntegrationTest
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

		public US_7_2_13_IntegrationTest()
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
		public async Task Edit_Unsuccessful1()
		{
			_mockSpecRepo.Setup(s => s.GetByIdAsync(It.IsAny<SpecializationCode>())).ReturnsAsync(_spec);
			_mockSpecRepo.Setup(s => s.GetByName(It.IsAny<string>())).ReturnsAsync(_spec);


			SpecializationDTO spec = new("TestingSpecialization", "", "");

			var exception = await Assert.ThrowsAsync<ArgumentException>(async () =>
				await _ctrl.EditSpecialization(spec.SpecializationCode, spec, _token.ToDto().TokenId)
				//await _service.UpdateSpecialization(_spec.Id.AsString(), "", "")
			);

			Assert.Equal("Can't update specialization with null name and description", exception.Message);

			_mockSpecRepo.Verify(r => r.GetByName(It.IsAny<string>()), Times.Never);
			_mockSpecRepo.Verify(r => r.AddAsync(It.IsAny<Specialization>()), Times.Never);
			_mockUnitOfWork.Verify(r => r.CommitAsync(), Times.Never);
			_mockTokenService.Verify(r => r.GetByIdAsync(It.IsAny<TokenId>()), Times.Once);
		}

		[Fact]
		public async Task Edit_Unsuccessful2()
		{
			_mockSpecRepo.Setup(s => s.GetByIdAsync(It.IsAny<SpecializationCode>())).ReturnsAsync(_spec);
			_mockSpecRepo.Setup(s => s.GetByName(It.IsAny<string>())).ReturnsAsync(_spec);

			var exception = await Assert.ThrowsAsync<ArgumentException>(async () =>
				await _ctrl.EditSpecialization(_spec.Id.AsString(), _spec.ToDTO(), _token.ToDto().TokenId)
			/* await _service.UpdateSpecialization(
				_spec.Id.AsString(), _spec.SpecializationName, _spec.SpecializationDescription
				) */
			);

			Assert.Equal("Update values are the same as the original specialization", exception.Message);

			_mockSpecRepo.Verify(r => r.GetByName(It.IsAny<string>()), Times.Never);
			_mockSpecRepo.Verify(r => r.AddAsync(It.IsAny<Specialization>()), Times.Never);
			_mockUnitOfWork.Verify(r => r.CommitAsync(), Times.Never);
			_mockTokenService.Verify(r => r.GetByIdAsync(It.IsAny<TokenId>()), Times.Once);
		}

		[Fact]
		public async Task Edit_Successful()
		{
			LogsBuilder logsBuilder = new();

			Log log = logsBuilder
				.WithDateAndTime(new DateAndTime(DateTime.Now))
				.WithInformation(_spec.ToString())
				.WithID(_spec.Id.AsString())
				.WithObjectType(ObjectLoggedType.SPECIALIZATION.ToString())
				.Build();

			_mockSpecRepo.Setup(s => s.GetByIdAsync(It.IsAny<SpecializationCode>())).ReturnsAsync(_spec);
			_mockSpecRepo.Setup(s => s.Update(It.IsAny<Specialization>())).Returns(_spec);
			_mockSpecRepo.Setup(s => s.GetByName(It.IsAny<string>()));
			_mockLogRepo.Setup(s => s.AddAsync(It.IsAny<Log>())).ReturnsAsync(log);


			SpecializationDTO spec = new(_spec.Id.AsString(), "new name", "new desc");
			// SpecializationDTO ret = await _service.UpdateSpecialization(_spec.Id.AsString(), "new name", "new desc");

			var ret  = await _ctrl.EditSpecialization(spec.SpecializationCode, spec, _token.ToDto().TokenId);


			_mockSpecRepo.Verify(r => r.GetByName(It.IsAny<string>()), Times.Once);
			_mockSpecRepo.Verify(r => r.Update(It.IsAny<Specialization>()), Times.Once);
			_mockLogRepo.Verify(r => r.AddAsync(It.IsAny<Log>()), Times.Once);
			_mockUnitOfWork.Verify(r => r.CommitAsync(), Times.Once);
			_mockTokenService.Verify(r => r.GetByIdAsync(It.IsAny<TokenId>()), Times.Once);
		}

	}
}