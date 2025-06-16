using System;
using DDDSample1.Domain.Shared;
using DDDSample1.Domain.ValueObjects;
using DDDSample1.Domain.Specializations;
using DDDSample1.Domain.OperationTypes;

namespace DDDSample1.Domain.RequiredSpecialists
{
	public class RequiredSpecialist : Entity<RequiredSpecialistID>, IAggregateRoot
	{
		public Specialization Specialization { get; set; }
		// public SpecializationName SpecializationName { get; set; }
		public SpecialistCount SpecialistCount { get; set; }
		public OperationType OperationType { get; set; }
		public PhaseName PhaseName { get; set; }
		public RequiredSpecialist() { }

		public RequiredSpecialist(Specialization specialization, SpecialistCount specialistCount, PhaseName phase)
		{
			ArgumentNullException.ThrowIfNull(specialization, "Specialization can't be null.");
			ArgumentNullException.ThrowIfNull(specialistCount, "Specialization count can't be null.");
			Specialization = specialization;
			SpecialistCount = specialistCount;
			PhaseName = phase;
			this.Id = new RequiredSpecialistID(Guid.NewGuid());
		}

		public RequiredSpecialist(Specialization specialization, SpecialistCount specialistCount, OperationType operationType, PhaseName phase)
		{
			ArgumentNullException.ThrowIfNull(specialization, "Specialization can't be null.");
			ArgumentNullException.ThrowIfNull(specialistCount, "Specialization count can't be null.");
			ArgumentNullException.ThrowIfNull(operationType, "Operation type can't be null.");
			Specialization = specialization;
			SpecialistCount = specialistCount;
			OperationType = operationType;
			PhaseName = phase;
			this.Id = new RequiredSpecialistID(Guid.NewGuid());
		}

		public override string ToString()
		{
			return $"Specialization: {/* Specialization.ToString() */Specialization.SpecializationName}, count: {SpecialistCount.ToString()}, phase: {PhaseName.ToString()}.";
		}
	}
}