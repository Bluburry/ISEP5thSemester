using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using DDDSample1.Domain.Shared;
using DDDSample1.Domain.Users;
using DDDSample1.Domain.HospitalStaff;
using DDDSample1.Domain.Tokens;
using AppServices;
using DDDSample1.Domain.Logs;
using DDDSample1.DTO;
using DDDSample1.Domain.ValueObjects;

namespace DDDSample1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StaffController : ControllerBase
    {
        private readonly StaffService _staffSvc;
        private readonly UserService _usrSvc;
		private readonly TokenService _tokenSvc;

        public StaffController(StaffService staffSvc, UserService usrSvc, TokenService tokenSvc)
        {
            _staffSvc = staffSvc;
			_usrSvc = usrSvc;
			_tokenSvc = tokenSvc;
        }

        // GET: api/Staff/List
        [HttpGet("List")]
        public async Task<ActionResult<IEnumerable<StaffDto>>> ListAll(
			[FromHeader] string auth
		)
        {
			if(!AuthAdmin(auth).Result)
                return BadRequest("ACCESS DENIED");

            return await _staffSvc.GetStaffList();
        }

        // GET: api/Staff/id
        [HttpGet("GetStaffById")]
        public async Task<ActionResult<IEnumerable<StaffDto>>> GetStaffById(
			[FromHeader] string id
		)
        {
            var staff = await _staffSvc.GetByIdAsync(id);
            if (staff == null)
                return NotFound();
            
            return Ok(staff);
        }

        // PUT: api/Staff/Create
        [HttpPut("Create")]
        public async Task<ActionResult<StaffDto>> CreateStaff(
			StaffDto staff,
            [FromHeader] string auth
		){

            if(!AuthAdmin(auth).Result)
                return BadRequest("ACCESS DENIED");

            //first name
            //last name
            //contact information
            //specialization
            return await _staffSvc.RegisterStaff(staff);
        }

        // PATCH: api/Staff/Update
        [HttpPatch("Update")]
        public async Task<ActionResult<UserDto>> UpdateStaff(
			[FromBody] StaffDto staff,
			[FromHeader] string auth
		){

            if(!AuthAdmin(auth).Result)
                return BadRequest("ACCESS DENIED");

            var oldData = await _staffSvc.GetByIdAsync(staff.LicenseNumber);
            Log log = await _staffSvc.UpdateAsync(staff);

			if(!oldData.Phone.Equals(staff.Phone) || !oldData.Email.Equals(staff.Email)){
			    var userDto = await _usrSvc.GetByIdAsync(new Username(oldData.Email));
                TokenDto token = await _tokenSvc.GeneratePasswordValidationTokenAsync(userDto);
                EmailService.sendContactConfirmation(oldData.Email.ToString(), token, log.Id.ToString());
			}

            // Ensure the response includes the whole UserDto object
            return CreatedAtAction(nameof(UpdateStaff), new { id = staff.LicenseNumber }, staff); 
        }

        // PUT: api/Staff/Activate
        [HttpPut("Activate")]
        public async Task<ActionResult<StaffDto>> ActivateAccountWithPassword(
			[FromQuery] string tokenId,
            [FromQuery] string password,
			[FromQuery] string logID
		){            
            var token = await _tokenSvc.GetByIdAsync(new TokenId(tokenId));
            var staff = await _staffSvc.GetLatestVersion(token.UserId);

            if(staff.TheUser.Password.Equals(new Password(password)))
                return await _staffSvc.UpdateVersion(staff.toDto(), logID, token.TokenId);
            else
                throw new ArgumentException("Password does not match.");

        }

        // PUT: api/Staff/Disable
        [HttpDelete("Disable")]
        public async Task<ActionResult<StaffDto>> DisableStaff(
            [FromQuery] string license,
            [FromHeader] string auth
        )
        {            
            if(!AuthAdmin(auth).Result)
                return BadRequest("ACCESS DENIED");

            return await _staffSvc.DisableStaff(license);
        }


        // POST: api/Staff/Filter
        [HttpPost("Filter")]
        public async Task<ActionResult<StaffDto>> FilterStaff(
            [FromQuery] string license,
            [FromQuery] string name, 
            [FromQuery] string email, 
            [FromQuery] string specialization,
            [FromQuery] string status,
            [FromHeader] string auth
        )
        {   

            if(!AuthAdmin(auth).Result)
                return BadRequest("ACCESS DENIED");

            return Ok((List<StaffDto>) _staffSvc.GetFilteredStaff(license, name, email, specialization, status));
        }

        private async Task<bool> AuthAdmin(String token){
            TokenDto tokenDto = await _tokenSvc.GetByIdAsync(new TokenId(token));
            if (tokenDto.TokenValue != TokenType.ADMIN_AUTH_TOKEN.ToString())
                return false;
            else
                return true;
        }
    }
}
