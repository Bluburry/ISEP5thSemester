using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DDDSample1.Domain.HospitalAppointment;
using DDDSample1.Domain.HospitalPatient;
using DDDSample1.Domain.OperationRooms;
using DDDSample1.Infrastructure.Shared;
using Microsoft.EntityFrameworkCore;


namespace DDDSample1.Infrastructure.OperationRooms
{
    public class OperationRoomRepository : BaseRepository<OperationRoom, OperationRoomId>, IOperationRoomRepository
    {
        private readonly HospitalDbContext _context;
        public OperationRoomRepository(HospitalDbContext context) : base(context.operationRooms)
        {
            this._context = context;
        }

        public async Task<OperationRoom> GetRoomById(OperationRoomId id)
        {
            var ret = _context.operationRooms
                .Include(room=>room.AvailabilitySlots)
                .Where(or => or.Id.Equals(id));

			return await ret.FirstOrDefaultAsync();
        }

        public async Task<List<OperationRoom>> GetAll()
        {
            var ret = _context.operationRooms
                .Include(room=>room.AvailabilitySlots);

			return await ret.ToListAsync();
        }
    }
}