using Moq;
using DDDSample1.Domain.ContactInformations;
using DDDSample1.Domain.Doctors;
using DDDSample1.Domain.HospitalAppointment;
using DDDSample1.Domain.HospitalPatient;
using DDDSample1.Domain.HospitalStaff;
using DDDSample1.Domain.Logs;
using DDDSample1.Domain.OperationRequests;
using DDDSample1.Domain.OperationTypes;
using DDDSample1.Domain.Shared;
using DDDSample1.Domain.Specializations;
using DDDSample1.Domain.Tokens;
using DDDSample1.Domain.Users;
using DDDSample1.Domain.ValueObjects;
using DDDSample1.DTO;
using OperationType = DDDSample1.Domain.OperationTypes.OperationType;
using DDDSample1.Domain.RequiredSpecialists;
using DDDSample1.Domain.OperationPhases;

namespace DDDNetCore.test.ServiceTest
{
	public class OperationRequestServiceTest
	{
		private readonly Mock<IUnitOfWork> _mockUnitOfWork;
		private readonly Mock<IOperationRequestRepository> _mockRequestRepo;
		private readonly Mock<IStaffRepository> _mockStaffRepo;
		private readonly Mock<IDoctorRepository> _mockDoctorRepo;
		private readonly Mock<IPatientRepository> _mockPatientRepo;
		private readonly Mock<IOperationTypeRepository> _mockTypeRepo;
		private readonly Mock<ILogRepository> _mockLogRepo;
		private readonly OperationRequestService _opReqSvc;
		private readonly List<OperationPhase> _opPhases;
		private readonly List<RequiredSpecialist> _reqSpec;
		private readonly OperationType _operationType;
		private readonly List<OperationType> _opTypeList;
		private readonly List<OperationTypeDTO> _opTypeDTOList;
		private readonly Token _tokenStaff;


		public OperationRequestServiceTest()
		{
			_mockUnitOfWork = new Mock<IUnitOfWork>();
			_mockRequestRepo = new Mock<IOperationRequestRepository>();
			_mockStaffRepo = new Mock<IStaffRepository>();
			_mockDoctorRepo = new Mock<IDoctorRepository>();
			_mockPatientRepo = new Mock<IPatientRepository>();
			_mockTypeRepo = new Mock<IOperationTypeRepository>();
			_mockLogRepo = new Mock<ILogRepository>();

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

			_opReqSvc = new OperationRequestService(_mockUnitOfWork.Object, _mockRequestRepo.Object, _mockStaffRepo.Object,
			_mockDoctorRepo.Object, _mockPatientRepo.Object, _mockTypeRepo.Object, _mockLogRepo.Object);
		}

		private static User _userPatient = new User("joaogarrafas@nope.com", UserRole.PATIENT);

		private static User _userStaff = new User("no@nope.university.com", UserRole.STAFF);

		private static Patient _patient = new Patient(
			new MedicalRecordNumber("MRN12346"), new FirstName("John"), new LastName("Bottles"), new FullName("John Bottles"), Gender.MALE,
			new DateOfBirth("2004-02-24"), new ContactInformation(new EmailAddress("joaogarrafas@nope.com"), new PhoneNumber("968502397")),
			new PhoneNumber("968502397"), new List<Appointment>(), _userPatient);

		private static Staff _staff = new(new LicenseNumber("1"), new ContactInformation(new EmailAddress("no@nope.university.com"), new PhoneNumber("123456789")),
			new FullName("Titigas Silvations"), new Specialization("blehSpecialization", ""), _userStaff, new FirstName("Titigas"), new LastName("Silvations"));

		private static Doctor _doctor = new(_staff);
		private OperationRequest _opReq = new(_doctor, _patient,
			new OperationType(new OperationTypeName("operatedTypings")), OperationPriority.LOW, DateTime.Now);

		private static Token _token = new Token(new TokenId("c185d517-d467-4ba5-a789-eb4f77a194b1"), DateTime.Now, _userStaff, TokenType.STAFF_AUTH_TOKEN);

		private List<OperationRequest> _opReqList = new List<OperationRequest>();

		/* [Fact]
		public async Task EditRequest_TestSuccessful()
		{
			_mockRequestRepo.Setup(req => req.GetRequestById(It.IsAny<OperationRequestId>()))
				.ReturnsAsync(_opReq);

			_mockStaffRepo.Setup(s => s.GetStaffByUser(It.IsAny<Username>()))
				.ReturnsAsync(_staff);

			_mockDoctorRepo.Setup(d => d.GetDoctorByLicenseNumber(It.IsAny<LicenseNumber>()))
				.ReturnsAsync(_doctor);

			_mockRequestRepo.Setup(req => req.GetByIdAsync(It.IsAny<OperationRequestId>()))
				.ReturnsAsync(_opReq);

			//_mockLogRepo.Setup(l => l.AddAsync(It.IsAny))
			_mockRequestRepo.Setup(req => req.Update(It.IsAny<OperationRequest>()))
				.Returns(_opReq);

			OperationRequestDTO result = await _opReqSvc.EditRequest(_opReq.Id.AsString(), "CRITICAL", "2015-01-20", _token.ToDto());

			Assert.Equal("CRITICAL", result.OperationPriority.ToString());
            _mockRequestRepo.Verify(r => r.GetRequestById(It.IsAny<OperationRequestId>()), Times.Once);
            _mockStaffRepo.Verify(r => r.GetStaffByUser(It.IsAny<Username>()), Times.Once);
			_mockDoctorRepo.Verify(r => r.GetDoctorByLicenseNumber(It.IsAny<LicenseNumber>()), Times.Once);
			_mockRequestRepo.Verify(r => r.GetByIdAsync(It.IsAny<OperationRequestId>()), Times.Once);
            _mockRequestRepo.Verify(r => r.Update(It.IsAny<OperationRequest>()), Times.Once);

        } */

		[Fact]
		public async Task EditRequest_TestUnsucessful()
		{
			_mockRequestRepo.Setup(req => req.GetRequestById(It.IsAny<OperationRequestId>()))
				.ReturnsAsync(_opReq);

			_mockStaffRepo.Setup(s => s.GetStaffByUser(It.IsAny<Username>()))
				.ReturnsAsync(_staff);

			_mockDoctorRepo.Setup(d => d.GetDoctorByLicenseNumber(It.IsAny<LicenseNumber>()))
				.ReturnsAsync(_doctor);

			_mockRequestRepo.Setup(req => req.GetByIdAsync(It.IsAny<OperationRequestId>()))
				.ReturnsAsync(_opReq);

			//_mockLogRepo.Setup(l => l.AddAsync(It.IsAny))
			_mockRequestRepo.Setup(req => req.Update(It.IsAny<OperationRequest>()))
				.Returns(_opReq);


			await Assert.ThrowsAsync<ArgumentException>(async () => await _opReqSvc.EditRequest(_opReq.Id.AsString(), "WHAT", "2015-01-20", _token.ToDto()));
            _mockRequestRepo.Verify(r => r.GetRequestById(It.IsAny<OperationRequestId>()), Times.Once);
            _mockStaffRepo.Verify(r => r.GetStaffByUser(It.IsAny<Username>()), Times.Once);
			_mockDoctorRepo.Verify(r => r.GetDoctorByLicenseNumber(It.IsAny<LicenseNumber>()), Times.Once);
			_mockRequestRepo.Verify(r => r.GetByIdAsync(It.IsAny<OperationRequestId>()), Times.Once);
            _mockRequestRepo.Verify(r => r.Update(It.IsAny<OperationRequest>()), Times.Never);
        }

		[Fact]
		public void DeleteOperationRequest_Test()
		{

			_mockRequestRepo.Setup(req => req.GetRequestById(It.IsAny<OperationRequestId>()))
				.ReturnsAsync(_opReq);

			_mockStaffRepo.Setup(s => s.GetStaffByUser(It.IsAny<Username>()))
				.ReturnsAsync(_staff);

			_mockDoctorRepo.Setup(d => d.GetDoctorByLicenseNumber(It.IsAny<LicenseNumber>()))
				.ReturnsAsync(_doctor);

			var result = _opReqSvc.DeleteOperationRequest(_opReq.Id.AsString(), _token.ToDto());

			Assert.NotNull(result);
            _mockRequestRepo.Verify(r => r.GetRequestById(It.IsAny<OperationRequestId>()), Times.Once);
            _mockStaffRepo.Verify(r => r.GetStaffByUser(It.IsAny<Username>()), Times.Once);
			_mockDoctorRepo.Verify(r => r.GetDoctorByLicenseNumber(It.IsAny<LicenseNumber>()), Times.Once);
		}

		/* [Fact]
		public async Task Test_OperationRequest_FilterList()
		{

			_opReqList.Add(_opReq);

			_mockStaffRepo.Setup(s => s.GetStaffByUser(It.IsAny<Username>()))
				.ReturnsAsync(_staff);

			_mockDoctorRepo.Setup(d => d.GetDoctorByLicenseNumber(It.IsAny<LicenseNumber>()))
				.ReturnsAsync(_doctor);

			_mockRequestRepo.Setup(req => req.GetRequestByDoctor(It.IsAny<DoctorId>(), It.IsAny<QueryDataDto>()))
				.ReturnsAsync(_opReqList);

			List<OperationRequestDTO> result = await _opReqSvc.ListOperationRequest(_patient.fullName.ToString(),
			"operatedTypings", OperationPriority.LOW.ToString(), OperationStatus.PENDING.ToString(), _token.ToDto());

			Assert.Single<OperationRequestDTO>(result);
			_mockStaffRepo.Verify(r => r.GetStaffByUser(It.IsAny<Username>()), Times.Once);
			_mockDoctorRepo.Verify(r => r.GetDoctorByLicenseNumber(It.IsAny<LicenseNumber>()), Times.Once);
			_mockRequestRepo.Verify(r => r.GetRequestByDoctor(It.IsAny<DoctorId>(), It.IsAny<QueryDataDto>()), Times.Once);
		}

		[Fact]
		public async Task Test_OperationRequest_Create()
		{
			_staff.specializationId = _reqSpec[0].Specialization.Id;

			_mockStaffRepo.Setup(s => s.GetStaffByUser(It.IsAny<Username>()))
				.ReturnsAsync(_staff);

			_mockDoctorRepo.Setup(d => d.GetDoctorByLicenseNumber(It.IsAny<LicenseNumber>()))
				.ReturnsAsync(_doctor);

			_mockPatientRepo.Setup(r => r.GetByIdAsync(It.IsAny<MedicalRecordNumber>()))
				.ReturnsAsync(_patient);

			_mockTypeRepo.Setup(r => r.GetByNameLatest(It.IsAny<OperationTypeName>()))
				.ReturnsAsync(_operationType);
			
			_mockRequestRepo.Setup(r => r.AddAsync(It.IsAny<OperationRequest>()))
				.ReturnsAsync(_opReq);

			OperationRequestDTO dto = await _opReqSvc.CreateRequest("patient", "testings", "2025/11/23", "low", _token.ToDto());

			Assert.NotNull(dto);
			Assert.Equal(_opReq.Patient.Id.AsString(), dto.Patient);
			Assert.Equal(_opReq.OperationType.OperationTypeName.OperationName, dto.OperationType);
			_mockStaffRepo.Verify(r => r.GetStaffByUser(It.IsAny<Username>()), Times.Once);
			_mockDoctorRepo.Verify(r => r.GetDoctorByLicenseNumber(It.IsAny<LicenseNumber>()), Times.Once);
			_mockPatientRepo.Verify(r => r.GetByIdAsync(It.IsAny<MedicalRecordNumber>()), Times.Once);
			_mockTypeRepo.Verify(r => r.GetByNameLatest(It.IsAny<OperationTypeName>()), Times.Once);
			_mockRequestRepo.Verify(r => r.AddAsync(It.IsAny<OperationRequest>()), Times.Once);
		} */

	}
}
