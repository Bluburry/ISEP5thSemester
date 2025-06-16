using System.Collections.Generic;
using System.Threading.Tasks;
using DDDSample1.Domain.Doctors;
using DDDSample1.Domain.HospitalStaff;
using DDDSample1.Domain.Shared;
using DDDSample1.DTO;

namespace DDDSample1.Domain.OperationRoomTypes
{
    public interface IOperationRoomTypeRepository : IRepository<OperationRoomType, OperationRoomTypeId>
    {

    }

}
