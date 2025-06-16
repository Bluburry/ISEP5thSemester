using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using DDDSample1.Domain.Shared;
using DDDSample1.Domain.Users;
using DDDSample1.Domain.HospitalStaff;
using DDDSample1.Domain.Tokens;
using AppServices;
using DDDSample1.Domain.Doctors;


namespace DDDSample1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterBackofficeController : ControllerBase
    {
        private readonly UserService _usrSvc;
        private readonly TokenService _tokenSvc;

 
        public RegisterBackofficeController(UserService usrSvc,TokenService tokenSvc)
        {

            _tokenSvc = tokenSvc;
            _usrSvc = usrSvc;

        }

        [HttpGet]
        public async Task<UserDto> GetById(string email){
            return await _usrSvc.GetByIdAsync(new Username(email));
        }

        [HttpPost("RegisterBackoffice")]
        public async Task<ActionResult<UserDto>> RegisterBackoffice(string emailAddress,[FromHeader] string tokenNumber)
        {
            TokenDto authToken = await _tokenSvc.GetByIdAsync(new TokenId(tokenNumber));

            if(authToken.TokenValue != TokenType.ADMIN_AUTH_TOKEN.ToString()){
                return BadRequest("ACCESS TO ADMIN PERMISSIONS DENIED.");
            }

            if (emailAddress == null)
            {
                return BadRequest("Staff email data cannot be null.");
            }
            var user = await _usrSvc.RegisterBackofficeUserAsync(emailAddress);

            if (user == null)
            {
                return BadRequest("Something went wrong when registering the user.");
            }

            // Ensure the response includes the whole UserDto object


            TokenDto token = await _tokenSvc.GeneratePasswordValidationTokenAsync(user);

            EmailService.sendActivationEmail(user.EmailAddress, token);
            
            return CreatedAtAction(nameof(RegisterBackoffice), new { id = user.EmailAddress }, user); 

        }
    }
}