using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DDDSample1.Domain.AssignedStaffs;
using DDDSample1.Domain.HospitalAppointment;
using DDDSample1.Domain.HospitalStaff;
using DDDSample1.Infrastructure.Shared;
using Microsoft.EntityFrameworkCore;

namespace DDDSample1.Infrastructure.AssignedStaffs
{
	public class AssignedStaffRepository : BaseRepository<AssignedStaff, AssignedStaffID>, IAssignedStaffRepository
	{
		private readonly HospitalDbContext _context;
		public AssignedStaffRepository(HospitalDbContext context) : base(context.AssignedStaff) {
			_context = context;
		}

        public IEnumerable<AssignedStaff> GetAssignedStaffByAppointment(AppointmentID appointment)
        {
			var assignedStaff = _context.AssignedStaff
				.Include(s => s.appointment)
				.Include(s => s.staff)
				.AsEnumerable()
                .Where(s => s.appointment.Id.Equals(appointment.Value));

            return assignedStaff.ToList();
		}

		public async Task DeleteAssignedStaff(LicenseNumber staffId, AppointmentID appointmentId)
		{
			var assignedStaff = await _context.AssignedStaff
				.FirstOrDefaultAsync(a => a.staff.Id.Equals(staffId.Value) && a.appointment.Id.Equals(appointmentId.Value));

			if (assignedStaff != null){
				assignedStaff.appointment = null;
				assignedStaff.staff = null;
				_context.AssignedStaff.Remove(assignedStaff);
			}	
		}
	}
}
