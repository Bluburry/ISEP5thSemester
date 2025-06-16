using DDDSample1.Controllers;
using DDDSample1.Domain.OperationPhases;
using DDDSample1.Domain.OperationTypes;
using DDDSample1.Domain.RequiredSpecialists;
using DDDSample1.Domain.Shared;
using DDDSample1.Domain.Specializations;
using DDDSample1.Domain.Tokens;
using DDDSample1.Domain.Users;
using DDDSample1.Domain.ValueObjects;
using Moq;


namespace DDDNetCore.test.IntegrationTest
{
	public class US22IntegrationTest
	{
		private readonly Mock<IUnitOfWork> _mockUnitOfWork;
		private readonly Mock<IOperationTypeRepository> _mockOpTypeRepo;
		private readonly Mock<ISpecializationRepository> _mockSpecRepo;
		private readonly Mock<IRequiredSpecialistRepository> _mockReqSpecRepo;
		private readonly Mock<IOperationPhaseRepository> _mockOpPhaseRepo;
		private readonly Mock<TokenService> _mockTokenSvc;
		private readonly OperationTypeService _opTypeSvc;
		private readonly List<OperationPhase> _opPhases;
		private readonly List<RequiredSpecialist> _reqSpecialist;
		private readonly User _userStaff;
		private readonly Token _token;
		private readonly OperationTypeController controller;
		private readonly OperationType _operationType;

		public US22IntegrationTest()
		{
			_mockUnitOfWork = new Mock<IUnitOfWork>();

			_mockOpTypeRepo = new Mock<IOperationTypeRepository>();
			_mockSpecRepo = new Mock<ISpecializationRepository>();
			_mockReqSpecRepo = new Mock<IRequiredSpecialistRepository>();
			_mockOpPhaseRepo = new Mock<IOperationPhaseRepository>();

			_reqSpecialist = [];
			_reqSpecialist.Add(new RequiredSpecialist(new Specialization("SpecializedInCopyright", ""), new SpecialistCount("10"), PhaseName.PREPARATION));
			_reqSpecialist.Add(new RequiredSpecialist(new Specialization("CopyrightedInSpecialization", ""), new SpecialistCount("10"), PhaseName.SURGERY));
			_reqSpecialist.Add(new RequiredSpecialist(new Specialization("CopyrightSpecialized", ""), new SpecialistCount("10"), PhaseName.CLEANING));

			_opPhases = [];
			_opPhases.Add(new OperationPhase(PhaseName.PREPARATION, 15));
			_opPhases.Add(new OperationPhase(PhaseName.SURGERY, 35));
			_opPhases.Add(new OperationPhase(PhaseName.CLEANING, 15));

			_userStaff = new("no@nope.university.com", UserRole.STAFF);
			_token = new(new TokenId("c185d517-d467-4ba5-a789-eb4f77a194b1"), DateTime.Now, _userStaff, TokenType.ADMIN_AUTH_TOKEN);

			_operationType = new OperationType(
				new OperationTypeName("OperatingTypingations"),
				_opPhases,// new List<OperationPhase>(),
				_reqSpecialist, // new List<RequiredSpecialist>(),
				new EstimatedDuration(70)
			);

			_opTypeSvc = new OperationTypeService(_mockUnitOfWork.Object, _mockOpTypeRepo.Object, _mockSpecRepo.Object,
			_mockReqSpecRepo.Object, _mockOpPhaseRepo.Object);

			_mockTokenSvc = new Mock<TokenService>();

			controller = new OperationTypeController(_opTypeSvc, _mockTokenSvc.Object);
		}

		[Fact]
		public async Task DeactivateByName_Successful()
		{

			// _operationType.RequiredSpecialists.Add(_reqSpecialist);

			// _operationType.OperationPhases.Add(new OperationPhase(PhaseName.PREPARATION, 20));
			// _operationType.OperationPhases.Add(new OperationPhase(PhaseName.SURGERY, 30));
			// _operationType.OperationPhases.Add(new OperationPhase(PhaseName.CLEANING, 10));

			// _operationType.ActivationStatus = ActivationStatus.ACTIVATED;

			_mockTokenSvc.Setup(s => s.GetByIdAsync(It.IsAny<TokenId>()))
				.ReturnsAsync(_token.ToDto());

			_mockOpTypeRepo.Setup(op => op.GetByNameLatest(It.IsAny<OperationTypeName>()))
			.ReturnsAsync(_operationType);

			_mockOpTypeRepo.Setup(op => op.Update(It.IsAny<OperationType>()))
			.Returns(_operationType);

			var val = await controller.DeactivateByName(_operationType.OperationTypeName.ToString(), _token.ToDto().TokenId);

			Assert.NotNull(val.Value);
			Assert.Equal(ActivationStatus.DEACTIVATED.ToString(), val.Value.ActivationStatus);
			_mockOpTypeRepo.Verify(r => r.GetByNameLatest(It.IsAny<OperationTypeName>()), Times.Once);
			_mockOpTypeRepo.Verify(r => r.Update(It.IsAny<OperationType>()), Times.Once);
			_mockTokenSvc.Verify(r => r.GetByIdAsync(It.IsAny<TokenId>()), Times.Once);
		}
	}
}
