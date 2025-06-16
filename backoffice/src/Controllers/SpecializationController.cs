using System.Collections.Generic;
using System.Threading.Tasks;
using DDDSample1.Domain.Specializations;
using DDDSample1.Domain.Tokens;
using Microsoft.AspNetCore.Mvc;

namespace DDDSample1.Controllers {
	[ApiController]
	[Route("api/[controller]")]
	public class SpecializationController : ControllerBase
	{

		private readonly TokenService _tkService;
		private readonly SpecializationService _service;

		public SpecializationController(TokenService tokenService, SpecializationService service)
		{
			_tkService = tokenService;
			_service = service;
		}

		[HttpGet("GetSpecializationList")]
		public async Task<ActionResult<IEnumerable<SpecializationDTO>>> GetAll(
			[FromHeader] string token
		)
		{
			if (!AuthAdmin(token).Result)
				return BadRequest("ACCESS DENIED");
			
			return await _service.GetAll();
		}

		[HttpGet("FilterSpecializationList")]
		public async Task<ActionResult<SpecializationDTO>> FilteredSearch(
			[FromQuery] string code,
			[FromQuery] string name,
			[FromHeader] string token
		)
		{
			if (!AuthAdmin(token).Result)
				return BadRequest("ACCESS DENIED");

			return await _service.FilteredGet(code, name);
		}

		[HttpPatch("EditSpecialization/{id}")]
		public async Task<ActionResult<SpecializationDTO>> EditSpecialization(string id,
			[FromBody] SpecializationDTO dto,
			// [FromQuery] string code,
			/* [FromQuery] string name,
			[FromQuery] string description, */
			[FromHeader] string token
		)
		{
			if (!AuthAdmin(token).Result)
				return BadRequest("ACCESS DENIED");
			
			return await _service.UpdateSpecialization(id, dto.SpecializationName, dto.SpecializationDescription);
		}

		[HttpPost("CreateSpecialization")]
		public async Task<ActionResult<SpecializationDTO>> CreateSpecialization(
			[FromBody] SpecializationDTO dto,
			/* [FromQuery] string code,
			[FromQuery] string name,
			[FromQuery] string description, */
			[FromHeader] string token
		)
		{
			if (!AuthAdmin(token).Result)
				return BadRequest("ACCESS DENIED");

			return await _service.CreateSpecialization(dto.SpecializationCode, dto.SpecializationName, dto.SpecializationDescription);
		}

		[HttpDelete("DeleteSpecialization/{id}")]
		public async Task<ActionResult<SpecializationDTO>> RemoveSpecialization(string id,
			[FromHeader] string token
		)
		{
			if (!AuthAdmin(token).Result)
				return BadRequest("ACCESS DENIED");

			return await _service.DeleteSpecialization(id);
		}

		private async Task<bool> AuthAdmin(string token)
		{
			TokenDto tokenDto = await _tkService.GetByIdAsync(new TokenId(token));
			if (tokenDto.TokenValue != TokenType.ADMIN_AUTH_TOKEN.ToString())
				return false;
			else
				return true;
		}
	}
}