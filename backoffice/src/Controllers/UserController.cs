using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using DDDSample1.Domain.Shared;
using DDDSample1.Domain.Users;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Linq;
using DDDSample1.Domain.HospitalStaff;
using DDDSample1.DTO;
using DDDSample1.Domain.HospitalPatient;
using Org.BouncyCastle.Asn1.Ess;
using AppServices;
using DDDSample1.Domain.Tokens;
using Microsoft.AspNetCore.Authentication.Google;

namespace DDDSample1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserService _service;
        private readonly LoginService _logSvc;
        private readonly PatientService _patSvc;
        private readonly TokenService _tokenSvc;

        public UsersController(UserService service, LoginService loginService, PatientService patientService, TokenService tokenService)
        {
            _service = service;
            _logSvc = loginService;
            _patSvc = patientService;
            _tokenSvc = tokenService;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAll()
        {
            return await _service.GetAllAsync();
        }

        // GET: api/Users/U1
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetById(string id)
        {
            var user = await _service.GetByIdAsync(new Username(id));

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }
        // POST: api/Users
        [HttpPost]
        public async Task<ActionResult<UserDto>> Create(UserDto dto)
        {
            var user = await _service.AddAsync(dto);

            // Ensure the response includes the whole UserDto object
            return CreatedAtAction(nameof(GetById), new { id = user.EmailAddress }, user); 
        }

        // PUT: api/Users/U1
        [HttpPut("{id}")]
        public async Task<ActionResult<UserDto>> Update(string id, UserDto dto)
        {
            if (id != dto.EmailAddress)
            {
                return BadRequest();
            }

            try
            {
                var user = await _service.UpdateAsync(dto);

                if (user == null)
                {
                    return NotFound();
                }

                return Ok(user);
            }
            catch (BusinessRuleValidationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        

        // DELETE: api/Users/U1/hard
        [HttpDelete("{id}/hard")]
        public async Task<ActionResult<UserDto>> HardDelete(string id)
        {
            try
            {
                var user = await _service.DeleteAsync(new Username(id));

                if (user == null)
                {
                    return NotFound();
                }

                return Ok(user);
            }
            catch (BusinessRuleValidationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPost("RegisterUserPatient")]
        public async Task<ActionResult<UserDto>> RegisterUserPatient([FromBody] LoginCredentialsDto dto)
        {            
            var existingPatientProfile = await _patSvc.checkIfPatientProfileExists(dto.Username);

            if (existingPatientProfile == null){
                return BadRequest("No Available Patient Profile shares the same e-mail as the one you are trying to login.");
            } else if (string.IsNullOrEmpty(existingPatientProfile.userId)){
                var user = await _service.AddWithPasswordAsync(dto);
                _patSvc.addUserToPatientProfile(existingPatientProfile.mrn, user.EmailAddress);
                
                TokenDto token = await _tokenSvc.GeneratePasswordValidationTokenAsync(user);
                
                EmailService.sendEmailVerificationEmail(dto.Username, token);
                return Ok(new { message = user.ToString()});
            } else {
                return BadRequest("The Patient Profile that shares the same email as the one you are trying to login, already has a user associated."+
                "\nTry logging in with your information.");
            }

        }

        [HttpGet("RegisterIAM")]
        public async Task<IActionResult> Login()
        {
            // Initiate Google login with redirection to LoginIAMResponse
            return Challenge(new AuthenticationProperties
            {
                RedirectUri = Url.Action("RegisterUserIAM")
            }, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet("RegisterUserIAM")]
        public async Task<ActionResult<string>> RegisterUserIAM()
        {            
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (result?.Principal == null) {return Unauthorized();}
            
            var claimStrings = result.Principal.Identities.FirstOrDefault().Claims
            .Select(claim => $"Type:{claim.Type.Split('/').Last()}, Value:{claim.Value}")
            .ToArray();

            LoginCredentialsDto loginCredentialsDto = _logSvc.TreatAuthenticateResult(claimStrings);
            
            var existingPatientProfile = await _patSvc.checkIfPatientProfileExists(loginCredentialsDto.Username);

            var redirectUrl="";
            if (existingPatientProfile == null){
                redirectUrl = $"http://localhost:4200/register-user?error=patient-profile-not-found";
                return Redirect(redirectUrl);
            } else if (string.IsNullOrEmpty(existingPatientProfile.userId)){
                var user = await _service.AddIAMAsync(loginCredentialsDto);
                _patSvc.addUserToPatientProfile(existingPatientProfile.mrn, user.EmailAddress);
                redirectUrl = $"http://localhost:4200/?action=successfull-registration";
                return Redirect(redirectUrl);
            } else {
                redirectUrl = $"http://localhost:4200/register-user?error=patient-profile-already-exists";
                return Redirect(redirectUrl);
            }

        }


        [HttpPost("ActivatePatientAccount")]
        public async Task<ActionResult<UserDto>> ActivatePatientAccount([FromHeader] string token)
        {            
            return await _service.ActivatePatientAccount(token);
        }
    }
}
