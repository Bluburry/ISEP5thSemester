using System;
using DDDSample1.Domain.OperationTypes;
using DDDSample1.Domain.Shared;
using DDDSample1.Domain.ValueObjects;

namespace DDDSample1.Domain.OperationPhases
{
	public class OperationPhase : Entity<OperationPhaseID>,  IAggregateRoot
	{
		public PhaseName PhaseName { get; set; }
		// public PhaseDuration PhaseDuration { get; set; }
		public int PhaseDuration { get; set; }
		public OperationType OperationType { get; set; }
		public OperationPhase() { }
		public OperationPhase(PhaseName phaseName, int phaseDuration)
		{
			ArgumentNullException.ThrowIfNull(phaseName, "Phase name can't be null.");
			ArgumentNullException.ThrowIfNull(phaseDuration, "Phase duration can't be null.");
			if (phaseDuration < 5)
				throw new ArgumentException("Phase duration can't be less than 5 minutes.", nameof(phaseDuration));
			PhaseName = phaseName;
			PhaseDuration = phaseDuration;
			this.Id = new OperationPhaseID(Guid.NewGuid());
		}

		public OperationPhase(PhaseName phaseName, string phaseDuration)
		{
			ArgumentNullException.ThrowIfNull(phaseName, "Phase name can't be null.");
			ArgumentNullException.ThrowIfNull(phaseDuration, "Phase duration can't be null.");
			if (!int.TryParse(phaseDuration, out int d))
				throw new ArgumentException("Couldn't parse phase duration.", nameof(phaseDuration));
			if (d < 5)
				throw new ArgumentException("Phase duration can't be less than 5 minutes.", nameof(phaseDuration));
			PhaseName = phaseName;
			PhaseDuration = int.Parse(phaseDuration);
			this.Id = new OperationPhaseID(Guid.NewGuid());
		}

		public OperationPhase(PhaseName phaseName, int phaseDuration, OperationType operationType)
		{
			ArgumentNullException.ThrowIfNull(phaseName, "Phase name can't be null.");
			ArgumentNullException.ThrowIfNull(phaseDuration, "Phase duration can't be null.");
			ArgumentNullException.ThrowIfNull(operationType, "Operation type can't be null.");
			if (phaseDuration < 5)
				throw new ArgumentException("Phase duration can't be less than 5 minutes.", nameof(phaseDuration));
			PhaseName = phaseName;
			PhaseDuration = phaseDuration;
			OperationType = operationType;
			this.Id = new OperationPhaseID(Guid.NewGuid());
		}

		public OperationPhase(PhaseName phaseName, string phaseDuration, OperationType operationType)
		{
			ArgumentNullException.ThrowIfNull(phaseName, "Phase name can't be null.");
			ArgumentNullException.ThrowIfNull(phaseDuration, "Phase duration can't be null.");
			ArgumentNullException.ThrowIfNull(operationType, "Operation type can't be null.");
			if (!int.TryParse(phaseDuration, out int d))
				throw new ArgumentException("Couldn't parse phase duration.", nameof(phaseDuration));
			if (d < 5)
				throw new ArgumentException("Phase duration can't be less than 5 minutes.", nameof(phaseDuration));
			PhaseName = phaseName;
			PhaseDuration = d;
			OperationType = operationType;
			this.Id = new OperationPhaseID(Guid.NewGuid());
		}


		public override string ToString()
		{
			return $"Operation phase: {PhaseName.ToString()}, duration: {PhaseDuration.ToString()}";
		}
	}
}