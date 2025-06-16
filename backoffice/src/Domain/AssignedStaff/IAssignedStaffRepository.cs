using System.Collections.Generic;
using System.Threading.Tasks;
using DDDSample1.Domain.HospitalAppointment;
using DDDSample1.Domain.HospitalStaff;
using DDDSample1.Domain.Shared;

namespace DDDSample1.Domain.AssignedStaffs
{
	public interface IAssignedStaffRepository : IRepository<AssignedStaff, AssignedStaffID>
	{
		IEnumerable<AssignedStaff>GetAssignedStaffByAppointment(AppointmentID id);

		Task DeleteAssignedStaff(LicenseNumber staffId, AppointmentID appointmentId);
	}
}