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

namespace DDDSample1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PasswordActivationController : ControllerBase
    {
        private readonly PasswordActivationService _passwordService;
        private readonly TokenService _tokenService;

        public PasswordActivationController(PasswordActivationService userService, TokenService tokenService)
        {
            _passwordService = userService;
            _tokenService = tokenService;
        }


        // PUT: api/Users/U1
        [HttpPost]
        public async Task<ActionResult<UserDto>> ActivatePassword([FromBody]string password,[FromHeader] string token)
        {            
            return await _passwordService.ActivatePassword(password, token);
        }

       
    }
}
