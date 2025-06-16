using System;
using DDDSample1.Domain.Doctors;
using DDDSample1.Domain.HospitalPatient;
using DDDSample1.Domain.OperationTypes;
using DDDSample1.Domain.ValueObjects;

namespace DDDSample1.Domain.OperationRequests
{
	public class OperationRequestBuilder
	{
		private Doctor _doctor;
		private Patient _patient;
		private OperationType _operationType;
		private OperationPriority? _operationPriority;
		private DateTime? _operationDeadline;
		private OperationStatus _operationStatus;

		public OperationRequestBuilder WithDoctor(Doctor doctor)
		{
			_doctor = doctor;
			return this;
		}

		public OperationRequestBuilder WithPatient(Patient patient)
		{
			_patient = patient;
			return this;
		}

		public OperationRequestBuilder WithType(OperationType type)
		{
			_operationType = type;
			return this;
		}

		public OperationRequestBuilder WithPriority(string priority)
		{
			if (!Enum.TryParse<OperationPriority>(priority, true, out _))
				return this;

			Enum.TryParse<OperationPriority>(priority, true, out OperationPriority op);
			_operationPriority = op;

			return this;
		}
		public OperationRequestBuilder WithDeadline(DateTime deadline)
		{
			_operationDeadline = deadline;
			return this;
		}

		public OperationRequestBuilder WithStatus(string status)
		{

			if (!Enum.TryParse<OperationStatus>(status, true, out _))
				return this;

			Enum.TryParse<OperationStatus>(status, true, out OperationStatus op);

			_operationStatus = op;
			return this;
		}

		public OperationRequest Build()
		{
			if (_doctor == null)
				throw new ArgumentException("Doctor is required.");
			if (_patient == null)
				throw new ArgumentException("Patient is required.");
			if (_operationType == null)
				throw new ArgumentException("Operation type is required.");
			if (_operationPriority == null)
                throw new ArgumentException("Operation priority is required.");
            if (_operationDeadline == null)
                throw new ArgumentException("Operation deadline is required.");

			var request = new OperationRequest(_doctor, _patient, _operationType, (OperationPriority)_operationPriority, (DateTime)_operationDeadline);

			return request;
		}
	}

}
