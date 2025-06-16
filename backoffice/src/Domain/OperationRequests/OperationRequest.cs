using System;
using System.Linq;
using DDDSample1.Domain.Doctors;
using DDDSample1.Domain.HospitalPatient;
using DDDSample1.Domain.HospitalStaff;
using DDDSample1.Domain.OperationTypes;
using DDDSample1.Domain.Shared;
using DDDSample1.Domain.ValueObjects;

namespace DDDSample1.Domain.OperationRequests
{
	public class OperationRequest : Entity<OperationRequestId>, IAggregateRoot
	{

		public Doctor Doctor { get; set; }
		public Patient Patient { get; set; }
		public OperationType OperationType { get; set; }
		public OperationPriority OperationPriority { get; set; }
		public DateTime OperationDeadline { get; set; }
		public OperationStatus OperationStatus { get; set; }


		public OperationRequest() { }

		public OperationRequest(Doctor doctor, Patient patient, OperationType type, OperationPriority priority, DateTime deadline)
		{
			ArgumentNullException.ThrowIfNull(doctor, "Staff can't be null.");
			ArgumentNullException.ThrowIfNull(patient, "Patient can't be null.");
			ArgumentNullException.ThrowIfNull(type, "Type can't be null.");
			ArgumentNullException.ThrowIfNull(priority, "Priority can't be null.");
			ArgumentNullException.ThrowIfNull(deadline, "Deadline can't be null.");
			this.Doctor = doctor;
			this.Patient = patient;
			this.OperationType = type;
			this.OperationPriority = priority;
			this.OperationDeadline = deadline;
			this.OperationStatus = OperationStatus.PENDING;
			this.Id = new OperationRequestId(Guid.NewGuid());
		}

		public OperationRequest(Doctor doctor, Patient patient, OperationType type, OperationPriority priority, DateTime deadline, OperationStatus operationStatus)
		{
			ArgumentNullException.ThrowIfNull(doctor, "Staff can't be null.");
			ArgumentNullException.ThrowIfNull(patient, "Patient can't be null.");
			ArgumentNullException.ThrowIfNull(type, "Type can't be null.");
			ArgumentNullException.ThrowIfNull(priority, "Priority can't be null.");
			ArgumentNullException.ThrowIfNull(deadline, "Deadline can't be null.");
			this.Doctor = doctor;
			this.Patient = patient;
			this.OperationType = type;
			this.OperationPriority = priority;
			this.OperationDeadline = deadline;
			this.OperationStatus = operationStatus;
			this.Id = new OperationRequestId(Guid.NewGuid());
		}

		public void ApproveRequest() { OperationStatus = OperationStatus.APPROVED; }

		public void RevokeRequest() { OperationStatus = OperationStatus.PENDING; }

		public OperationRequestDTO toDTO()
		{
			if(this.OperationType.RequiredSpecialists == null){
				return new OperationRequestDTO
			{
				ID = this.Id.AsString(),
				Doctor = this.Doctor.Id.AsString(),
				Patient = this.Patient.Id.AsString(),
				OperationType = this.OperationType.OperationTypeName.ToString(),
				OperationDeadline = this.OperationDeadline.ToString(),
				OperationPriority = this.OperationPriority.ToString(),
				EstimatedTime = this.OperationType.EstimatedDuration.Duration.ToString()
			};
			}else{
				return new OperationRequestDTO
			{
				ID = this.Id.AsString(),
				Doctor = this.Doctor.Id.AsString(),
				Patient = this.Patient.Id.AsString(),
				OperationType = this.OperationType.OperationTypeName.ToString(),
				OperationDeadline = this.OperationDeadline.ToString(),
				OperationPriority = this.OperationPriority.ToString(),
				EstimatedTime = this.OperationType.EstimatedDuration.Duration.ToString(),
				RequiredSpecialists = this.OperationType.RequiredSpecialists
            		.Select(rs => rs.ToString())
            		.ToArray(),
			};
		}
		}

		public override string ToString()
		{
			return $"Operation Request: {this.Id.AsString()} | Doctor: {this.Doctor.Id.AsString()} | Patient: {this.Patient.Id.AsString()} | Priority: {this.OperationPriority.ToString()} | Deadline: {this.OperationDeadline.ToString()} | Status: {this.OperationStatus.ToString()}";
		}
	}
}
