using System;
using System.Text.Json.Serialization;
using DDDSample1.Domain.HospitalStaff;
using DDDSample1.Domain.Shared;
using DDDSample1.Domain.Users;

namespace DDDSample1.Domain.Doctors
{
	public class Doctor : Entity<DoctorId>, IAggregateRoot
	{
		public Staff Staff { get; set; }

		//public LicenseNumber staffId {get; set;}

		public Doctor(Staff staff)
		{
			this.Id = new DoctorId(Guid.NewGuid());
			this.Staff = staff;
		}


		public Doctor() { }

		public DoctorDto ToDto()
		{
			return new DoctorDto(
				//this.staffId,
				this.Staff.Id,
				this.Id
			);
		}
	}


}