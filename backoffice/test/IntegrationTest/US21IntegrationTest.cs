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
	public class US21IntegrationTest
	{
		private readonly Mock<IUnitOfWork> _mockUnitOfWork;
		private readonly Mock<IOperationTypeRepository> _mockOpTypeRepo;
		private readonly Mock<ISpecializationRepository> _mockSpecRepo;
		private readonly Mock<IRequiredSpecialistRepository> _mockReqSpecRepo;
		private readonly Mock<IOperationPhaseRepository> _mockOpPhaseRepo;
		private readonly Mock<TokenService> _mockTokenSvc;
		private readonly OperationTypeService _opTypeSvc;

		private readonly OperationTypeController controller;
		private readonly List<OperationPhase> _opPhases;
		private readonly List<RequiredSpecialist> _reqSpec;
		private readonly User _userStaff;
		private readonly Token _token;
		private readonly OperationType _operationType;
		private readonly List<RequiredSpecialist> _reqSpec2;
		private OperationType _operationType2;


		public US21IntegrationTest()
		{
			_mockUnitOfWork = new Mock<IUnitOfWork>();

			_mockOpTypeRepo = new Mock<IOperationTypeRepository>();
			_mockSpecRepo = new Mock<ISpecializationRepository>();
			_mockReqSpecRepo = new Mock<IRequiredSpecialistRepository>();
			_mockOpPhaseRepo = new Mock<IOperationPhaseRepository>();

			_reqSpec = [];
			_reqSpec.Add(new RequiredSpecialist(new Specialization("SpecializedInCopyright", ""), new SpecialistCount("10"), PhaseName.PREPARATION));
			_reqSpec.Add(new RequiredSpecialist(new Specialization("CopyrightedInSpecialization", ""), new SpecialistCount("10"), PhaseName.SURGERY));
			_reqSpec.Add(new RequiredSpecialist(new Specialization("CopyrightSpecialized", ""), new SpecialistCount("10"), PhaseName.CLEANING));

			_opPhases = [];
			_opPhases.Add(new OperationPhase(PhaseName.PREPARATION, 15));
			_opPhases.Add(new OperationPhase(PhaseName.SURGERY, 35));
			_opPhases.Add(new OperationPhase(PhaseName.CLEANING, 15));

			_userStaff = new("no@nope.university.com", UserRole.STAFF);
			_token = new(new TokenId("c185d517-d467-4ba5-a789-eb4f77a194b1"), DateTime.Now, _userStaff, TokenType.ADMIN_AUTH_TOKEN);

			// _operationType = new OperationType(new OperationTypeName("OperatingTypingations"), new List<OperationPhase>(), new List<RequiredSpecialist>());
			_operationType = new OperationType(
				new OperationTypeName("OperatingTypingations"),
				_opPhases,// new List<OperationPhase>(),
				_reqSpec, // new List<RequiredSpecialist>(),
				new EstimatedDuration(70)
			);

			_reqSpec2 = [];
			_reqSpec2.Add(new RequiredSpecialist(new Specialization("Copyright Savante", ""), new SpecialistCount("10"), PhaseName.PREPARATION));
			_reqSpec2.Add(new RequiredSpecialist(new Specialization("Savante Copyright", ""), new SpecialistCount("10"), PhaseName.SURGERY));
			_reqSpec2.Add(new RequiredSpecialist(new Specialization("Copynte Savaright", ""), new SpecialistCount("10"), PhaseName.CLEANING));

			_operationType2 = new OperationType(
				new OperationTypeName("OperatingTypingations"),
				_opPhases,// new List<OperationPhase>(),
			   _reqSpec2,// new List<RequiredSpecialist>(),
			   new EstimatedDuration(70)
		   );

			_opTypeSvc = new OperationTypeService(_mockUnitOfWork.Object, _mockOpTypeRepo.Object, _mockSpecRepo.Object,
			_mockReqSpecRepo.Object, _mockOpPhaseRepo.Object);

			_mockTokenSvc = new Mock<TokenService>();

			controller = new OperationTypeController(_opTypeSvc, _mockTokenSvc.Object);
		}

		[Fact]
		public async Task UpdateOperationType_Successful()
		{
			// OperationType _operationType_copy = _operationType;

			// _operationType.RequiredSpecialists.Add(_reqSpec);

			// _operationType_copy.RequiredSpecialists.Add(_reqSpec2);
			// _operationType_copy.OperationPhases.Add(new OperationPhase(PhaseName.PREPARATION, 20));
			// _operationType_copy.OperationPhases.Add(new OperationPhase(PhaseName.SURGERY, 30));
			// _operationType_copy.OperationPhases.Add(new OperationPhase(PhaseName.CLEANING, 10));

			_mockTokenSvc.Setup(s => s.GetByIdAsync(It.IsAny<TokenId>()))
				.ReturnsAsync(_token.ToDto());

			_mockOpTypeRepo.Setup(op => op.GetByNameLatest(It.IsAny<OperationTypeName>()))
			.ReturnsAsync(_operationType);

			_mockSpecRepo.Setup(s => s.GetByName(It.IsAny<string>()))
			.ReturnsAsync(_reqSpec2[0].Specialization);

			_mockOpTypeRepo.Setup(op => op.AddAsync(It.IsAny<OperationType>()))
			.ReturnsAsync(_operationType);

			var val = await controller.UpdateByName(_operationType.OperationTypeName.ToString(), _operationType2.ToDTO(), _token.ToDto().TokenId);

			Assert.NotNull(val.Value);
			Assert.Equal("OperatingTypingations", val.Value.OperationName);

			_mockTokenSvc.Verify(r => r.GetByIdAsync(It.IsAny<TokenId>()), Times.Once);
			_mockOpTypeRepo.Verify(r => r.GetByNameLatest(It.IsAny<OperationTypeName>()), Times.Once);
			_mockSpecRepo.Verify(r => r.GetByName(It.IsAny<string>()), Times.AtLeast(1));
			_mockOpTypeRepo.Verify(r => r.AddAsync(It.IsAny<OperationType>()), Times.Once);
		}
	}
}
