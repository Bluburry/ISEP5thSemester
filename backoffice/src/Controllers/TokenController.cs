using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using DDDSample1.Domain.Shared;
using DDDSample1.Domain.Tokens;
using DDDSample1.Domain.Users;
using DDDSample1.Domain.HospitalStaff;

namespace DDDSample1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokensController : ControllerBase
    {
        private readonly TokenService _service;

        public TokensController(TokenService service)
        {
            _service = service;
        }

        // GET: api/Tokens/{tokenId}/user
        [HttpGet("GetTokenUserById")]
        public async Task<ActionResult<UserDto>> GetTokenUserById([FromHeader] string tokenId)
        {
            try
            {
                var user = await _service.GetTokenUserById(tokenId);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }

        // GET: api/Tokens
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TokenDto>>> GetAllTokens()
        {
            var tokens = await _service.GetAllTokensAsync();
            return Ok(tokens);
        }

        [HttpPost]
        public async Task<ActionResult<string>> ValidateRole(
            [FromHeader] string token
        )
        {
            try{
                TokenDto tokenDto = await _service.GetByIdAsync(new TokenId(token));
                var role = tokenDto.TokenValue switch{
                    "PATIENT_AUTH_TOKEN" => "PATIENT",
                    "STAFF_AUTH_TOKEN" => "STAFF",
                    "ADMIN_AUTH_TOKEN" => "ADMIN",
                    _ => "NO ROLE"
                };
                return Ok(new { role });
            }
            catch (Exception ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }
    }
}
