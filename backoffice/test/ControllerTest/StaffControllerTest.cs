using DDDSample1.Controllers;
using DDDSample1.Domain.ContactInformations;
using DDDSample1.Domain.HospitalStaff;
using DDDSample1.Domain.Shared;
using DDDSample1.Domain.Specializations;
using DDDSample1.Domain.Tokens;
using DDDSample1.Domain.Users;
using DDDSample1.Domain.ValueObjects;
using Moq;

namespace DDDNetCore.test.ControllerTest
{
	public class StaffControllerTest
	{
		private readonly Mock<StaffService> _mockStaffService;
		private readonly Mock<UserService> _mockUserService;
		private readonly Mock<TokenService> _mockTokenService;
		private readonly StaffController _ctrl;
		private readonly User _user;
		private readonly Staff _staff;
		private readonly TokenDto _tokenDto;

		public StaffControllerTest()
		{
			_mockStaffService = new();
			_mockUserService = new();
			_mockTokenService = new();

			_user = new User(
				"testEmail@hospital.com",
				"P@ssw0rd",
				UserRole.STAFF
			);

			_staff = new(
				new LicenseNumber("12345678"),
				new ContactInformation(
					new EmailAddress("testEmail@hospital.com"),
					new PhoneNumber("11111111111")),
				new FullName("John Doe"),
				new Specialization("oldSpecialization", ""),
				_user,
				new FirstName("John"),
				new LastName("Doe")
			);

			_tokenDto = new TokenDto{
				TokenId = "c185d517-d467-4ba5-a789-eb4f77a194b1",
				TokenValue = TokenType.ADMIN_AUTH_TOKEN.ToString(),
				ExpirationDate = DateTime.Now.ToString(),
				UserId = "12345678"
			};

			List<StaffDto> list = [_staff.toDto()];

			_mockTokenService.Setup(s => s.GetByIdAsync(It.IsAny<TokenId>())).ReturnsAsync(_tokenDto);
			_mockStaffService.Setup(s => s.GetStaffList()).ReturnsAsync(list);
			_mockStaffService.Setup(s => s.RegisterStaff(It.IsAny<StaffDto>())).ReturnsAsync(_staff.toDto());

			_ctrl = new StaffController(_mockStaffService.Object,
				_mockUserService.Object, _mockTokenService.Object);
		}

		[Fact]
		public async Task Test_StaffControlerTest_List()
		{
			await _ctrl.ListAll(_tokenDto.TokenId);

			_mockStaffService.Verify(s => s.GetStaffList(), Times.Once);
		}

		[Fact]
		public async Task Test_StaffControllerTest_ListError()
		{
			TokenDto dto = new()
			{
				TokenId = "c185d517-d467-4ba5-a789-eb4f77a194b1",
				TokenValue = TokenType.STAFF_AUTH_TOKEN.ToString(),
				ExpirationDate = DateTime.Now.ToString(),
				UserId = "12345678"
			};
			_mockTokenService.Setup(s => s.GetByIdAsync(It.IsAny<TokenId>())).ReturnsAsync(dto);

			await _ctrl.ListAll(dto.TokenId);

			_mockStaffService.Verify(s => s.GetStaffList(), Times.Never);
		}

		[Fact]
		public async Task Test_StaffControllerTest_Create()
		{
			await _ctrl.CreateStaff(_staff.toDto(), _tokenDto.TokenId);

			_mockStaffService.Verify(s => s.RegisterStaff(It.IsAny<StaffDto>()), Times.Once);
		}

		[Fact]
		public async Task Test_StaffControllerTest_CreateError()
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

			_mockStaffService.Verify(s => s.RegisterStaff(It.IsAny<StaffDto>()), Times.Never);
		}
	}
}