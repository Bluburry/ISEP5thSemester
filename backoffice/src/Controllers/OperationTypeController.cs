using Microsoft.AspNetCore.Mvc;
using DDDSample1.Domain.OperationTypes;
using System.Threading.Tasks;
using System.Collections.Generic;
using DDDSample1.Domain.ValueObjects;
using System;
using DDDSample1.Domain.Tokens;

namespace DDDSample1.Controllers
{

	[ApiController]
	[Route("api/[controller]")]
	public class OperationTypeController : ControllerBase
	{
		private readonly OperationTypeService _service;
		private readonly TokenService _tkService;

		public OperationTypeController(OperationTypeService service, TokenService tokenService)
		{
			_service = service;
			_tkService = tokenService;
		}

		//[HttpGet]
		/* public async Task<ActionResult<IEnumerable<OperationTypeDTO>>> GetAll(
			[FromHeader] string token
		)
		{
			if (!AuthAdmin(token).Result)
				return BadRequest("ACCESS DENIED");
			return await _service.GetAllAsync();
		} */

		[HttpGet("ListForOperation")]
		public async Task<ActionResult<IEnumerable<OperationTypeDTO>>> GetAllForDoctor(
			[FromHeader] string token
		)
		{
			TokenDto tokenDto = await _tkService.GetByIdAsync(new TokenId(token));
			if (tokenDto.TokenValue != TokenType.STAFF_AUTH_TOKEN.ToString())
				return BadRequest("ACCESS DENIED");

			return await _service.GetAllAsync();
		}


		// [HttpGet("{id}")]
		/* public async Task<ActionResult<OperationTypeDTO>> GetById(string id,
			[FromHeader] string token
		)
		{
			if (!AuthAdmin(token).Result)
				return BadRequest("ACCESS DENIED");
			var op = await _service.GetByIDAsync(new OperationTypeID(id));

			if (op == null)
			{
				return NotFound();
			}

			return op;
		} */

		[HttpGet("name/{operationName}")]
		public async Task<ActionResult<OperationTypeDTO>> GetByName(string operationName,
			[FromHeader] string token
		)
		{
			if (!AuthAdmin(token).Result)
				return BadRequest("ACCESS DENIED");
			var op = await _service.GetByNameAsync(new OperationTypeName(operationName));

			if (op == null)
			{
				return NotFound();
			}

			return op;
		}

		[HttpGet("allofname/{operationName}")]
		public async Task<ActionResult<IEnumerable<OperationTypeDTO>>> GetAllByName(string operationName,
			[FromHeader] string token
		)
		{
			if (!AuthAdmin(token).Result)
				return BadRequest("ACCESS DENIED");
			return await _service.GetAllByNameAsync(new OperationTypeName(operationName));
		}

		/* [HttpGet("deactivate/{operationName}")]
		public async Task<ActionResult<IEnumerable<OperationTypeDTO>>> DeactivateByName(string operationName,
			[FromHeader] string token
		)
		{
			if (!AuthAdmin(token).Result)
				return BadRequest("ACCESS DENIED");
			return await _service.GetAllByNameAsync(new OperationTypeName(operationName));
		} */

		[HttpGet("filterOperationType")]
		public async Task<ActionResult<IEnumerable<OperationTypeDTO>>> FilteredSearch(
			[FromQuery] string operationName,
			[FromQuery] string specialization,
			[FromQuery] string activeStatus,
			[FromHeader] string token
		)
		{
			if (!AuthAdmin(token).Result)
				return BadRequest("ACCESS DENIED");

			return await _service.FilteredGet(operationName, specialization, activeStatus);
		}

		[HttpPost("createOperation")]
		// [HttpPut]
		public async Task<ActionResult<OperationTypeDTO>> Create(OperationTypeDTO dto,
			[FromHeader] string token
		)
		{
			if (!AuthAdmin(token).Result)
				return BadRequest("ACCESS DENIED");
			var op = await _service.RegisterOperationType(dto);

			return op;
		}

		// [HttpPost("{name}")]
		[HttpPatch("updateOperation/{name}")]
		public async Task<ActionResult<OperationTypeDTO>> UpdateByName(string name, OperationTypeDTO dto,
			[FromHeader] string token
		)
		{
			if (!AuthAdmin(token).Result)
				return BadRequest("ACCESS DENIED");
			var op = await _service.UpdateOperationType(new OperationTypeName(name), dto);

			return op;
		}

		//[HttpPost("deactivate/{name}")]
		[HttpDelete("deactivateOperation/{name}")]
		public async Task<ActionResult<OperationTypeDTO>> DeactivateByName(
			string name,
			[FromHeader] string token
		)
		{
			if (!AuthAdmin(token).Result)
				return BadRequest("ACCESS DENIED");
			var op = await _service.DeactivateByName(new OperationTypeName(name));

			return op;
		}

		[HttpGet("getSpecializations")]
		public async Task<ActionResult<IEnumerable<string>>> GetSpecializations(
			[FromHeader] string token
		)
		{
			if (!AuthAdmin(token).Result)
				return BadRequest("ACCESS DENIED");
			return await _service.GetSpecializations();
		}

		private async Task<bool> AuthAdmin(String token)
		{
			TokenDto tokenDto = await _tkService.GetByIdAsync(new TokenId(token));
			if (tokenDto.TokenValue != TokenType.ADMIN_AUTH_TOKEN.ToString())
				return false;
			else
				return true;
		}
	}
}