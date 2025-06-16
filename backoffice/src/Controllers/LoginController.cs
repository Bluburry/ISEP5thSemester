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
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Linq;
using Microsoft.AspNetCore.Authentication.Google;
using DDDSample1.Domain.Logs;

namespace DDDSample1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly TokenService _tokenService;
        private readonly LoginService _logSvc;
        private readonly UserService _usrService;

        public LoginController( TokenService tokenService, UserService usrService, LoginService logSvc)
        {
            _tokenService = tokenService;
            _usrService = usrService;
            _logSvc = logSvc;
        }


        // PUT: api/Users/U1
        [HttpPost("Login")]
        public async Task<ActionResult<LoginOutputDto>> Login(LoginCredentialsDto loginCredentialsDto)
        {            
            LoginOutputDto dto = await _logSvc.Login(loginCredentialsDto);

            if(dto.Result==LoginResult.DEACTIVATED.ToString()){
                UserDto userDto = await _usrService.DeactivateAsync(new Username(loginCredentialsDto.Username));
                return BadRequest("Deactivated, Loging Attempts Exceeded");
            }

            if(dto.Result==LoginResult.Failure.ToString()){
                return BadRequest("Login Credentials do not match.");
            }

            TokenDto tokenDto = await _tokenService.CreateAuthToken(loginCredentialsDto);


            dto.Token= tokenDto.TokenId;
            dto.Type = tokenDto.TokenValue;

            return Ok(dto); 

        }

        [HttpGet("LoginIAM")]
        public async Task<IActionResult> Login()
        {
            // Initiate Google login with redirection to LoginIAMResponse
            return Challenge(new AuthenticationProperties
            {
                RedirectUri = Url.Action("LoginIAMResponse")
            }, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet("LoginIAMResponse")]
        public async Task<IActionResult> LoginIAMResponse()
        {
            // Authenticate the user after returning from Google login
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (result?.Principal == null)
            {
                return Unauthorized(); // Authentication failed
            }

            var claimStrings = result.Principal.Identities.FirstOrDefault()?.Claims
                .Select(claim => $"Type:{claim.Type.Split('/').Last()}, Value:{claim.Value}")
                .ToArray();

            if (claimStrings == null)
            {
                return BadRequest("Failed to retrieve user claims.");
            }

            var loginCredentialsDto = _logSvc.TreatAuthenticateResult(claimStrings);
            LoginOutputDto dto;

            var redirectUrl="";
            try
            {
                dto = await _logSvc.Login(loginCredentialsDto);
            }
            catch (ArgumentException)
            {
                return BadRequest("User Does Not Exist");
            }
            catch(Exception){
                redirectUrl = $"http://localhost:4200/?error=user-not-found";
                return Redirect(redirectUrl);
            }

            var tokenDto = await _tokenService.CreateAuthToken(loginCredentialsDto);
            dto.Token = tokenDto.TokenId;

            // Redirect with token
            redirectUrl = $"http://localhost:4200/?token={dto.Token}";
            return Redirect(redirectUrl);
        }
       
    }
}
