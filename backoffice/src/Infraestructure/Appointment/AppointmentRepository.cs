using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DDDSample1.Domain.HospitalAppointment;
using DDDSample1.Domain.HospitalPatient;
using DDDSample1.Domain.OperationRooms;
using DDDSample1.Infrastructure.Shared;
using Microsoft.EntityFrameworkCore;


namespace DDDSample1.Infrastructure.HospitalAppointment
{
    public class AppointmentRepository : BaseRepository<Appointment, AppointmentID>, IAppointmentRepository
    {
        private readonly HospitalDbContext _context;
        public AppointmentRepository(HospitalDbContext context) : base(context.Appointments)
        {
            _context = context;
        }
        public IEnumerable<Appointment> GetAppointmentList()
        {
            var appointment = _context.Appointments
                .Include(a => a.request)
                .Include(a => a.OperationRoom)
                .Include(a => a.designedStaff)
                .ThenInclude(s => s.staff)
                .Include(a => a.patient)
                .AsEnumerable(); 

            return appointment.ToList(); 
        }

        public async Task<Appointment> GetAppointmentByID(AppointmentID id)
        {
            IQueryable<Appointment> staff = _context.Appointments.Where(s => s.Id.Equals(id))
                .Include(s => s.designedStaff)
                .Include(s => s.OperationRoom)
                .Include(s => s.patient)
                .Include(s => s.request);
                
            return await staff.FirstOrDefaultAsync();
        }

        public IEnumerable<Appointment> GetByRoomID(OperationRoomId id){
             var appointment = _context.Appointments
                .Include(a => a.request)
                .ThenInclude(r => r.OperationType)
                .Include(a => a.OperationRoom)
                .Include(a => a.designedStaff)
                .ThenInclude(s => s.staff)
                .Include(a => a.patient)
                .AsEnumerable()
                .Where(a => a.OpRoomId.Equals(id));

            return appointment.ToList(); 
        }


    }
}