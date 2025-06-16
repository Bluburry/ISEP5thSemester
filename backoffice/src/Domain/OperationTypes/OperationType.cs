using DDDSample1.Domain.Shared;
using DDDSample1.Domain.RequiredSpecialists;
using DDDSample1.Domain.OperationPhases;
using DDDSample1.Domain.ValueObjects;
using System.Collections.Generic;
using System;

namespace DDDSample1.Domain.OperationTypes
{
	public class OperationType : Entity<OperationTypeID>, IAggregateRoot
	{
		public OperationTypeName OperationTypeName { get; set; }
		public List<OperationPhase> OperationPhases { get; set; }
		public List<RequiredSpecialist> RequiredSpecialists { get; set; }
		public EstimatedDuration EstimatedDuration { get; set; }
		public OperationTypeEndDate OperationTypeEndDate { get; set; }
		public OperationTypeStartDate OperationTypeStartDate { get; set; }
		public ActivationStatus ActivationStatus { get; set; }
		// public OperationTypeVersion OperationTypeVersion { get; set; }
		// [DatabaseGenerated(DatabaseGeneratedOption.Computed)] // might have to remove this
		public int VersionNumber { get; set; }

		public OperationType(OperationTypeName operationTypeName)
		{
			this.Id = new OperationTypeID(Guid.NewGuid());
			OperationTypeName = operationTypeName;
			OperationTypeStartDate = new OperationTypeStartDate(DateTime.Now);
			OperationTypeEndDate = null;
			VersionNumber = 1;
			ActivationStatus = ActivationStatus.ACTIVATED;
		}

		public OperationType(OperationTypeName operationTypeName, int version)
		{
			this.Id = new OperationTypeID(Guid.NewGuid());
			OperationTypeName = operationTypeName;
			OperationTypeStartDate = new OperationTypeStartDate(DateTime.Now);
			OperationTypeEndDate = null;
			VersionNumber = version;
			ActivationStatus = ActivationStatus.ACTIVATED;
		}

		public OperationType(OperationTypeName operationTypeName, List<OperationPhase> operationPhases, List<RequiredSpecialist> requiredSpecialists)
		{
			this.Id = new OperationTypeID(Guid.NewGuid());
			OperationTypeName = operationTypeName;
			ValidatePhases(operationPhases);
			ValidateSpecialists(requiredSpecialists);
			OperationPhases = operationPhases;
			RequiredSpecialists = requiredSpecialists;
			OperationTypeStartDate = new OperationTypeStartDate(DateTime.Now);
			OperationTypeEndDate = null;
			int estimatedDuration = 0;
			foreach (OperationPhase op in operationPhases)
			{
				estimatedDuration += op.PhaseDuration;
			}
			EstimatedDuration = new EstimatedDuration(estimatedDuration);
			VersionNumber = 1;
			ActivationStatus = ActivationStatus.ACTIVATED;
		}

		public OperationType()
		{
			// for ORM
		}

		public OperationType(OperationTypeName operationTypeName, List<OperationPhase> operationPhases,
			List<RequiredSpecialist> requiredSpecialists, EstimatedDuration estimatedDuration)
		{
			this.Id = new OperationTypeID(Guid.NewGuid());
			ValidatePhases(operationPhases);
			ValidateSpecialists(requiredSpecialists);
			OperationTypeName = operationTypeName;
			OperationPhases = operationPhases;
			RequiredSpecialists = requiredSpecialists;
			EstimatedDuration = estimatedDuration;
			OperationTypeStartDate = new OperationTypeStartDate(DateTime.Now);
			OperationTypeEndDate = null;
			VersionNumber = 1;
			ActivationStatus = ActivationStatus.ACTIVATED;
		}

		public void ChangeVersion(int version) { VersionNumber = version; }

		public void IncrementVersion() { VersionNumber++; }

		public void ChangeEndDateNow() { OperationTypeEndDate = new OperationTypeEndDate(DateTime.Now); }

		public void ChangeEndDate() { OperationTypeEndDate = new OperationTypeEndDate(DateTime.Now); }

		public void DeactivateOperationType() { ActivationStatus = ActivationStatus.DEACTIVATED; }

		public void ActivateOperationType() { ActivationStatus = ActivationStatus.ACTIVATED; }

		public void AddRequiredSpecialists(List<RequiredSpecialist> requiredSpecialists)
		{
			ValidateSpecialists(requiredSpecialists);
			RequiredSpecialists = requiredSpecialists;
		}

		public void AddOperationPhases(List<OperationPhase> operationPhases)
		{
			ValidatePhases(operationPhases);
			OperationPhases = operationPhases;
		}

		public void AddEstimatedDuration(EstimatedDuration estimatedDuration)
		{
			EstimatedDuration = estimatedDuration;
		}

		private void ValidateSpecialists(List<RequiredSpecialist> requiredSpecialists)
		{
			ArgumentNullException.ThrowIfNull(requiredSpecialists, "List of required specialists is null.");
			bool cleanPhase = false, surgPhase = false, prepPhase = false;
			foreach (RequiredSpecialist r in requiredSpecialists)
			{
				if (cleanPhase && surgPhase && prepPhase)
					break;
				switch (r.PhaseName)
				{
					case PhaseName.CLEANING:
						cleanPhase = true;
						break;
					case PhaseName.PREPARATION:
						prepPhase = true;
						break;
					case PhaseName.SURGERY:
						surgPhase = true;
						break;
					default:
						throw new ArgumentException("Found a specialist without a valid operation phase in operation type");
				}
			}
			if (!(cleanPhase && surgPhase && prepPhase))
				throw new ArgumentException("Not all operation phases were accounted for in specialists (operation type)");

		}

		private void ValidatePhases(List<OperationPhase> operationPhases)
		{
			ArgumentNullException.ThrowIfNull(operationPhases, "List of required specialists is null.");
			bool cleanPhase = false, surgPhase = false, prepPhase = false;
			foreach (OperationPhase r in operationPhases)
			{
				if (cleanPhase && surgPhase && prepPhase)
					break;
				switch (r.PhaseName)
				{
					case PhaseName.CLEANING:
						cleanPhase = true;
						break;
					case PhaseName.PREPARATION:
						prepPhase = true;
						break;
					case PhaseName.SURGERY:
						surgPhase = true;
						break;
					default:
						throw new ArgumentException("Found an invalid operation phase in operation type");
				}
			}
			if (!(cleanPhase && surgPhase && prepPhase))
				throw new ArgumentException("Not all operation phases were accounted for in operation phases (operation type)");
		}


		public OperationTypeDTO ToDTO()
		{
			return new OperationTypeDTO
			{
				ID = this.Id.AsString(),
				OperationName = this.OperationTypeName.ToString(),
				EstimatedDuration = this.EstimatedDuration.ToString(),
				OperationStartDate = this.OperationTypeStartDate.ToString(),
				OperationEndDate = this.OperationTypeEndDate == null ? "not set" : this.OperationTypeEndDate.ToString(),
				VersionNumber = this.VersionNumber.ToString(),
				ActivationStatus = this.ActivationStatus.ToString(),
				OperationPhases = this.OperationPhases.ConvertAll(op => op.ToString()),
				PhaseNames = this.OperationPhases.ConvertAll(op => op.PhaseName.ToString()),
				PhasesDuration = this.OperationPhases.ConvertAll(op => op.PhaseDuration.ToString()),
				RequiredSpecialists = this.RequiredSpecialists.ConvertAll(rs => rs.ToString()),
				SpecialistNames = this.RequiredSpecialists.ConvertAll(rs => rs.Specialization.SpecializationName),
				SpecialistsCount = this.RequiredSpecialists.ConvertAll(rs => rs.SpecialistCount.ToString()),
				SpecialistPhases = this.RequiredSpecialists.ConvertAll(rs => rs.PhaseName.ToString()),
			};
		}
	}
}
