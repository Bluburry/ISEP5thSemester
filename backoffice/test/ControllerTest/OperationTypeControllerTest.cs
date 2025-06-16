using DDDSample1.Controllers;
using DDDSample1.Domain.OperationPhases;
using DDDSample1.Domain.OperationTypes;
using DDDSample1.Domain.RequiredSpecialists;
using DDDSample1.Domain.Shared;
using DDDSample1.Domain.Specializations;
using DDDSample1.Domain.Tokens;
using DDDSample1.Domain.Users;
using DDDSample1.Domain.ValueObjects;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;


namespace DDDNetCore.test.ControllerTest
{
	public class OperationTypeControllerTest
	{
		private readonly Mock<OperationTypeService> _mockOpTypeSvc;

		private readonly Mock<TokenService> _mockTokenSvc;
		private readonly OperationTypeController controller;
		private readonly List<OperationPhase> _opPhases;
		private readonly List<RequiredSpecialist> _reqSpec;
		private readonly User _userStaff;
		private readonly Token _token;
		private readonly OperationType _operationType;
		private readonly List<OperationType> _opTypeList;
		private readonly List<OperationTypeDTO> _opTypeDTOList;
		private readonly Token _tokenStaff;

		public OperationTypeControllerTest()
		{

			_mockOpTypeSvc = new Mock<OperationTypeService>();
			_mockTokenSvc = new Mock<TokenService>();

			_userStaff = new("no@nope.university.com", UserRole.STAFF);
			_tokenStaff = new(new TokenId("d185d517-d467-4ba5-a789-eb4f77a194b1"), DateTime.Now, _userStaff, TokenType.STAFF_AUTH_TOKEN);

			_reqSpec = [];
			_reqSpec.Add(new RequiredSpecialist(new Specialization("SpecializedInCopyright", ""), new SpecialistCount("10"), PhaseName.PREPARATION));
			_reqSpec.Add(new RequiredSpecialist(new Specialization("CopyrightedInSpecialization", ""), new SpecialistCount("10"), PhaseName.SURGERY));
			_reqSpec.Add(new RequiredSpecialist(new Specialization("CopyrightSpecialized", ""), new SpecialistCount("10"), PhaseName.CLEANING));

			_opPhases = [];
			_opPhases.Add(new OperationPhase(PhaseName.PREPARATION, 15));
			_opPhases.Add(new OperationPhase(PhaseName.SURGERY, 35));
			_opPhases.Add(new OperationPhase(PhaseName.CLEANING, 15));

			_token = new(new TokenId("c185d517-d467-4ba5-a789-eb4f77a194b1"), DateTime.Now, _userStaff, TokenType.ADMIN_AUTH_TOKEN);

			// _operationType = new OperationType(new OperationTypeName("OperatingTypingations"), new List<OperationPhase>(), new List<RequiredSpecialist>());
			_operationType = new OperationType(
				new OperationTypeName("OperatingTypingations"),
				_opPhases,// new List<OperationPhase>(),
				_reqSpec, // new List<RequiredSpecialist>(),
				new EstimatedDuration(70)
			);

			_opTypeList = [];
			_opTypeList.Add(_operationType);

			_opTypeDTOList = [];
			_opTypeDTOList.Add(_operationType.ToDTO());

			controller = new OperationTypeController(_mockOpTypeSvc.Object, _mockTokenSvc.Object);
		}


		[Fact]
		public async Task FilteredSearch_Successful()
		{
			_mockTokenSvc.Setup(s => s.GetByIdAsync(It.IsAny<TokenId>()))
				.ReturnsAsync(_token.ToDto());


			_mockOpTypeSvc.Setup(s => s.FilteredGet(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
				.ReturnsAsync(_opTypeDTOList);

			var listDTO = await controller.FilteredSearch("OperatingTypingations", "SpecializedInCopyright", "ACTIVATED", _token.ToDto().TokenId);

			Assert.Single(listDTO.Value.ToList());
			_mockTokenSvc.Verify(r => r.GetByIdAsync(It.IsAny<TokenId>()), Times.Once);
			_mockOpTypeSvc.Verify(r => r.FilteredGet(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
		}

		[Fact]
		public async Task FilteredSearch_Unsuccessful()
		{
			_mockTokenSvc.Setup(s => s.GetByIdAsync(It.IsAny<TokenId>()))
			.ReturnsAsync(_tokenStaff.ToDto);

			_mockOpTypeSvc.Setup(s => s.FilteredGet(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
				.ReturnsAsync(_opTypeDTOList);

			await controller.FilteredSearch("OperatingTypingations", "SpecializedInCopyright", "ACTIVATED", _tokenStaff.ToDto().TokenId);

			_mockTokenSvc.Verify(r => r.GetByIdAsync(It.IsAny<TokenId>()), Times.Once);
			_mockOpTypeSvc.Verify(r => r.FilteredGet(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
		}

		[Fact]
		public async Task Create_Successful()
		{
			_mockTokenSvc.Setup(s => s.GetByIdAsync(It.IsAny<TokenId>()))
			.ReturnsAsync(_token.ToDto);

			_mockOpTypeSvc.Setup(s => s.RegisterOperationType(It.IsAny<OperationTypeDTO>()))
				.ReturnsAsync(_operationType.ToDTO());

			var val = await controller.Create(_operationType.ToDTO(), _tokenStaff.ToDto().TokenId);

			Assert.NotNull(val.Value);
			Assert.Equal("OperatingTypingations", val.Value.OperationName);
			_mockTokenSvc.Verify(r => r.GetByIdAsync(It.IsAny<TokenId>()), Times.Once);
			_mockOpTypeSvc.Verify(r => r.RegisterOperationType(It.IsAny<OperationTypeDTO>()), Times.Once);
		}

		[Fact]
		public async Task Create_Unsuccessful()
		{
			_mockTokenSvc.Setup(s => s.GetByIdAsync(It.IsAny<TokenId>()))
			.ReturnsAsync(_tokenStaff.ToDto);

			_mockOpTypeSvc.Setup(s => s.RegisterOperationType(It.IsAny<OperationTypeDTO>()))
				.ReturnsAsync(_operationType.ToDTO());

			await controller.Create(_operationType.ToDTO(), _tokenStaff.ToDto().TokenId);

			_mockTokenSvc.Verify(r => r.GetByIdAsync(It.IsAny<TokenId>()), Times.Once);
			_mockOpTypeSvc.Verify(r => r.RegisterOperationType(It.IsAny<OperationTypeDTO>()), Times.Never);
		}

		[Fact]
		public async Task UpdateByName_Successful()
		{
			OperationTypeDTO UpdatedOp = _operationType.ToDTO();
			UpdatedOp.OperationName = "BlehTypingations";

			_mockTokenSvc.Setup(s => s.GetByIdAsync(It.IsAny<TokenId>()))
				.ReturnsAsync(_token.ToDto);

			_mockOpTypeSvc.Setup(s => s.UpdateOperationType(It.IsAny<OperationTypeName>(), It.IsAny<OperationTypeDTO>()))
				.ReturnsAsync(UpdatedOp);

			var val = await controller.UpdateByName("BlehTypingations", _operationType.ToDTO(), _token.ToDto().TokenId);

			Assert.NotNull(val.Value);
			Assert.Equal("BlehTypingations", val.Value.OperationName);
			_mockTokenSvc.Verify(r => r.GetByIdAsync(It.IsAny<TokenId>()), Times.Once);
			_mockOpTypeSvc.Verify(r => r.UpdateOperationType(It.IsAny<OperationTypeName>(), It.IsAny<OperationTypeDTO>()), Times.Once);
		}

		[Fact]
		public async Task UpdateByName_Unsuccessful()
		{
			_mockTokenSvc.Setup(s => s.GetByIdAsync(It.IsAny<TokenId>()))
			.ReturnsAsync(_tokenStaff.ToDto);

			_mockOpTypeSvc.Setup(s => s.UpdateOperationType(It.IsAny<OperationTypeName>(), It.IsAny<OperationTypeDTO>()))
				.ReturnsAsync(_operationType.ToDTO());

			await controller.UpdateByName(_operationType.ToDTO().OperationName, _operationType.ToDTO(), _tokenStaff.ToDto().TokenId);

			_mockTokenSvc.Verify(r => r.GetByIdAsync(It.IsAny<TokenId>()), Times.Once);
			_mockOpTypeSvc.Verify(r => r.UpdateOperationType(It.IsAny<OperationTypeName>(), It.IsAny<OperationTypeDTO>()), Times.Never);
		}

		[Fact]
		public async Task DeactivateByName_Successful()
		{
			OperationType UpdatedOp = _operationType;
			UpdatedOp.ActivationStatus = ActivationStatus.DEACTIVATED; ;

			_mockTokenSvc.Setup(s => s.GetByIdAsync(It.IsAny<TokenId>()))
			.ReturnsAsync(_token.ToDto);

			_mockOpTypeSvc.Setup(s => s.DeactivateByName(It.IsAny<OperationTypeName>()))
				.ReturnsAsync(UpdatedOp.ToDTO());

			var val = await controller.DeactivateByName(_operationType.ToDTO().OperationName, _token.ToDto().TokenId);

			Assert.NotNull(val.Value);
			Assert.Equal(ActivationStatus.DEACTIVATED.ToString(), val.Value.ActivationStatus);
			_mockTokenSvc.Verify(r => r.GetByIdAsync(It.IsAny<TokenId>()), Times.Once);
			_mockOpTypeSvc.Verify(r => r.DeactivateByName(It.IsAny<OperationTypeName>()), Times.Once);
		}

		[Fact]
		public async Task DeactivateByName_Unsuccessful()
		{
			_mockTokenSvc.Setup(s => s.GetByIdAsync(It.IsAny<TokenId>()))
			.ReturnsAsync(_tokenStaff.ToDto);

			_mockOpTypeSvc.Setup(s => s.DeactivateByName(It.IsAny<OperationTypeName>()))
				.ReturnsAsync(_operationType.ToDTO());

			await controller.DeactivateByName(_operationType.ToDTO().OperationName, _tokenStaff.ToDto().TokenId);

			_mockTokenSvc.Verify(r => r.GetByIdAsync(It.IsAny<TokenId>()), Times.Once);
			_mockOpTypeSvc.Verify(r => r.DeactivateByName(It.IsAny<OperationTypeName>()), Times.Never);
		}
	}
}
