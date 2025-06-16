
using DDDSample1.Domain.AvailabilitySlots;
using DDDSample1.Domain.ContactInformations;
using DDDSample1.Domain.Doctors;
using DDDSample1.Domain.HospitalStaff;
using DDDSample1.Domain.Logs;
using DDDSample1.Domain.Shared;
using DDDSample1.Domain.Specializations;
using DDDSample1.Domain.Tokens;
using DDDSample1.Domain.Users;
using DDDSample1.Domain.ValueObjects;
using DDDSample1.DTO;
using Moq;

namespace DDDNetCore.test.ServiceTest
{
	public class StaffServiceTest
	{

		private readonly Mock<IUnitOfWork> _mockWorkUnit;
		private readonly Mock<IStaffRepository> _mockStaffRepo;
		private readonly Mock<IUserRepository> _mockUserRepo;
		private readonly Mock<ISpecializationRepository> _mockSpecializationRepo;
		private readonly Mock<ITokenRepository> _mockTokenRepo;
		private readonly Mock<IContactInformationRepository> _mockContactRepo;
		private readonly Mock<ILogRepository> _mockLogRepo;
		private readonly Mock<IAvailabilitySlotsRepository> _mockSlotsRepo;
		private readonly Mock<IDoctorRepository> _mockDocRepo;
		private readonly StaffService _service;
		private readonly StaffDto _staffDto;
		private readonly User _user;

		public StaffServiceTest()
		{
			_mockWorkUnit = new Mock<IUnitOfWork>();
			_mockStaffRepo = new Mock<IStaffRepository>();
			_mockUserRepo = new Mock<IUserRepository>();
			_mockSpecializationRepo = new Mock<ISpecializationRepository>();
			_mockTokenRepo = new Mock<ITokenRepository>();
			_mockContactRepo = new Mock<IContactInformationRepository>();
			_mockLogRepo = new Mock<ILogRepository>();
			_mockSlotsRepo = new Mock<IAvailabilitySlotsRepository>();
			_mockDocRepo = new Mock<IDoctorRepository>();

			_staffDto = new StaffDto
			{
				LicenseNumber = "123456789",
				Email = "testEmail@hospital.com",
				Phone = "911112999",
				FirstName = "John",
				LastName = "Doe",
				Specialization = "testSpecialization"
			};

			_user = new User(
				_staffDto.Email,
				"P@ssw0rd",
				UserRole.STAFF
			);

			_service = new StaffService(
				_mockWorkUnit.Object,
				_mockStaffRepo.Object,
				_mockSpecializationRepo.Object,
				_mockUserRepo.Object,
				_mockLogRepo.Object,
				_mockSlotsRepo.Object,
				_mockContactRepo.Object,
				_mockTokenRepo.Object,
				_mockDocRepo.Object
			);
		}

		[Fact]
		public async Task Test_StaffService_RegisterStaffSuccessful()
		{
			StaffBuilder builder = new();
			Specialization sp = new Specialization(_staffDto.Specialization, "");

			Staff staff =
			builder.WithFullName(_staffDto.FirstName + " " + _staffDto.LastName)
				.WithContactInformation(_staffDto.Phone, _staffDto.Email)
				.WithLicenseNumber(_staffDto.LicenseNumber)
				.WithSpecialization(sp)
				.WithUser(_user)
				.WithFirstName(_staffDto.FirstName)
				.WithLastNAme(_staffDto.LastName)
				.Build();

			_mockUserRepo.Setup(r => r.GetByIdAsync(It.IsAny<Username>())).ReturnsAsync(_user);
			_mockSpecializationRepo.Setup(r => r.GetByName(It.IsAny<string>())).ReturnsAsync(sp);
			// _mockWorkUnit.Setup(w => w.CommitAsync());

			// _mockStaffRepo.Setup(r => r.AddAsync(It.IsAny<Staff>())).ReturnsAsync(staff);

			StaffDto dto = await _service.RegisterStaff(_staffDto);

			Assert.NotEqual("User is null", dto.Email);
			Assert.Equal(staff.Id.AsString(), dto.LicenseNumber);
			_mockStaffRepo.Verify(r => r.AddAsync(It.IsAny<Staff>()), Times.Once);
			_mockSpecializationRepo.Verify(r => r.GetByName(It.IsAny<string>()), Times.Once);
			_mockWorkUnit.Verify(w => w.CommitAsync(), Times.Once);
		}

		[Fact]
		public async Task Test_StaffService_RegisterStaffUnsuccessful()
		{
			var dto = new StaffDto
			{
				LicenseNumber = "123456789",
				Email = "email@hospital.com",
				Phone = "911112999",
				FirstName = "John",
				LastName = "Doe",
				Specialization = "testSpecialization"
			};

			StaffDto ret = await _service.RegisterStaff(dto);

			Assert.Equal("User is null", ret.Email);
		}

		[Fact]
		public async Task Test_StaffService_UpdateSuccessful()
		{
			Staff oldStaff = new(
				new LicenseNumber("12345678"),
				new ContactInformation(
					new EmailAddress("testEmail@hospital.com"),
					new PhoneNumber("911112999")),
				new FullName("John Doe"),
				new Specialization("oldSpecialization", ""),
				_user,
				new FirstName("John"),
				new LastName("Doe")
			);

			Staff newStaff = new(
				new LicenseNumber("12345678"),
				new ContactInformation(
					new EmailAddress("testEmail@hospital.com"),
					new PhoneNumber("911112999")),
				new FullName("John Doe"),
				new Specialization("newSpecialization", ""),
				_user,
				new FirstName("John"),
				new LastName("Doe")
			);

			Log log = new()
			{
				loggedType = ObjectLoggedType.STAFF,
				LoggedInformation = oldStaff.ToString(),
				LoggedDate = new DateAndTime(DateTime.Now),
				LoggedId = oldStaff.Id.AsString()
			};

			_mockStaffRepo.Setup(r => r.GetByIdAsync(It.IsAny<LicenseNumber>()))
				.ReturnsAsync(oldStaff);
			_mockStaffRepo.Setup(r => r.Update(It.IsAny<Staff>()))
				.Returns(newStaff);
			_mockSpecializationRepo.Setup(r => r.GetByName(It.IsAny<string>()))
				.ReturnsAsync(new Specialization("newSpecialization", ""));
			_mockLogRepo.Setup(r => r.AddAsync(It.IsAny<Log>()))
				.ReturnsAsync(log);

			// _mockWorkUnit.Setup(w => w.CommitAsync());

			Log dto = await _service.UpdateAsync(newStaff.toDto());

			Assert.Equal(log.Id, dto.Id);
			_mockStaffRepo.Verify(r => r.Update(It.IsAny<Staff>()), Times.Once);
			_mockLogRepo.Verify(r => r.AddAsync(It.IsAny<Log>()), Times.Once);
			_mockSpecializationRepo.Verify(r => r.GetByName(It.IsAny<string>()), Times.Once);
			_mockWorkUnit.Verify(w => w.CommitAsync(), Times.Once);
		}

		[Fact]
		public async Task Test_StaffService_UpdateUnsuccessful()
		{
			Staff oldStaff = new(
				new LicenseNumber("12345678"),
				new ContactInformation(
					new EmailAddress("testEmail@hospital.com"),
					new PhoneNumber("911112999")),
				new FullName("John Doe"),
				new Specialization("oldSpecialization", ""),
				_user,
				new FirstName("John"),
				new LastName("Doe")
			);

			Staff newStaff = new(
				new LicenseNumber("12345678"),
				new ContactInformation(
					new EmailAddress("testEmail@hospital.com"),
					new PhoneNumber("911112999")),
				new FullName("John Doe"),
				new Specialization("newSpecialization", ""),
				_user,
				new FirstName("John"),
				new LastName("Doe")
			);

			Log log = new()
			{
				loggedType = ObjectLoggedType.STAFF,
				LoggedInformation = oldStaff.ToString(),
				LoggedDate = new DateAndTime(DateTime.Now),
				LoggedId = oldStaff.Id.AsString()
			};

			_mockStaffRepo.Setup(r => r.GetByIdAsync(It.IsAny<LicenseNumber>()))
				.ReturnsAsync(oldStaff);
			_mockStaffRepo.Setup(r => r.Update(It.IsAny<Staff>()))
				.Returns(newStaff);
			_mockLogRepo.Setup(r => r.AddAsync(It.IsAny<Log>()))
				.ReturnsAsync(log);

			//_mockWorkUnit.Setup(w => w.CommitAsync());

			var exception = await Assert.ThrowsAsync<ArgumentException>(async () => await _service.UpdateAsync(newStaff.toDto()));
			Assert.Equal("The specialization given does not exist.", exception.Message);
		}

		[Fact]
		public async Task Test_StaffService_DisableStaff()
		{
			Staff newStaff = new(
				new LicenseNumber("12345678"),
				new ContactInformation(
					new EmailAddress("testEmail@hospital.com"),
					new PhoneNumber("911112999")),
				new FullName("John Doe"),
				new Specialization("newSpecialization", ""),
				_user,
				new FirstName("John"),
				new LastName("Doe")
			);

			_mockStaffRepo.Setup(r => r.GetStaffByLicense(It.IsAny<LicenseNumber>()))
				.ReturnsAsync(newStaff);

			StaffDto staffDto = await _service.DisableStaff(newStaff.Id.AsString());

			_mockStaffRepo.Verify(r => r.GetStaffByLicense(It.IsAny<LicenseNumber>()), Times.Once);
			_mockWorkUnit.Verify(w => w.CommitAsync(), Times.Once);
		}

		[Fact]
		public void Test_StaffService_FilterStaff()
		{
			Staff oldStaff = new(
				new LicenseNumber("12345678"),
				new ContactInformation(
					new EmailAddress("testEmail@hospital.com"),
					new PhoneNumber("911112999")),
				new FullName("John Doe"),
				new Specialization("Specialization", ""),
				_user,
				new FirstName("John"),
				new LastName("Doe")
			);

			Staff newStaff = new(
				new LicenseNumber("12345677"),
				new ContactInformation(
					new EmailAddress("emailTest@hospital.com"),
					new PhoneNumber("911112888")),
				new FullName("John Doe"),
				new Specialization("Specialization", ""),
				_user,
				new FirstName("John"),
				new LastName("Doe")
			);

			IEnumerable<Staff> list = [oldStaff, newStaff];

			_mockStaffRepo.Setup(r => r.GetFilteredStaff(It.IsAny<QueryDataDto>()))
				.Returns(list);

			IEnumerable<StaffDto> dto = _service.GetFilteredStaff(null, null, null, "Specialization", null);

			/* StaffDto oldDto = oldStaff.toDto();
			StaffDto newDto = newStaff.toDto();

			Assert.Contains(oldDto, dto);
			Assert.Contains(newDto, dto); */
			Assert.Equal(2, dto.Count());
			_mockStaffRepo.Verify(r => r.GetFilteredStaff(It.IsAny<QueryDataDto>()), Times.Once);
		}
	}
}