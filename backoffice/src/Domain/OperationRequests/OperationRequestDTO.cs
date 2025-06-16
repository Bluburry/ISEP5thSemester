
using Microsoft.AspNetCore.JsonPatch.Operations;
using Org.BouncyCastle.Asn1.Cms;

namespace DDDSample1.Domain.OperationRequests
{
	public class OperationRequestDTO
	{
		public string ID { get; set; }
		public string Doctor { get; set; }
		public string Patient { get; set; }
		public string OperationType { get; set; }
		public string OperationDeadline { get; set; }
		
		public string[] RequiredSpecialists {get;set;}
		public string OperationPriority { get; set; }

		public string EstimatedTime { get; set;}

		public OperationRequestDTO() { }

			// Constructor to easily create a StaffDto with all properties
		public OperationRequestDTO(string id, string doctor, string patient, string type, string deadline, string priority, string time)
		{
			ID = id;
			Doctor = doctor;
			Patient = patient;
			OperationType = type;
			OperationDeadline = deadline;
			OperationPriority = priority;
			EstimatedTime = time;
		}

		public OperationRequestDTO(string id, string doctor, string patient, string type, string deadline, string priority, string time, string[] requiredSpecialists)
		{
			ID = id;
			Doctor = doctor;
			Patient = patient;
			OperationType = type;
			OperationDeadline = deadline;
			OperationPriority = priority;
			EstimatedTime = time;
			this.RequiredSpecialists = requiredSpecialists;
		}

	}

}