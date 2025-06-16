using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using DDDSample1.Domain.Users;
using DDDSample1.Domain.Tokens;
using DDDSample1.Domain.OperationRequests;
using System.Collections.Generic;
using DDDSample1.Domain.HospitalAppointment;
using DDDSample1.DTO;
using DDDSample1.Domain.HospitalStaff;
using System.Collections;
using DDDSample1.Domain.HospitalPatient;
using DDDSample1.Domain.OperationRooms;
using DDDSample1.Domain.AvailabilitySlots;
using DDDSample1.Application.AvailabilitySlots;

namespace DDDSample1.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class OperationRequestController : ControllerBase
	{
		private readonly OperationRequestService _opReqSvc;
		private readonly UserService _usrSvc;
		private readonly TokenService _tokenSvc;
		private readonly StaffService _staffSvc;
		private readonly AppointmentService _appSvc;

		public OperationRequestController(OperationRequestService opReqSvc, UserService usrSvc, TokenService tokenSvc, AppointmentService appSvc, StaffService staffSvc)
		{
			_opReqSvc = opReqSvc;
			_usrSvc = usrSvc;
			_tokenSvc = tokenSvc;
			_appSvc = appSvc;
			_staffSvc = staffSvc;
		}

		[HttpPost]
		public async Task<ActionResult<OperationRequestDTO>> CreateOperationRequest(
			[FromQuery] string patient,
			[FromQuery] string type,
			[FromQuery] string deadline,
			[FromQuery] string priority,
			[FromHeader] string auth
		)
		{
			TokenDto token = AuthStaff(auth).Result;
			if (token == null)
				return BadRequest("ACCESS DENIED");


			return await _opReqSvc.CreateRequest(patient, type, deadline, priority, token);
		}

		[HttpPatch("{id}")]
		public async Task<ActionResult<OperationRequestDTO>> EditOperationRequest(string id,
			//[FromQuery] string operationName,
			[FromQuery] string deadline,
			[FromQuery] string priority,
			[FromHeader] string auth)
		{
			TokenDto token = AuthStaff(auth).Result;
			if (token == null)
				return BadRequest("ACCESS DENIED");

			// EditRequest(id, name, priority, deadline, token)
			return await _opReqSvc.EditRequest(id, /* operationName,  */priority, deadline, token);
		}

		[HttpDelete("{id}")]
		public async Task<ActionResult<OperationRequestDTO>> DeleteOperationRequest(string id,
			[FromHeader] string auth)
		{
			TokenDto token = AuthStaff(auth).Result;
			if (token == null)
				return BadRequest("ACCESS DENIED");

			return await _opReqSvc.DeleteOperationRequest(id, token);
		}
		
		[HttpGet]
		public async Task<ActionResult<List<OperationRequestDTO>>> ListOperationRequest(
			[FromQuery] string patient,	
			[FromQuery] string type,	
			[FromQuery] string priority,	
			[FromQuery] string status,	
			[FromHeader] string auth
		){
			TokenDto token = AuthStaff(auth).Result;
			if (token == null)
				return BadRequest("ACCESS DENIED");

			return await _opReqSvc.ListOperationRequest(patient, type, priority, status, token);
		}

		// [HttpPost]
		[HttpGet("Operation")]
		public async Task<ActionResult<OperationRequestDTO>> GetOperationById(
			[FromQuery] string id,
			[FromHeader] string auth
		){
			TokenDto token = AuthStaff(auth).Result;
			if (token == null)
				return BadRequest("ACCESS DENIED");

			return await _opReqSvc.GetByIdAsync(id);
		}

		[HttpGet("ListForAdmin")]
		public async Task<ActionResult<List<OperationRequestDTO>>> ListOperationRequestAdmin(
			[FromHeader] string auth
		){
			TokenDto tokenDto = await _tokenSvc.GetByIdAsync(new TokenId(auth));
			if (tokenDto.TokenValue != TokenType.ADMIN_AUTH_TOKEN.ToString())
				return BadRequest("ACCESS DENIED");

			return await _opReqSvc.OperationRequestsForAdmin();
		}

		[HttpGet("OperationRooms")]
		public async Task<ActionResult<List<OperationRoomDto>>> ListOperationRooms(
			[FromHeader] string auth
		){
			TokenDto tokenDto = await _tokenSvc.GetByIdAsync(new TokenId(auth));
			if (tokenDto.TokenValue == TokenType.ADMIN_AUTH_TOKEN.ToString() || tokenDto.TokenValue == TokenType.STAFF_AUTH_TOKEN.ToString())
				return await _appSvc.GetOperationRooms();
			else
				return BadRequest("ACCESS DENIED");
		}

		[HttpPost("Schedule")]
		public async Task<ActionResult<SchedulingResult>> ScheduleOperation(
			[FromBody] ScheduleAppointmentsDto dto,
			[FromHeader] string auth
		){
			
			List<StaffDto> staffList = new List<StaffDto>();
			foreach(string mrn in dto.mrnList){
				staffList.Add(await _staffSvc.GetByIdAsync(mrn));
			}

			List<OperationRequestDTO> opList = new List<OperationRequestDTO>();
			foreach(string opCode in dto.opCodes){
				opList.Add(await _opReqSvc.GetByIdAsync(opCode));
			}
			List<OperationRoom> opRooms = new List<OperationRoom>();

			foreach(string opRoom in dto.operationRooms){
				opRooms.Add(await _appSvc.GetOperationRoom(opRoom));
			}
			
			return await _appSvc.ScheduleAppointmentForRoomDay(staffList, opList, opRooms, dto.day);
		}

		private async Task<TokenDto> AuthStaff(String token)
		{
			TokenDto tokenDto = await _tokenSvc.GetByIdAsync(new TokenId(token));
			if (tokenDto.TokenValue != TokenType.STAFF_AUTH_TOKEN.ToString())
				return null;
			else
				return tokenDto;
		}

		// GET: api/Patient
        [HttpGet("AllRoomAvailabilitySlots")]
        public async Task<ActionResult<List<RoomAvailabilityDto>>> GetAllRoomAvailabilitySlots()
        {
            var slots = await _appSvc.GetAllRoomAsync();
            return Ok(slots);
        }

	}
}
