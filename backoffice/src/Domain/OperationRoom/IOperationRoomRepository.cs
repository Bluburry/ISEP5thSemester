using System.Collections.Generic;
using System.Threading.Tasks;
using DDDSample1.Domain.Shared;

namespace DDDSample1.Domain.OperationRooms
{
    public interface IOperationRoomRepository : IRepository<OperationRoom, OperationRoomId>
    {
        public Task<OperationRoom> GetRoomById(OperationRoomId id);
        public Task<List<OperationRoom>> GetAll();
    }
}
