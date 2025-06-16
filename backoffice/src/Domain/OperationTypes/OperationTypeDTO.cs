using System.Collections.Generic;

namespace DDDSample1.Domain.OperationTypes
{
	public class OperationTypeDTO
	{
		public string ID { get; set; }
		public string OperationName { get; set; }
		public string EstimatedDuration { get; set; }
		public string OperationStartDate { get; set; }
		public string OperationEndDate { get; set; }
		public string VersionNumber { get; set; }
		public string ActivationStatus { get; set; }
		public List<string> OperationPhases { get; set; }
		public List<string> PhaseNames { get; set; }
		public List<string> PhasesDuration { get; set; }
		public List<string> RequiredSpecialists { get; set; }
		public List<string> SpecialistNames { get; set; }
		public List<string> SpecialistsCount { get; set; }
		public List<string> SpecialistPhases { get; set; }

		public OperationTypeDTO() { }

		public OperationTypeDTO(string id, string operationName, string estimatedDuration, string operationStartDate,
			string operationEndDate, List<string> operationPhases, List<string> requiredSpecialists, List<string> specialistPhases)
		{
			ID = id;
			OperationName = operationName;
			EstimatedDuration = estimatedDuration;
			OperationStartDate = operationStartDate;
			OperationEndDate = operationEndDate;
			OperationPhases = operationPhases;
			RequiredSpecialists = requiredSpecialists;
			SpecialistPhases = specialistPhases;
		}

		public OperationTypeDTO(string id, string operationName, string estimatedDuration, string operationStartDate,
			string operationEndDate, string versionNumber, List<string> operationPhases, List<string> requiredSpecialists, List<string> specialistPhases)
		{
			ID = id;
			OperationName = operationName;
			EstimatedDuration = estimatedDuration;
			OperationStartDate = operationStartDate;
			OperationEndDate = operationEndDate;
			VersionNumber = versionNumber;
			OperationPhases = operationPhases;
			RequiredSpecialists = requiredSpecialists;
			SpecialistPhases = specialistPhases;
		}

		public OperationTypeDTO(string id, string operationName, string estimatedDuration, string operationStartDate, string operationEndDate,
			List<string> phaseNames, List<string> phasesDuration, List<string> specialistNames, List<string> specialistsCount, List<string> specialistPhases)
		{
			ID = id;
			OperationName = operationName;
			EstimatedDuration = estimatedDuration;
			OperationStartDate = operationStartDate;
			OperationEndDate = operationEndDate;
			PhaseNames = phaseNames;
			PhasesDuration = phasesDuration;
			SpecialistNames = specialistNames;
			SpecialistsCount = specialistsCount;
			SpecialistPhases = specialistPhases;
		}

		public OperationTypeDTO(string id, string operationName, string estimatedDuration, string operationStartDate, string operationEndDate, string versionNumber,
			List<string> phaseNames, List<string> phasesDuration, List<string> specialistNames, List<string> specialistsCount, List<string> specialistPhases)
		{
			ID = id;
			OperationName = operationName;
			EstimatedDuration = estimatedDuration;
			OperationStartDate = operationStartDate;
			OperationEndDate = operationEndDate;
			VersionNumber = versionNumber;
			PhaseNames = phaseNames;
			PhasesDuration = phasesDuration;
			SpecialistNames = specialistNames;
			SpecialistsCount = specialistsCount;
			SpecialistPhases = specialistPhases;
		}
	}
}