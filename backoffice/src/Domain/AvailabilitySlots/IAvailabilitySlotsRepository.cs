using System.Collections.Generic;
using System.Threading.Tasks;
using DDDSample1.Domain.HospitalStaff;
using DDDSample1.Domain.OperationRooms;
using DDDSample1.Domain.Shared;

namespace DDDSample1.Domain.AvailabilitySlots
{
    public interface IAvailabilitySlotsRepository : IRepository<AvailabilitySlot, AvailabilitySlotsId>
    {

        public Task<AvailabilitySlot> GetAvailabilitySlots(LicenseNumber staff, string val);

        public Task<IEnumerable<AvailabilitySlot>> GetAllStaffSlots(LicenseNumber staff);

        public Task<IEnumerable<AvailabilitySlot>> GetAllRoomsAsync();
        public IEnumerable<AvailabilitySlot> GetByRoomID(OperationRoomId roomId);


    }
}
