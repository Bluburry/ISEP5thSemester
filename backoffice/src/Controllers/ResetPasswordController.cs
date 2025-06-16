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
using DDDSample1.Domain.ValueObjects;
using AppServices;

namespace DDDSample1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResetPasswordController : ControllerBase
    {
        private readonly PasswordActivationService _passwordService;
        private readonly TokenService _tokenService;
        private readonly UserService _usrService;



        public ResetPasswordController(PasswordActivationService passwordService, TokenService tokenService, UserService usrService)
        {
            _passwordService = passwordService;
            _tokenService = tokenService;
            _usrService = usrService;
        }


        // PUT: api/Users/U1
        [HttpPut("RequestPasswordChange")]
        public async Task<ActionResult<TokenDto>> GeneratePasswordResetToken([FromQuery] string email)
        {            
            TokenDto dto = await _tokenService.GeneratePasswordResetToken(email);

            EmailService.sendActivationEmail(email, dto);

            return Ok();
        }

        [HttpPost("ChangePassword")]
        public async Task<ActionResult<UserDto>> ResetPassword([FromBody] string password, [FromHeader] string token)
        {            
            TokenDto tokenDto = await _tokenService.GetByIdAsync(new TokenId(token));

            if(tokenDto.TokenValue != TokenType.PASSWORD_RESET_TOKEN.ToString()){
                return BadRequest("Invalid Token");
            }

            UserDto user = await _usrService.GetByIdAsync(new Username(tokenDto.UserId));

            if(user.ActivationStatus != ActivationStatus.ACTIVATED.ToString()){
                return BadRequest("User Not Activated");
            }


            UserDto userDto = await _passwordService.ResetPassword(password, user);
            _tokenService.RemoveToken(token);
            return Ok(userDto);
        }

       
    }
}
