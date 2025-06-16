using System.Collections.Generic;
using System.Threading.Tasks;
using DDDSample1.Domain.Doctors;
using DDDSample1.Domain.HospitalStaff;
using DDDSample1.Domain.Shared;
using DDDSample1.DTO;

namespace DDDSample1.Domain.OperationRequests
{
    public interface IOperationRequestRepository : IRepository<OperationRequest, OperationRequestId>
    {
      Task<OperationRequest> GetRequestById(OperationRequestId id);
      Task<List<OperationRequest>> GetRequestByDoctor(DoctorId id, QueryDataDto queryData);
      Task<List<OperationRequest>> GetRequestsForAdmin();
    }

}
