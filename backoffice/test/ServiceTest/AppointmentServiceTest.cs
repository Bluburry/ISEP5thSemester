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
using DDDSample1.Application.AvailabilitySlots;
using DDDSample1.Domain.OperationRooms;
using DDDSample1.Domain.OperationRoomTypes;
using DDDSample1.Domain.AssignedStaffs;
using DDDSample1.Domain.AvailabilitySlots;

namespace DDDNetCore.test.ServiceTest
{
	public class AppointmentServiceTest
	{
		private readonly Mock<IUnitOfWork> _mockUnitOfWork;
		private readonly Mock<IAppointmentRepository> _mockAppointmentRepo;
		private readonly Mock<IOperationRequestRepository> _mockRequestRepo;
		private readonly Mock<IOperationRoomRepository> _mockOpRoomRepo;
		private readonly Mock<IStaffRepository> _mockStaffRepo;
		private readonly Mock<IDoctorRepository> _mockDoctorRepo;
		private readonly Mock<IPatientRepository> _mockPatientRepo;
		private readonly Mock<IOperationTypeRepository> _mockTypeRepo;
		private readonly Mock<IAvailabilitySlotsRepository> _mockAvailRepo;
		private readonly Mock<IAssignedStaffRepository> _mockAssignedRepo;
		private readonly AppointmentService _apptSvc;
		private readonly List<OperationPhase> _opPhases;
		private readonly List<RequiredSpecialist> _reqSpec;
		private readonly OperationType _operationType;
		private readonly List<OperationType> _opTypeList;
		private readonly List<OperationTypeDTO> _opTypeDTOList;
		private readonly Token _tokenStaff;
		private readonly TokenDto _tokenDto;
		private readonly AppointmentDto _appointmentDTO;
		private readonly AppointmentDto _appointmentDTO2;
		private readonly Appointment _appointment;
		private readonly OperationRequest _opReq;
		private readonly OperationRoom _opRoom = new OperationRoom("c", new OperationRoomType(new OperationRoomTypeId("123"), new OperationRoomName("c"), new OperationRoomTypeDescription("what")), new List<AvailabilitySlot>()); 

		public AppointmentServiceTest()
		{
			_mockUnitOfWork = new Mock<IUnitOfWork>();
			_mockAppointmentRepo = new Mock<IAppointmentRepository>();
			_mockPatientRepo = new Mock<IPatientRepository>();
			_mockStaffRepo = new Mock<IStaffRepository>();
			_mockOpRoomRepo = new Mock<IOperationRoomRepository>();
			_mockRequestRepo = new Mock<IOperationRequestRepository>();
			_mockAvailRepo = new Mock<IAvailabilitySlotsRepository>();
			_mockAssignedRepo = new Mock<IAssignedStaffRepository>();

			_tokenDto = new TokenDto{
				TokenId = "c185d517-d467-4ba5-a789-eb4f77a194b1",
				TokenValue = TokenType.STAFF_AUTH_TOKEN.ToString(),
				ExpirationDate = DateTime.Now.ToString(),
				UserId = "12345678"
			};

			var _staff2 = new Staff(new LicenseNumber("128"), 
				new ContactInformation(new EmailAddress("johnbottles@nope.com"), new PhoneNumber("999111333")), new FullName("John Staffington"),
				new Specialization("specializatings"), new User(), new FirstName("John"), new LastName("Staffington"));

			_appointmentDTO = new AppointmentDto {
				id = "1",
				dateAndTime = "2013-12-10 13:00",
				appoitmentStatus = "SCHEDULED",
				staffId = "101",
				patientNumber = "MRN1031",
				operationRoom = "C",
				OperationRequestId = "abcdef",
			};

			_appointmentDTO2 = new AppointmentDto {
				id = "2",
				dateAndTime = "2013-12-10 13:00",
				appoitmentStatus = "SCHEDULED",
				staffId = "102",
				patientNumber = "MRN10",
				operationRoom = "D",
				OperationRequestId = "abcdef",
			};

			_reqSpec = [];
			_reqSpec.Add(new RequiredSpecialist(new Specialization("SpecializedInCopyright", ""), new SpecialistCount("10"), PhaseName.PREPARATION));
			_reqSpec.Add(new RequiredSpecialist(new Specialization("CopyrightedInSpecialization", ""), new SpecialistCount("10"), PhaseName.SURGERY));
			_reqSpec.Add(new RequiredSpecialist(new Specialization("CopyrightSpecialized", ""), new SpecialistCount("10"), PhaseName.CLEANING));

			_opPhases = [];
			_opPhases.Add(new OperationPhase(PhaseName.PREPARATION, 15));
			_opPhases.Add(new OperationPhase(PhaseName.SURGERY, 35));
			_opPhases.Add(new OperationPhase(PhaseName.CLEANING, 15));



			var _patient = new Patient(new MedicalRecordNumber("MRN10"), new FirstName("John"), new LastName("Bottles"), new FullName("John Bottles"), Gender.MALE, new DateOfBirth("2024-02-20"),
			new ContactInformation(new EmailAddress("johnbottles@nope.com"), new PhoneNumber("999111333")), new PhoneNumber("999111333")); 
			
			_opReq = new(new Doctor(), _patient, new OperationType(new OperationTypeName("operatedTypings"), _opPhases, _reqSpec, new EstimatedDuration(70)), OperationPriority.LOW, DateTime.Now);
			
			var _appointment2 = new Appointment(new DateAndTime(DateTime.Today), AppointmentStatus.SCHEDULED, null, _patient, _opReq, _opRoom);

			_appointment = new Appointment(new DateAndTime(DateTime.Today), AppointmentStatus.SCHEDULED, new List<AssignedStaff>([new AssignedStaff(_staff2, _appointment2)]), _patient, _opReq, _opRoom);

			_appointment.requestId = _opReq.Id;
			List<Appointment> list = [];
			list.Add(_appointment);

			var _roomAvailabilityDto = new RoomAvailabilityDto {
				Value = "{100, 300}",
				OperationRoomName = "c",
				OperationRoomID = "123"
			};

			List<RoomAvailabilityDto> list2 = [_roomAvailabilityDto];
			List<OperationRoomDto> list3 = [_opRoom.ToDto()];

			var _staffDto = new StaffDto {
				LicenseNumber = "123",
				Email = "johnstaffington@nope.com",
				Phone = "999333666",
				FirstName = "John",
				Fullname = "John Staffington",
				LastName = "Staffington",
				Specialization = "specializatings",
				AvailabilitySlots = ["hi", "hi2"],
			};

			List<StaffDto> list4 = [_staffDto];

			_apptSvc = new AppointmentService(_mockUnitOfWork.Object, _mockAppointmentRepo.Object, _mockPatientRepo.Object,
			_mockStaffRepo.Object, _mockOpRoomRepo.Object, _mockRequestRepo.Object, _mockAvailRepo.Object,
			_mockAssignedRepo.Object);
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

		private static Token _token = new Token(new TokenId("c185d517-d467-4ba5-a789-eb4f77a194b1"), DateTime.Now, _userStaff, TokenType.STAFF_AUTH_TOKEN);

		private List<OperationRequest> _opReqList = new List<OperationRequest>();

		[Fact]
		public async Task CreateAppointment_TestSuccessful()
		{
			_mockRequestRepo.Setup(req => req.GetByIdAsync(It.IsAny<OperationRequestId>()))
				.ReturnsAsync(_opReq);

			_mockPatientRepo.Setup(p => p.GetByIdAsync(It.IsAny<MedicalRecordNumber>()))
				.ReturnsAsync(_patient);

			_mockOpRoomRepo.Setup(d => d.GetByIdAsync(It.IsAny<OperationRoomId>()))
				.ReturnsAsync(_opRoom);

			_mockStaffRepo.Setup(staff => staff.GetStaffByLicense(It.IsAny<LicenseNumber>()))
				.ReturnsAsync(_staff);

			_mockAssignedRepo.Setup(staff => staff.AddAsync(It.IsAny<AssignedStaff>()))
				.ReturnsAsync(new AssignedStaff(_staff, _appointment));


			AppointmentDto result = await _apptSvc.registerSurgeryAppointment("2015-01-20", "1","MRN12346", _opRoom.Id.Value, _opReq.Id.Value);

			Assert.Equal(result.OperationRequestId, _opReq.Id.Value);
            _mockRequestRepo.Verify(r => r.GetByIdAsync(It.IsAny<OperationRequestId>()), Times.Once);
            _mockPatientRepo.Verify(r => r.GetByIdAsync(It.IsAny<MedicalRecordNumber>()), Times.Once);
			_mockOpRoomRepo.Verify(r => r.GetByIdAsync(It.IsAny<OperationRoomId>()), Times.Once);
			_mockStaffRepo.Verify(r => r.GetStaffByLicense(It.IsAny<LicenseNumber>()), Times.Once);
			_mockAssignedRepo.Verify(staff => staff.AddAsync(It.IsAny<AssignedStaff>()), Times.Once);
        }

		[Fact]
		public async Task UpdateAppointment_TestSuccessful()
		{

			var asgStaff = new AssignedStaff(_staff, _appointment);

			_mockAppointmentRepo.Setup(app => app.GetAppointmentByID(It.IsAny<AppointmentID>()))
				.ReturnsAsync(_appointment);

			_mockOpRoomRepo.Setup(d => d.GetByIdAsync(It.IsAny<OperationRoomId>()))
				.ReturnsAsync(_opRoom);

			_mockStaffRepo.Setup(staff => staff.GetStaffByLicense(It.IsAny<LicenseNumber>()))
				.ReturnsAsync(_staff);

			_mockAssignedRepo.Setup(staff => staff.GetByIdAsync(It.IsAny<AssignedStaffID>()))
				.ReturnsAsync(asgStaff);

			_mockAssignedRepo.Setup(staff => staff.AddAsync(It.IsAny<AssignedStaff>()))
				.ReturnsAsync(asgStaff);


			AppointmentDto result = await _apptSvc.updateSurgeryAppointment(_appointment.Id.Value, "2016-01-05 09:00", "1", _opRoom.Id.Value);

			Assert.Equal("2016-01-05 09:00", result.dateAndTime);
            _mockAppointmentRepo.Verify(r => r.GetAppointmentByID(It.IsAny<AppointmentID>()), Times.Once);
			_mockOpRoomRepo.Verify(r => r.GetByIdAsync(It.IsAny<OperationRoomId>()), Times.Once);
			_mockStaffRepo.Verify(r => r.GetStaffByLicense(It.IsAny<LicenseNumber>()), Times.Once);
			_mockAssignedRepo.Verify(staff => staff.GetByIdAsync(It.IsAny<AssignedStaffID>()), Times.Once);
			_mockAssignedRepo.Verify(staff => staff.AddAsync(It.IsAny<AssignedStaff>()), Times.Once);
        }
	}
}
