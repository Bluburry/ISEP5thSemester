using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using DDDSample1.Domain.Shared;
using DDDSample1.Domain.Users;
using DDDSample1.Domain.Tokens;
using DDDSample1.DTO.PasswordActivationDto;
using DDDSample1.Domain.HospitalStaff;
using DDDSample1.AppServices;
using DDDSample1.DTO;
using DDDSample1.DTO.LoginAttemptTrackers;
using DDDSample1.Domain.HospitalPatient;

namespace DDDSample1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilteredPatientListController : ControllerBase
    {
        private readonly PasswordActivationService _passwordService;
        private readonly TokenService _tokenService;
        private readonly LoginService _logSvc;
        private readonly PatientService _patService;

        public FilteredPatientListController(PasswordActivationService userService, TokenService tokenService, PatientService patService, LoginService logSvc)
        {
            _passwordService = userService;
            _tokenService = tokenService;
            _patService = patService;
            _logSvc = logSvc;
        }


        // PUT: api/Users/U1
        [HttpGet("filteredPatients")]
        public async Task<ActionResult<List<PatientDto>>> GetFilteredUsers(
            [FromQuery] string name, 
            [FromQuery] string email, 
            [FromQuery] string phoneNumber, 
            [FromQuery] string medicalRecordNumber, 
            [FromQuery] string dateOfBirth, 
            [FromQuery] string gender, 
            [FromHeader] string token)
        {
            TokenDto tokenDto = await _tokenService.GetByIdAsync(new TokenId(token));

            if (tokenDto.TokenValue != TokenType.ADMIN_AUTH_TOKEN.ToString())
            {
                return BadRequest("ACCESS DENIED");
            }

            var queryData = new QueryDataDto
            {
                Name = name,
                Email = email,
                PhoneNumber = phoneNumber,
                MedicalRecordNumber = medicalRecordNumber,
                DateOfBirth = dateOfBirth,
                Gender = gender
            };

            return Ok((List<PatientDto>) _patService.GetFilteredPatients(queryData));
        }

        [HttpGet("medicalNumber")]
        public async Task<ActionResult<PatientDto>> GetById(string medicalNumber, string authToken){
            TokenDto tokenDto = await _tokenService.GetByIdAsync(new TokenId(authToken));

            if(tokenDto.TokenValue != TokenType.ADMIN_AUTH_TOKEN.ToString()){
                return BadRequest("ACCESS DENIED");
            }
            
            return Ok(await _patService.GetByIdAsync(medicalNumber));
        }

       
    }
}
