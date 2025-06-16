
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DDDSample1.Domain.AvailabilitySlots;
using DDDSample1.Domain.HospitalStaff;
using DDDSample1.Domain.OperationRooms;
using DDDSample1.Infrastructure.Shared;
using Microsoft.EntityFrameworkCore;

namespace DDDSample1.Infrastructure.AvailabilitySlots
{
    public class AvailabilitySlotsRepository : BaseRepository<AvailabilitySlot, AvailabilitySlotsId>, IAvailabilitySlotsRepository
    {
    
        private readonly HospitalDbContext _context;

        public AvailabilitySlotsRepository(HospitalDbContext context) : base(context.availabilitySlots)
        {
            _context = context;
        }

        public async Task<AvailabilitySlot> GetAvailabilitySlots(LicenseNumber staff, string val)
        {
            var avalSlot = _context.availabilitySlots
                .Where(s => s.StaffId.Equals(staff))
                .Where(s => s.Value.Equals(val));

            return await avalSlot.FirstOrDefaultAsync(); 
        }

        public async Task<IEnumerable<AvailabilitySlot>> GetAllStaffSlots(LicenseNumber staff)
        {
            return await _context.availabilitySlots
                .Where(s => s.StaffId.Equals(staff))
                .ToListAsync();
        }
        public async Task<IEnumerable<AvailabilitySlot>> GetAllRoomsAsync()
        {
            return await _context.availabilitySlots
            .Where(slot => slot.roomId != null) // Filter slots that have a roomId
            .ToListAsync();
        }

        public IEnumerable<AvailabilitySlot> GetByRoomID(OperationRoomId roomId){
            return _context.availabilitySlots
            .Where(slot => slot.roomId.Equals(roomId)).AsEnumerable();
        }

    }
}