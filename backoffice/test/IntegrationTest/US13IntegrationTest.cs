using DDDSample1.Controllers;
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
using Moq;

namespace DDDNetCore.Test.IntegrationTest
{
	public class US13IntegrationTest
	{

		private readonly Mock<UserService> _mockUserService;
		private readonly Mock<TokenService> _mockTokenService;
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
		private readonly StaffController _ctrl;
		private readonly User _user;
		private readonly Staff _staff;
		private readonly TokenDto _tokenDto;
		private static Token? _token;

		public US13IntegrationTest()
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
			_mockUserService = new();
			_mockTokenService = new();


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

			_staff = new(
				new LicenseNumber("12345678"),
				new ContactInformation(
					new EmailAddress("testEmail@hospital.com"),
					new PhoneNumber("911112999")),
				new FullName("John Doe"),
				new Specialization("Doctor", ""),
				_user,
				new FirstName("John"),
				new LastName("Doe")
			);

			_token = new Token(new TokenId("c185d517-d467-4ba5-a789-eb4f77a194b1"), DateTime.Now, _user, TokenType.STAFF_AUTH_TOKEN);

			_tokenDto = new TokenDto
			{
				TokenId = "c185d517-d467-4ba5-a789-eb4f77a194b1",
				TokenValue = TokenType.ADMIN_AUTH_TOKEN.ToString(),
				ExpirationDate = DateTime.Now.ToString(),
				UserId = "12345678"
			};

			List<StaffDto> list = [_staff.toDto()];

			Specialization sp = new Specialization(_staffDto.Specialization, "");

			_mockUserRepo.Setup(r => r.GetByIdAsync(It.IsAny<Username>())).ReturnsAsync(_user);
			_mockTokenService.Setup(s => s.GetByIdAsync(It.IsAny<TokenId>())).ReturnsAsync(_tokenDto);
			_mockSpecializationRepo.Setup(r => r.GetByName(It.IsAny<string>())).ReturnsAsync(sp);

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
			
			_ctrl = new StaffController(_service, _mockUserService.Object, _mockTokenService.Object);
		}


		[Fact]
		public async Task Test_US13_UpdateContact()
		{
			await _ctrl.CreateStaff(_staff.toDto(), _tokenDto.TokenId);

			_mockUserRepo.Verify(r => r.GetByIdAsync(It.IsAny<Username>()), Times.Once);
			_mockStaffRepo.Verify(r => r.AddAsync(It.IsAny<Staff>()), Times.Once);
			_mockSpecializationRepo.Verify(r => r.GetByName(It.IsAny<string>()), Times.Once);
			_mockWorkUnit.Verify(w => w.CommitAsync(), Times.Once);
		}

		[Fact]
		public async Task Test_US13_UpdateContactError()
		{
			TokenDto dto = new()
			{
				TokenId = "c185d517-d467-4ba5-a789-eb4f77a194b1",
				TokenValue = TokenType.STAFF_AUTH_TOKEN.ToString(),
				ExpirationDate = DateTime.Now.ToString(),
				UserId = "12345678"
			};

			_mockTokenService.Setup(s => s.GetByIdAsync(It.IsAny<TokenId>())).ReturnsAsync(dto);

			await _ctrl.CreateStaff(_staff.toDto(), dto.TokenId);

			_mockUserRepo.Verify(r => r.GetByIdAsync(It.IsAny<Username>()), Times.Never);
		}
	}
}