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
	public class AppointmentController : ControllerBase
	{
		private readonly TokenService _tokenSvc;
		private readonly AppointmentService _appSvc;

		public AppointmentController(TokenService tokenSvc, AppointmentService appSvc)
		{
			_tokenSvc = tokenSvc;
			_appSvc = appSvc;
		}

		[HttpGet]
		public async Task<ActionResult<List<AppointmentDto>>> GetAppointments(
			[FromHeader] string auth
		)
		{
			TokenDto token = AuthStaff(auth).Result;
			if (token == null)
				return BadRequest("ACCESS DENIED");
            			
			var all = await _appSvc.GetAllAsync();
			List<AppointmentDto> dto = new List<AppointmentDto>();
			
			all.ForEach(appt => {
				dto.Add(appt.toDto());
			});

			return dto;
		}

		[HttpPut]
		public async Task<ActionResult<AppointmentDto>> CreateAppointment(
			[FromQuery] string dateAndTime,
			[FromQuery] string staffId,
			[FromQuery] string patientNumber,
			[FromQuery] string operationRoom,
			[FromQuery] string requestID,
			[FromHeader] string auth
		)
		{
			TokenDto token = AuthStaff(auth).Result;
			if (token == null)
				return BadRequest("ACCESS DENIED");
            			
			return await _appSvc.registerSurgeryAppointment(dateAndTime, staffId, patientNumber, operationRoom, requestID);
		}

		[HttpPatch]
		public async Task<ActionResult<AppointmentDto>> UpdateAppointment(
			[FromQuery] string ID,
			[FromQuery] string dateAndTime,
			[FromQuery] string staffIDs,
			[FromQuery] string operationRoom,
			[FromHeader] string auth
		)
		{
			TokenDto token = AuthStaff(auth).Result;
			if (token == null)
				return BadRequest("ACCESS DENIED");
			
			return await _appSvc.updateSurgeryAppointment(ID, dateAndTime, staffIDs, operationRoom);
		}

		[HttpGet("StaffSlot")]
		public async Task<ActionResult<IEnumerable<StaffDto>>> GetStaffByTimeSlot(
			[FromQuery] string request,
			[FromQuery] string dateAndTime,
			[FromHeader] string auth
		)
		{
			TokenDto token = AuthStaff(auth).Result;
			if (token == null)
				return BadRequest("ACCESS DENIED");
			
			return await _appSvc.GetStaffByTimeSlot(request, dateAndTime);
		}

		[HttpGet("AppointmentByRoom")]
		public async Task<ActionResult<AppointmentDto>> GetAppointmentByRoom(
			[FromQuery] string operationRoom,
			[FromHeader] string auth
		)
		{
			TokenDto token = AuthStaff(auth).Result;
			if (token == null)
				return BadRequest("ACCESS DENIED");
			
			var appt = await _appSvc.GetByRoomID(operationRoom);
			return appt;
		}

		private async Task<TokenDto> AuthStaff(String token)
		{
			TokenDto tokenDto = await _tokenSvc.GetByIdAsync(new TokenId(token));
			if (tokenDto.TokenValue != TokenType.STAFF_AUTH_TOKEN.ToString())
				return null;
			else
				return tokenDto;
		}
	}
}
