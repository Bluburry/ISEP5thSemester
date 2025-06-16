using DDDSample1.Controllers;
using DDDSample1.Domain.HospitalStaff;
using DDDSample1.Domain.OperationRequests;
using DDDSample1.Domain.Tokens;
using DDDSample1.Domain.Users;
using Moq;
using Org.BouncyCastle.Crypto.Engines;

namespace DDDNetCore.test.ControllerTest
{
	public class OperationRequestControllerTest
	{
		private readonly Mock<OperationRequestService> _mockRequestService;
		private readonly Mock<UserService> _mockUserService;
		private readonly Mock<TokenService> _mockTokenService;
		private readonly OperationRequestController _ctrl;
		private readonly TokenDto _tokenDto;
		private readonly OperationRequestDTO _requestDTO;

		public OperationRequestControllerTest()
		{
			_mockRequestService = new();
			_mockUserService = new();
			_mockTokenService = new();


			_tokenDto = new TokenDto{
				TokenId = "c185d517-d467-4ba5-a789-eb4f77a194b1",
				TokenValue = TokenType.STAFF_AUTH_TOKEN.ToString(),
				ExpirationDate = DateTime.Now.ToString(),
				UserId = "12345678"
			};

			_requestDTO = new OperationRequestDTO {
				ID = "1",
				Doctor = "doctor1",
				Patient = "patient1",
				OperationType = "operation1",
				OperationDeadline = "2025/10/23",
				OperationPriority = "low"
			};
			
			_mockTokenService.Setup(s => s.GetByIdAsync(It.IsAny<TokenId>())).ReturnsAsync(_tokenDto);
			_mockRequestService
				.Setup(s => s.CreateRequest(
					It.IsAny<string>(), It.IsAny<string>(),
					It.IsAny<string>(), It.IsAny<string>(),
					It.IsAny<TokenDto>()))
				.ReturnsAsync(_requestDTO);
			
			_mockRequestService
				.Setup(r => r.DeleteOperationRequest(
					It.IsAny<string>(), It.IsAny<TokenDto>()))
				.ReturnsAsync(_requestDTO);
			
			List<OperationRequestDTO> list = [_requestDTO];

			_mockRequestService
				.Setup(s => s.ListOperationRequest(
					It.IsAny<string>(), It.IsAny<string>(),
					It.IsAny<string>(), It.IsAny<string>(),
					It.IsAny<TokenDto>()))
				.ReturnsAsync(list);

			_ctrl = new OperationRequestController(_mockRequestService.Object, _mockUserService.Object, _mockTokenService.Object, null, null);
		}

		[Fact]
		public async Task Test_OperationRequestController_CreateError()
		{
			TokenDto dto = new()
			{
				TokenId = "c185d517-d467-4ba5-a789-eb4f77a194b1",
				TokenValue = TokenType.ADMIN_AUTH_TOKEN.ToString(),
				ExpirationDate = DateTime.Now.ToString(),
				UserId = "12345678"
			};
			_mockTokenService.Setup(s => s.GetByIdAsync(It.IsAny<TokenId>())).ReturnsAsync(dto);

			await _ctrl.CreateOperationRequest("name", "heart surgery", "2025/10/29", "low", _tokenDto.TokenId);
			
			_mockRequestService.Verify(r => r.CreateRequest(It.IsAny<string>(),
				It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TokenDto>()), Times.Never);
		}

		[Fact]
		public async Task Test_OperationRequestController_Create()
		{
			await _ctrl.CreateOperationRequest("name", "heart surgery", "2025/10/29", "low", _tokenDto.TokenId);

			_mockRequestService.Verify(r => r.CreateRequest(It.IsAny<string>(),
				It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TokenDto>()), Times.Once);
		}

		[Fact]
		public async Task Test_OperationRequestController_List()
		{
			await _ctrl.ListOperationRequest(
				_requestDTO.Patient, _requestDTO.OperationType,
				_requestDTO.OperationPriority, _requestDTO.OperationPriority,
				_tokenDto.TokenId);

			_mockRequestService.Verify(r => r.ListOperationRequest(It.IsAny<string>(),
				It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TokenDto>()), Times.Once);
		}

		[Fact]
		public async Task Test_OperationRequestController_Delete()
		{
			await _ctrl.DeleteOperationRequest(
				_requestDTO.Patient, _tokenDto.TokenId);

			_mockRequestService.Verify(r => r.DeleteOperationRequest(
				It.IsAny<string>(), It.IsAny<TokenDto>()), Times.Once);
		}

	}
}