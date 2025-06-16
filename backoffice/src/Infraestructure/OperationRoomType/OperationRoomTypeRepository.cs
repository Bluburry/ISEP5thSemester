using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DDDSample1.Domain.HospitalAppointment;
using DDDSample1.Domain.HospitalPatient;
using DDDSample1.Domain.OperationRooms;
using DDDSample1.Domain.OperationRoomTypes;
using DDDSample1.Infrastructure.Shared;
using Microsoft.EntityFrameworkCore;


namespace DDDSample1.Infrastructure.OperationRoomTypes
{
    public class OperationRoomTypeRepository : BaseRepository<OperationRoomType, OperationRoomTypeId>, IOperationRoomTypeRepository
    {
        private readonly HospitalDbContext _context;
        public OperationRoomTypeRepository(HospitalDbContext context) : base(context.operationRoomTypes)
        {
            this._context = context;
        }

    }
}