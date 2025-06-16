using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DDDSample1.AppServices.OperationRoomTypes;
using DDDSample1.Domain.OperationRoomTypes;
using DDDSample1.Domain.Tokens;
using DDDSample1.DTO;

namespace DDDSample1.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OperationRoomTypeController : ControllerBase
    {
        private readonly OperationRoomTypeService _operationRoomTypeService;
        private readonly TokenService _tokenSvc;

        public OperationRoomTypeController(OperationRoomTypeService operationRoomTypeService, TokenService tokenSvc)
        {
            _tokenSvc = tokenSvc;
            _operationRoomTypeService = operationRoomTypeService;
        }

        // POST: api/OperationRoomType
        [HttpPost]
        public async Task<ActionResult<OperationRoomTypeDto>> AddOperationRoomType([FromHeader] string token,[FromBody] OpRoomTypeDto dto)
        {
            TokenDto tokenDto = await _tokenSvc.GetByIdAsync(new TokenId(token));
			if (tokenDto.TokenValue != TokenType.ADMIN_AUTH_TOKEN.ToString())
				return BadRequest("ACCESS DENIED");
            try
            {
                var result = await _operationRoomTypeService.AddOperationRoomTypeAsync(dto);
                return CreatedAtAction(nameof(AddOperationRoomType), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }

    
}