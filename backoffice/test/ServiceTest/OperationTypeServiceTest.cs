using DDDSample1.Domain.ContactInformations;
using DDDSample1.Domain.Doctors;
using DDDSample1.Domain.HospitalAppointment;
using DDDSample1.Domain.HospitalPatient;
using DDDSample1.Domain.HospitalStaff;
using DDDSample1.Domain.Logs;
using DDDSample1.Domain.OperationPhases;
using DDDSample1.Domain.OperationRequests;
using DDDSample1.Domain.OperationTypes;
using DDDSample1.Domain.RequiredSpecialists;
using DDDSample1.Domain.Shared;
using DDDSample1.Domain.Specializations;
using DDDSample1.Domain.Tokens;
using DDDSample1.Domain.Users;
using DDDSample1.Domain.ValueObjects;
using DDDSample1.DTO;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Moq;
using Org.BouncyCastle.Ocsp;
using SQLitePCL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OperationType = DDDSample1.Domain.OperationTypes.OperationType;

namespace DDDNetCore.test.ServiceTest
{
	public class OperationTypeServiceTest
	{
		private readonly Mock<IUnitOfWork> _mockUnitOfWork;
		private readonly Mock<IOperationTypeRepository> _mockOpTypeRepo;
		private readonly Mock<ISpecializationRepository> _mockSpecRepo;
		private readonly Mock<IRequiredSpecialistRepository> _mockReqSpecRepo;
		private readonly Mock<IOperationPhaseRepository> _mockOpPhaseRepo;
		private readonly OperationTypeService _opTypeSvc;
		private readonly List<OperationPhase> _opPhases;
		private readonly List<RequiredSpecialist> _reqSpec;
		private readonly User _userStaff;
		private readonly Token _token;
		private readonly OperationType _operationType;
		private readonly List<RequiredSpecialist> _reqSpec2;
		private readonly List<OperationType> _opTypeList;
		private readonly OperationType _operationType2;


		public OperationTypeServiceTest()
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

			_opTypeList = [];
			_opTypeList.Add(_operationType);

			_opTypeSvc = new OperationTypeService(_mockUnitOfWork.Object, _mockOpTypeRepo.Object, _mockSpecRepo.Object,
			_mockReqSpecRepo.Object, _mockOpPhaseRepo.Object);
		}

		[Fact]
		public async Task FilteredGet_Successful()
		{
			// _operationType.RequiredSpecialists.Add(_reqSpec);
			// _opTypeList.Add(_operationType);

			_mockOpTypeRepo.Setup(op => op.GetFiltered(It.IsAny<OperationTypeName>(), It.IsAny<string>(), null))
			.ReturnsAsync(_opTypeList);


			List<OperationTypeDTO> listDTO = await _opTypeSvc.FilteredGet("OperatingTypingations", "SpecializedInCopyright", "ACTIVATED");

			Assert.Single(listDTO);
			_mockOpTypeRepo.Verify(r => r.GetFiltered(It.IsAny<OperationTypeName>(), It.IsAny<string>(), null), Times.Once);
		}


		[Fact]
		public async Task RegisterOperationType_Successful()
		{
			// _operationType.RequiredSpecialists.Add(_reqSpec);
			// _operationType.OperationPhases.Add(new OperationPhase(PhaseName.PREPARATION, 20));
			// _operationType.OperationPhases.Add(new OperationPhase(PhaseName.SURGERY, 30));
			// _operationType.OperationPhases.Add(new OperationPhase(PhaseName.CLEANING, 10));

			_mockOpTypeRepo.Setup(op => op.GetByName(It.IsAny<OperationTypeName>()));

			_mockSpecRepo.Setup(s => s.GetByName(It.IsAny<string>()))
			.ReturnsAsync(_reqSpec[0].Specialization);

			_mockOpTypeRepo.Setup(s => s.AddAsync(It.IsAny<OperationType>()))
			.ReturnsAsync(_operationType);

			OperationTypeDTO val = await _opTypeSvc.RegisterOperationType(_operationType.ToDTO());

			Assert.NotNull(val);
			Assert.Equal("OperatingTypingations", val.OperationName);

			_mockOpTypeRepo.Verify(r => r.GetByName(It.IsAny<OperationTypeName>()), Times.Once);
			_mockSpecRepo.Verify(r => r.GetByName(It.IsAny<string>()), Times.Exactly(3));
			_mockOpTypeRepo.Verify(r => r.AddAsync(It.IsAny<OperationType>()), Times.Once);
		}

		[Fact]
		public async Task RegisterOperationType_UnsuccessfulNotEnoughPhases()
		{
			// _operationType.RequiredSpecialists.Add(_reqSpec);
			// _operationType.OperationPhases.Add(new OperationPhase(PhaseName.PREPARATION, 28));
			// _operationType.OperationPhases.Add(new OperationPhase(PhaseName.SURGERY, 30));
			OperationType ot = new(new OperationTypeName("test"), _opPhases, _reqSpec, new EstimatedDuration(70));

			ot.OperationPhases.RemoveAt(1);

			_mockOpTypeRepo.Setup(op => op.GetByName(It.IsAny<OperationTypeName>()));

			_mockSpecRepo.Setup(s => s.GetByName(It.IsAny<string>()))
			.ReturnsAsync(_reqSpec[0].Specialization);

			_mockOpTypeRepo.Setup(s => s.AddAsync(It.IsAny<OperationType>()))
			.ReturnsAsync(_operationType);

			var exception = await Assert.ThrowsAsync<ArgumentException>(async () => await _opTypeSvc.RegisterOperationType(_operationType.ToDTO()));
			Assert.Equal("A minimum of three operation phases are required", exception.Message);

			_mockOpTypeRepo.Verify(r => r.GetByName(It.IsAny<OperationTypeName>()), Times.Once);
			_mockSpecRepo.Verify(r => r.GetByName(It.IsAny<string>()), Times.Exactly(3));
			_mockOpTypeRepo.Verify(r => r.AddAsync(It.IsAny<OperationType>()), Times.Never);
		}

		[Fact]
		public async Task RegisterOperationType_UnsuccessfulExistingType()
		{
			// _operationType.RequiredSpecialists.Add(_reqSpec);
			// _operationType.OperationPhases.Add(new OperationPhase(PhaseName.PREPARATION, 20));
			// _operationType.OperationPhases.Add(new OperationPhase(PhaseName.SURGERY, 30));
			// _operationType.OperationPhases.Add(new OperationPhase(PhaseName.CLEANING, 10));

			_mockOpTypeRepo.Setup(op => op.GetByName(It.IsAny<OperationTypeName>()))
			.ReturnsAsync(_operationType);

			_mockSpecRepo.Setup(s => s.GetByName(It.IsAny<string>()))
			.ReturnsAsync(_reqSpec[0].Specialization);

			_mockOpTypeRepo.Setup(s => s.AddAsync(It.IsAny<OperationType>()))
			.ReturnsAsync(_operationType);

			var exception = await Assert.ThrowsAsync<ArgumentException>(async () => await _opTypeSvc.RegisterOperationType(_operationType.ToDTO()));
			Assert.Equal("An operation with that name already exists.", exception.Message);

			_mockOpTypeRepo.Verify(r => r.GetByName(It.IsAny<OperationTypeName>()), Times.Once);
			_mockSpecRepo.Verify(r => r.GetByName(It.IsAny<string>()), Times.Never);
			_mockOpTypeRepo.Verify(r => r.AddAsync(It.IsAny<OperationType>()), Times.Never);
		}

		[Fact]
		public async Task DeactivateByName_Successful()
		{
			// _operationType.RequiredSpecialists.Add(_reqSpec);

			// _operationType.OperationPhases.Add(new OperationPhase(PhaseName.PREPARATION, 20));
			// _operationType.OperationPhases.Add(new OperationPhase(PhaseName.SURGERY, 30));
			// _operationType.OperationPhases.Add(new OperationPhase(PhaseName.CLEANING, 10));

			// _operationType.ActivationStatus = ActivationStatus.ACTIVATED;

			_mockOpTypeRepo.Setup(op => op.GetByNameLatest(It.IsAny<OperationTypeName>()))
			.ReturnsAsync(_operationType);

			_mockOpTypeRepo.Setup(op => op.Update(It.IsAny<OperationType>()))
			.Returns(_operationType);

			var deactivatedType = await _opTypeSvc.DeactivateByName(_operationType.OperationTypeName);

			Assert.Equal(ActivationStatus.DEACTIVATED.ToString(), deactivatedType.ActivationStatus);
			_mockOpTypeRepo.Verify(r => r.GetByNameLatest(It.IsAny<OperationTypeName>()), Times.Once);
			_mockOpTypeRepo.Verify(r => r.Update(It.IsAny<OperationType>()), Times.Once);
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

			_mockOpTypeRepo.Setup(op => op.GetByNameLatest(It.IsAny<OperationTypeName>()))
			.ReturnsAsync(_operationType);

			_mockSpecRepo.Setup(s => s.GetByName(It.IsAny<string>()))
			.ReturnsAsync(_reqSpec2[0].Specialization);

			_mockOpTypeRepo.Setup(op => op.AddAsync(It.IsAny<OperationType>()))
			.ReturnsAsync(_operationType);

			OperationTypeDTO dto = await _opTypeSvc.UpdateOperationType(_operationType.OperationTypeName, _operationType2.ToDTO());

			Assert.NotNull(dto);
			Assert.Equal("OperatingTypingations", dto.OperationName);
			_mockOpTypeRepo.Verify(r => r.GetByNameLatest(It.IsAny<OperationTypeName>()), Times.Once);
			_mockSpecRepo.Verify(r => r.GetByName(It.IsAny<string>()), Times.Exactly(3));
			_mockOpTypeRepo.Verify(r => r.AddAsync(It.IsAny<OperationType>()), Times.Once);
		}
	}
}
