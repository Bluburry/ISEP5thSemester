using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using DDDSample1.Domain.Users;
using DDDSample1.Domain.Tokens;
using DDDSample1.AppServices;
using DDDSample1.Domain.HospitalPatient;

namespace DDDSample1.Controllers
{
    [Route("api/Patient")]
    [ApiController]
    public class ConfirmUpdateController : ControllerBase
    {
        private readonly UpdateInformationService _updSvc;

        public ConfirmUpdateController(UpdateInformationService updateInformationService)
        {
            _updSvc = updateInformationService;         
        }


        // PUT: api/Users/U1
        [HttpPost]
        [Route("ConfirmUpdate")]
        public async Task<ActionResult<PatientDto>> ConfirmPatientInformation([FromHeader] string token)
        {            
            return await _updSvc.updatePatientInformation(token);
        }

       
    }
}
