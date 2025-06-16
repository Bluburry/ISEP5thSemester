using System.Collections.Generic;
using System.Threading.Tasks;
using DDDSample1.Domain.OperationRooms;
using DDDSample1.Domain.Shared;

namespace DDDSample1.Domain.HospitalAppointment
{
    public interface IAppointmentRepository : IRepository<Appointment, AppointmentID>
    {
        IEnumerable<Appointment> GetAppointmentList();
        Task<Appointment> GetAppointmentByID(AppointmentID id);
        IEnumerable<Appointment> GetByRoomID(OperationRoomId id);

    }
}
