
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using DDDSample1.Domain.HospitalStaff;
using DDDSample1.Domain.Shared;


namespace DDDSample1.Domain.Specializations
{
	public class Specialization : Entity<SpecializationCode>, IAggregateRoot
	{
		public string SpecializationName { get; set; }
		public string SpecializationDescription { get; set; }

		public Specialization(string specializationName, string specializationDescription, string specializationCode)
		{
			if (string.IsNullOrEmpty(specializationName))
				throw new ArgumentException("Specialization must have a name");
			SpecializationName = specializationName;
			if (!string.IsNullOrEmpty(specializationDescription))
				SpecializationDescription = specializationDescription;
			this.Id = new SpecializationCode(specializationCode);
		}

		public Specialization(string specializationName, string specializationDescription)
		{
			if (string.IsNullOrEmpty(specializationName))
				throw new ArgumentException("Specialization must have a name");
			SpecializationName = specializationName;
			if (!string.IsNullOrEmpty(specializationDescription))
				SpecializationDescription = specializationDescription;
			this.Id = new SpecializationCode(Guid.NewGuid().ToString());
		}

		public Specialization(string specializationName)
		{
			if (string.IsNullOrEmpty(specializationName))
				throw new ArgumentException("Specialization must have a name");
			SpecializationName = specializationName;
			this.Id = new SpecializationCode(Guid.NewGuid().ToString());
		}

		public void ChangeName(string name)
		{
			if (string.IsNullOrEmpty(name))
				throw new ArgumentException("Specialization must have a name");
			SpecializationName = name;
		}

		public void ChangeDescription(string description)
		{
			if (string.IsNullOrEmpty(description))
				throw new ArgumentException("A description must be passed to Specialization");
			SpecializationDescription = description;
		}

		public override string ToString()
		{
			return $"Specialization Code: {Id.AsString()}, Name: {SpecializationName}, Description: {SpecializationDescription}";
			// return Id.ToString();
		}

		private Specialization() { }

		public SpecializationDTO ToDTO()
		{
			return new SpecializationDTO
			{
				SpecializationCode = this.Id.AsString(),
				SpecializationName = this.SpecializationName,
				SpecializationDescription = this.SpecializationDescription == null ? "" : this.SpecializationDescription
			};
		}
	}

}