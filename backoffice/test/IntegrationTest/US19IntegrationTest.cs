using DDDSample1.Controllers;
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
using Moq;

namespace DDDNetCore.Test.IntegrationTest
{
	public class US19IntegrationTest
	{
		private readonly Mock<IUnitOfWork> _mockUnitOfWork;
		private readonly Mock<IOperationRequestRepository> _mockRequestRepo;
		private readonly Mock<IStaffRepository> _mockStaffRepo;
		private readonly Mock<IDoctorRepository> _mockDoctorRepo;
		private readonly Mock<IPatientRepository> _mockPatientRepo;
		private readonly Mock<IOperationTypeRepository> _mockTypeRepo;
		private readonly Mock<ILogRepository> _mockLogRepo;
		private readonly Mock<ITokenRepository> _mockTokenRepo;
		private readonly Mock<IUserRepository> _mockUserRepo;
		private readonly OperationRequestService _opReqSvc;
		private readonly UserService _usrSvc;
		private readonly Mock<TokenService> _mockTokenSvc;
		private readonly LogService _logSvc;
		private readonly OperationRequestController _ctrl;
		private readonly User user;
		private readonly User _userPatient;
		private readonly User _userStaff;
		private readonly Patient _patient;
		private readonly Staff _staff;
		private readonly Doctor _doctor;
		private readonly OperationRequest _opReq;
		private readonly Token _token;
		private readonly List<OperationRequest> _opReqList;
		private readonly List<OperationPhase> _opPhases;
		private readonly List<RequiredSpecialist> _reqSpec;
		private readonly OperationType _operationType;
		private readonly List<RequiredSpecialist> _reqSpec2;
		private List<OperationType> _opTypeList;
		public US19IntegrationTest()
		{
			_mockUnitOfWork = new Mock<IUnitOfWork>();
			_mockUserRepo = new Mock<IUserRepository>();
			_mockPatientRepo = new Mock<IPatientRepository>();
			_mockRequestRepo = new Mock<IOperationRequestRepository>();
			_mockStaffRepo = new Mock<IStaffRepository>();
			_mockDoctorRepo = new Mock<IDoctorRepository>();
			_mockTypeRepo = new Mock<IOperationTypeRepository>();
			_mockTokenRepo = new Mock<ITokenRepository>();
			_mockLogRepo = new Mock<ILogRepository>();
			_mockTokenSvc = new Mock<TokenService>();

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

			_opTypeList = [];
			_opTypeList.Add(_operationType);
			_userPatient = new User("joaogarrafas@nope.com", UserRole.PATIENT);

			_userStaff = new User("no@nope.university.com", UserRole.STAFF);

			_patient = new Patient(
					new MedicalRecordNumber("MRN12346"),
					new FirstName("John"), new LastName("Bottles"),
					new FullName("John Bottles"), Gender.MALE,
					new DateOfBirth("2004-02-24"),
					new ContactInformation(new EmailAddress("joaogarrafas@nope.com"),
					new PhoneNumber("968502397")),
					new PhoneNumber("968502397"),
					new List<Appointment>(),
					_userPatient
				);

			Patient patient = new Patient(
					new MedicalRecordNumber("MRN12345"),
					new FirstName("John"), new LastName("Bottles"),
					new FullName("John Bottles"), Gender.MALE,
					new DateOfBirth("2004-02-24"),
					new ContactInformation(new EmailAddress("joaogarrafas@nope.com"),
					new PhoneNumber("968502397")),
					new PhoneNumber("968502397"),
					new List<Appointment>(),
					_userPatient
				);

			_staff = new(
					new LicenseNumber("1"),
					new ContactInformation(
						new EmailAddress("no@nope.university.com"),
						new PhoneNumber("123456789")),
					new FullName("Titigas Silvations"),
					new Specialization("blehSpecialization", ""),
					_userStaff,
					new FirstName("Titigas"),
					new LastName("Silvations")
				);

			_doctor = new(_staff);

			_opReq = new(_doctor, _patient, new OperationType(new OperationTypeName("operatedTypings"), _opPhases, _reqSpec, new EstimatedDuration(70)), OperationPriority.LOW, DateTime.Now);

			OperationRequest opReq = new(_doctor, patient,
					new OperationType(new OperationTypeName("typingsOperated"), _opPhases, _reqSpec, new EstimatedDuration(70)),
					OperationPriority.HIGH, DateTime.Now
				);

			_token = new Token(
					new TokenId("c185d517-d467-4ba5-a789-eb4f77a194b1"),
					DateTime.Now, _userStaff,
					TokenType.STAFF_AUTH_TOKEN
				);

			_opReqList = [_opReq, opReq];

			RequiredSpecialist specialist = new(new Specialization("blehSpecialization", ""), new SpecialistCount(3), PhaseName.CLEANING);
			List<RequiredSpecialist> specialists = [specialist];
			_staff.specializationId = specialist.Specialization.Id;

			_opReqSvc = new OperationRequestService(_mockUnitOfWork.Object, _mockRequestRepo.Object, _mockStaffRepo.Object,
				_mockDoctorRepo.Object, _mockPatientRepo.Object, _mockTypeRepo.Object, _mockLogRepo.Object);

			_mockTokenSvc.Setup(s => s.GetByIdAsync(It.IsAny<TokenId>()))
				.ReturnsAsync(_token.ToDto());

			_mockRequestRepo.Setup(req => req.GetRequestById(It.IsAny<OperationRequestId>()))
				.ReturnsAsync(_opReq);

			_mockStaffRepo.Setup(s => s.GetStaffByUser(It.IsAny<Username>()))
				.ReturnsAsync(_staff);

			_mockRequestRepo.Setup(req => req.GetByIdAsync(It.IsAny<OperationRequestId>()))
				.ReturnsAsync(_opReq);

			_mockRequestRepo.Setup(req => req.Update(It.IsAny<OperationRequest>()))
				.Returns(_opReq);

			_mockPatientRepo.Setup(r => r.GetByIdAsync(It.IsAny<MedicalRecordNumber>()))
				.ReturnsAsync(_patient);

			_mockTypeRepo.Setup(r => r.GetByNameLatest(It.IsAny<OperationTypeName>()))
				.ReturnsAsync(_operationType);

			_mockRequestRepo.Setup(r => r.AddAsync(It.IsAny<OperationRequest>()))
				.ReturnsAsync(_opReq);

			_mockDoctorRepo.Setup(d => d.GetDoctorByLicenseNumber(It.IsAny<LicenseNumber>()))
				.ReturnsAsync(_doctor);

			_mockRequestRepo.Setup(req => req.GetRequestByDoctor(It.IsAny<DoctorId>(), It.IsAny<QueryDataDto>()))
				.ReturnsAsync(_opReqList);

			_usrSvc = new UserService(_mockUnitOfWork.Object, _mockUserRepo.Object, _mockTokenSvc.Object);

			_ctrl = new OperationRequestController(_opReqSvc, _usrSvc, _mockTokenSvc.Object, null, null);
		}

		[Fact]
		public async Task Test_US19IntegrationTest_FilterTest1()
		{
			List<OperationRequestDTO> ret = (await _ctrl.ListOperationRequest(
				null, //_opReq.Patient.Id.AsString(),
				null,
				_opReq.OperationPriority.ToString(),
				null,
				_token.Id.Value.ToString()
			)).Value;

			Assert.NotNull(ret);
			Assert.Equal(2, ret.Count);
			_mockStaffRepo.Verify(r => r.GetStaffByUser(It.IsAny<Username>()), Times.Once);
			_mockDoctorRepo.Verify(r => r.GetDoctorByLicenseNumber(It.IsAny<LicenseNumber>()), Times.Once);
			_mockRequestRepo.Verify(r => r.GetRequestByDoctor(It.IsAny<DoctorId>(), It.IsAny<QueryDataDto>()), Times.Once);
		}

		[Fact]
		public async Task Test_US19IntegrationTest_FilterTest2()
		{
			List<OperationRequest> operations = [_opReq];

			_mockRequestRepo.Setup(req => req.GetRequestByDoctor(It.IsAny<DoctorId>(), It.IsAny<QueryDataDto>()))
				.ReturnsAsync(operations);

			List<OperationRequestDTO> ret = (await _ctrl.ListOperationRequest(
				_opReq.Patient.Id.AsString(),
				null,
				_opReq.OperationPriority.ToString(),
				null,
				_token.Id.Value.ToString()
			)).Value;

			Assert.NotNull(ret);
			Assert.Equal(1, ret.Count);
			_mockStaffRepo.Verify(r => r.GetStaffByUser(It.IsAny<Username>()), Times.Once);
			_mockDoctorRepo.Verify(r => r.GetDoctorByLicenseNumber(It.IsAny<LicenseNumber>()), Times.Once);
			_mockRequestRepo.Verify(r => r.GetRequestByDoctor(It.IsAny<DoctorId>(), It.IsAny<QueryDataDto>()), Times.Once);
		}
	}
}
