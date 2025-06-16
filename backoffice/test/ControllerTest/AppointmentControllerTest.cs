using DDDSample1.Application.AvailabilitySlots;
using DDDSample1.Controllers;
using DDDSample1.Domain.AssignedStaffs;
using DDDSample1.Domain.AvailabilitySlots;
using DDDSample1.Domain.ContactInformations;
using DDDSample1.Domain.Doctors;
using DDDSample1.Domain.HospitalAppointment;
using DDDSample1.Domain.HospitalPatient;
using DDDSample1.Domain.HospitalStaff;
using DDDSample1.Domain.OperationPhases;
using DDDSample1.Domain.OperationRequests;
using DDDSample1.Domain.OperationRooms;
using DDDSample1.Domain.OperationRoomTypes;
using DDDSample1.Domain.OperationTypes;
using DDDSample1.Domain.RequiredSpecialists;
using DDDSample1.Domain.Shared;
using DDDSample1.Domain.Specializations;
using DDDSample1.Domain.Tokens;
using DDDSample1.Domain.Users;
using DDDSample1.Domain.ValueObjects;
using DDDSample1.DTO;
using Moq;
using Org.BouncyCastle.Crypto.Engines;

namespace DDDNetCore.test.ControllerTest
{
	public class AppointmentControllerTest
	{
		private readonly Mock<UserService> _mockUserService;
		private readonly Mock<TokenService> _mockTokenService;
		private readonly Mock<AppointmentService> _mockAppointmentService;
		private readonly AppointmentController _ctrl;
		private readonly TokenDto _tokenDto;
		private readonly AppointmentDto _appointmentDTO;
		private readonly AppointmentDto _appointmentDTO2;
		private readonly Appointment _appointment;
		private readonly Patient _patient;
		private readonly List<OperationPhase> _opPhases;
		private readonly List<RequiredSpecialist> _reqSpec;
		private readonly OperationRequest _opReq;
		public AppointmentControllerTest()
		{
			_mockTokenService = new();
			_mockUserService = new();
			_mockAppointmentService = new();
			_ctrl = new(_mockTokenService.Object, _mockAppointmentService.Object);


			_tokenDto = new TokenDto{
				TokenId = "c185d517-d467-4ba5-a789-eb4f77a194b1",
				TokenValue = TokenType.STAFF_AUTH_TOKEN.ToString(),
				ExpirationDate = DateTime.Now.ToString(),
				UserId = "12345678"
			};

			var _staff = new Staff(new LicenseNumber("128"), 
				new ContactInformation(new EmailAddress("johnbottles@nope.com"), new PhoneNumber("111111111111111")), new FullName("John Staffington"),
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

			var _opRoom = new OperationRoom("c", new OperationRoomType(new OperationRoomTypeId("123"), new OperationRoomName("c"), new OperationRoomTypeDescription("what")), new List<AvailabilitySlot>()); 


			_patient = new Patient(new MedicalRecordNumber("MRN10"), new FirstName("John"), new LastName("Bottles"), new FullName("John Bottles"), Gender.MALE, new DateOfBirth("2024-02-20"),
			new ContactInformation(new EmailAddress("johnbottles@nope.com"), new PhoneNumber("1111111111111")), new PhoneNumber("999111333")); 
			
			_opReq = new(new Doctor(), _patient, new OperationType(new OperationTypeName("operatedTypings"), _opPhases, _reqSpec, new EstimatedDuration(70)), OperationPriority.LOW, DateTime.Now);
			
			var _appointment2 = new Appointment(new DateAndTime(DateTime.Today), AppointmentStatus.SCHEDULED, null, _patient, _opReq, _opRoom);

			_appointment = new Appointment(new DateAndTime(DateTime.Today), AppointmentStatus.SCHEDULED, new List<AssignedStaff>([new AssignedStaff(_staff, _appointment2)]), _patient, _opReq, _opRoom);

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

			_mockTokenService.Setup(s => s.GetByIdAsync(It.IsAny<TokenId>())).ReturnsAsync(_tokenDto);

			_mockAppointmentService.Setup(s => s.GetAllAsync()).ReturnsAsync(list);
			_mockAppointmentService.Setup(s => s.GetByRoomID(It.IsAny<string>())).ReturnsAsync(_appointmentDTO);	
			_mockAppointmentService.Setup(s => s.registerSurgeryAppointment(
				It.IsAny<string>(),
				It.IsAny<string>(),
				It.IsAny<string>(),
				It.IsAny<string>(),
				It.IsAny<string>()
			)).ReturnsAsync(_appointmentDTO);

			_mockAppointmentService.Setup(s => s.updateSurgeryAppointment(
				It.IsAny<string>(),
				It.IsAny<string>(),
				It.IsAny<string>(),
				It.IsAny<string>()
			)).ReturnsAsync(_appointmentDTO);

			_mockAppointmentService.Setup(s => s.GetStaffByTimeSlot(
				It.IsAny<string>(),
				It.IsAny<string>()
			)).ReturnsAsync(list4);

		}

		[Fact]
		public async Task Test_AppointmentController_GetAppointments()
		{
			TokenDto dto = new()
			{
				TokenId = "c185d517-d467-4ba5-a789-eb4f77a194b1",
				TokenValue = TokenType.STAFF_AUTH_TOKEN.ToString(),
				ExpirationDate = DateTime.Now.ToString(),
				UserId = "12345678"
			};
			
			_mockTokenService.Setup(s => s.GetByIdAsync(It.IsAny<TokenId>())).ReturnsAsync(dto);

			await _ctrl.GetAppointments(_tokenDto.TokenId);
		
			_mockAppointmentService.Verify(s => s.GetAllAsync(), Times.Once);
		}

		[Fact]
		public async Task Test_AppointmentController_CreateAppointment()
		{
			TokenDto dto = new()
			{
				TokenId = "c185d517-d467-4ba5-a789-eb4f77a194b1",
				TokenValue = TokenType.STAFF_AUTH_TOKEN.ToString(),
				ExpirationDate = DateTime.Now.ToString(),
				UserId = "12345678"
			};

			_mockTokenService.Setup(s => s.GetByIdAsync(It.IsAny<TokenId>())).ReturnsAsync(dto);

			await _ctrl.CreateAppointment("2024-10-25", "101", "MRN10", "C", "1", _tokenDto.TokenId);

			_mockAppointmentService.Verify(s => s.registerSurgeryAppointment(It.IsAny<string>(),
				It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
		}

		[Fact]
		public async Task Test_AppointmentController_UpdateAppointment()
		{
			TokenDto dto = new()
			{
				TokenId = "c185d517-d467-4ba5-a789-eb4f77a194b1",
				TokenValue = TokenType.STAFF_AUTH_TOKEN.ToString(),
				ExpirationDate = DateTime.Now.ToString(),
				UserId = "12345678"
			};
			
			_mockTokenService.Setup(s => s.GetByIdAsync(It.IsAny<TokenId>())).ReturnsAsync(dto);
			
			await _ctrl.UpdateAppointment("1","2024-10-25", "101", "C", _tokenDto.TokenId);

			_mockAppointmentService.Verify(s => s.updateSurgeryAppointment(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
		}


		[Fact]
		public async Task Test_AppointmentController_GetStaffByTimeSlot()
		{
			TokenDto dto = new()
			{
				TokenId = "c185d517-d467-4ba5-a789-eb4f77a194b1",
				TokenValue = TokenType.STAFF_AUTH_TOKEN.ToString(),
				ExpirationDate = DateTime.Now.ToString(),
				UserId = "12345678"
			};
			
			_mockTokenService.Setup(s => s.GetByIdAsync(It.IsAny<TokenId>())).ReturnsAsync(dto);

			await _ctrl.GetStaffByTimeSlot("123", "2024-12-20", _tokenDto.TokenId);

			_mockAppointmentService.Verify(s => s.GetStaffByTimeSlot(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
		}

		[Fact]
		public async Task Test_AppointmentController_GetAppointmentByRoom()
		{
			TokenDto dto = new()
			{
				TokenId = "c185d517-d467-4ba5-a789-eb4f77a194b1",
				TokenValue = TokenType.STAFF_AUTH_TOKEN.ToString(),
				ExpirationDate = DateTime.Now.ToString(),
				UserId = "12345678"
			};
			
			_mockTokenService.Setup(s => s.GetByIdAsync(It.IsAny<TokenId>())).ReturnsAsync(dto);

			await _ctrl.GetAppointmentByRoom("C", _tokenDto.TokenId);

			_mockAppointmentService.Verify(s => s.GetByRoomID(It.IsAny<string>()), Times.Once);
		}

	}
}