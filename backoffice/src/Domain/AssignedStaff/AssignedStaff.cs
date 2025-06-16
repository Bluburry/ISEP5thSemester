using System;
using DDDSample1.Domain.Shared;

using DDDSample1.Domain.HospitalStaff;
using DDDSample1.Domain.HospitalAppointment;

namespace DDDSample1.Domain.AssignedStaffs
{
	public class AssignedStaff : Entity<AssignedStaffID>, IAggregateRoot
	{
		public Staff staff { get; set; }
		public Appointment appointment { get; set; }

		public AssignedStaff() { }

		public AssignedStaff(Staff staff, Appointment appointment)
		{
			ArgumentNullException.ThrowIfNull(staff, "Staff can't be null.");
			ArgumentNullException.ThrowIfNull(appointment, "Appointment can't be null.");
			this.staff = staff;
			this.appointment = appointment;
			this.Id = new AssignedStaffID(Guid.NewGuid());
		}

		public override string ToString()
		{
			return $"Staff: {staff.FullName}, Appointment: {appointment.Id}.";
		}
	}
}