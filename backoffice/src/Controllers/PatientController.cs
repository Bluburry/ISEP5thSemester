using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using DDDSample1.Domain.HospitalPatient;
using DDDSample1.DTO;
using DDDSample1.Domain.Shared;
using DDDSample1.Domain.Tokens;
using DDDSample1.Domain.Logs;
using AppServices;

namespace DDDSample1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientController : ControllerBase
    {
        private readonly PatientService _patSvc;
        private readonly TokenService _tokenSvc;
        private readonly LogService _logSvc;

        public PatientController(PatientService patService, TokenService tokenSvc, LogService logSvc)
        {
            _patSvc = patService;
            _tokenSvc = tokenSvc;
            _logSvc = logSvc;
        }

        // GET: api/Patient
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PatientDto>>> GetAllPatients()
        {
            var patients = await _patSvc.GetAllAsync();
            var patientDtos = patients.ConvertAll(p => p.toDto()); // Assuming there is a toDto() method in the Patient entity.
            return Ok(patientDtos);
        }

        // GET: api/Patient/{id}
        [HttpGet("GetPatientById")]
        public async Task<ActionResult<PatientDto>> GetPatientById([FromHeader] string id)
        {
            var patient = await _patSvc.GetByIdAsync(id);
            if (patient == null)
            {
                return NotFound();
            }
            return Ok(patient);
        }

        // GET: api/Patient/
        [HttpGet("GetPatientByToken")]
        public async Task<ActionResult<PatientDto>> GetPatientByToken([FromHeader] string token)
        {
            var tokenDto = await _tokenSvc.GetByIdAsync(new TokenId(token));

            var patient = await _patSvc.GetByUserIdAsync(tokenDto.UserId);

            return Ok(patient);
        }

        [HttpPost("CreatePatient")]
        public async Task<ActionResult<PatientDto>> CreatePatientProfile([FromHeader] string token, PatientRegistrationDto dto)
        {
            var tokenDto = await _tokenSvc.GetByIdAsync(new TokenId(token));

            if(tokenDto.TokenValue != TokenType.ADMIN_AUTH_TOKEN.ToString()){
                return BadRequest("ACCESS TO RESOURCE DENIED.");
            }
            
            var patientDto = await _patSvc.registerPatient(dto);

            return Ok(patientDto); 
        }

        // DELETE: api/Patient/{id}
        [HttpDelete("DeletePatient")]
        public async Task<IActionResult> DeletePatientProfile([FromBody] string mrn, [FromHeader] string token)
        {
            var tokenDto = await _tokenSvc.GetByIdAsync(new TokenId(token));

            if(tokenDto.TokenValue != TokenType.ADMIN_AUTH_TOKEN.ToString()){
                return BadRequest("ACCESS TO RESOURCE DENIED.");
            }
            PatientDto patient = await _patSvc.GetByIdAsync(mrn);
            

            await _logSvc.LogPatientDeletion(patient);

            return Ok();
        }

        [HttpDelete("DeleteSelfPatient")]
        public async Task<ActionResult<object>> DeleteSelfPatientProfile([FromHeader] string token)
        {
            var tokenDto = await _tokenSvc.GetByIdAsync(new TokenId(token));

            if(tokenDto.TokenValue != TokenType.PATIENT_AUTH_TOKEN.ToString()){
                return BadRequest("ACCESS TO RESOURCE DENIED.");
            }

            TokenDto confirmationToken = await this._tokenSvc.GenerateDeletionConfirmationToken(tokenDto.UserId);
            EmailService.sendDeletionConfirmation(tokenDto.UserId, confirmationToken);

            return Ok(new { message ="Patient Deletion requires Email Confirmation. Check your inbox to confirm the process."});
        }

        [HttpDelete("ConfirmPatientDeletion")]
        public async Task<ActionResult<object>> ConfirmPatientDeletion([FromHeader] string token)
        {
            var tokenDto = await _tokenSvc.GetByIdAsync(new TokenId(token));

            if(tokenDto.TokenValue != TokenType.DELETION_TOKEN.ToString()){
                return BadRequest("ACCESS TO RESOURCE DENIED.");
            }
            PatientDto patient = await _patSvc.GetByUserIdAsync(tokenDto.UserId);

            await _logSvc.LogPatientDeletion(patient);

            return Ok(new { message ="Confirmation Accepted. Patient Deletion is schedule to happen within the GRPD Parameters."});
        }

        // GET: api/Patient/filter
        [HttpPost("filter")]
        public ActionResult<IEnumerable<PatientDto>> GetFilteredPatients([FromBody] QueryDataDto queryData)
        {
            var filteredPatients = _patSvc.GetFilteredPatients(queryData);
            return Ok(filteredPatients);
        }

        [HttpPost("editPatient_Admin")]
        public async Task<ActionResult<PatientDto>> EditPatientProfileAdmin([FromBody] EditPatientDto_Admin editData, [FromHeader] string token)
        {
            var tokenDto = await _tokenSvc.GetByIdAsync(new TokenId(token));

            if(tokenDto.TokenValue != TokenType.ADMIN_AUTH_TOKEN.ToString()){
                return BadRequest("ACCESS TO RESOURCE DENIED.");
            }

            string email = (await _patSvc.GetByIdAsync(editData.patientId)).email;

            PatientDto patientDto = await _patSvc.EditPatientProfileAdmin(editData);

            

            if(editData.FirstName != null || editData.Fullname != null || editData.LastName != null || editData.Phone != null){
                EmailService.SendEmail(tokenDto.UserId, "Some sensitive information of yours in our platform was altered by an administrator. Please do check your profile and contact us if necessary.");
            }
            

            await _logSvc.LogPatientEditing(patientDto);

            return Ok(patientDto);
        }

        [HttpPost("editPatient_Patient")]
        public virtual async Task<ActionResult<object>> EditPatientProfilePatient([FromBody] EditPatientDto_Patient editData, [FromHeader] string token)
        {
            TokenDto tokenDto = await _tokenSvc.GetByIdAsync(new TokenId(token));
            PatientDto patientDto = await _patSvc.GetByUserIdAsync(tokenDto.UserId);

            if(tokenDto.TokenValue != TokenType.PATIENT_AUTH_TOKEN.ToString()){
                return BadRequest("ACCESS TO RESOURCE DENIED.");
            }
            PatientDto patientDtoUpdated = await _patSvc.EditPatientProfilePatient(editData, tokenDto);

            if (patientDto.email != patientDtoUpdated.email){
                await _logSvc.LogPatientEditingAttempt(patientDtoUpdated, tokenDto.UserId);
                return Ok(new { message = "Information Stored and Email Sent. Awaiting Update Confirmation." });
            }

            await _logSvc.LogPatientEditing(patientDtoUpdated);
            return Ok(patientDtoUpdated);
        }
    }
}
