using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DDDSample1.Domain.Doctors;
using DDDSample1.Domain.HospitalPatient;
using DDDSample1.Domain.HospitalStaff;
using DDDSample1.Domain.Logs;
using DDDSample1.Domain.OperationTypes;
using DDDSample1.Domain.RequiredSpecialists;
using DDDSample1.Domain.Shared;
using DDDSample1.Domain.Specializations;
using DDDSample1.Domain.Tokens;
using DDDSample1.Domain.Users;
using DDDSample1.Domain.ValueObjects;
using DDDSample1.DTO;
using Org.BouncyCastle.Asn1.Misc;
using OperationType = DDDSample1.Domain.OperationTypes.OperationType;

namespace DDDSample1.Domain.OperationRequests
{
	public class OperationRequestService
	{
		private readonly IUnitOfWork _workUnit;
		private readonly IOperationRequestRepository _requestRepo;
		private readonly IStaffRepository _staffRepo;
		private readonly IDoctorRepository _doctorRepo;
		private readonly IPatientRepository _patientRepo;
		private readonly IOperationTypeRepository _typeRepo;
		private readonly ILogRepository _logRepo;

		public OperationRequestService() { }
		public OperationRequestService(IUnitOfWork workUnit, IOperationRequestRepository requestRepo, IStaffRepository staffRepo,
			IDoctorRepository doctorRepo, IPatientRepository patientRepo, IOperationTypeRepository typeRepo, ILogRepository logRepo)
		{
			_workUnit = workUnit;
			_requestRepo = requestRepo;
			_staffRepo = staffRepo;
			_doctorRepo = doctorRepo;
			_patientRepo = patientRepo;
			_typeRepo = typeRepo;
			_logRepo = logRepo;
		}

		public virtual async Task<OperationRequestDTO> CreateRequest(string patientId, string operationType, string deadline, string priority, TokenDto token)
		{

			Staff staff = await _staffRepo.GetStaffByUser(new Username(token.UserId));
			Doctor doctor = await _doctorRepo.GetDoctorByLicenseNumber(staff.Id);

			var patient = await this._patientRepo.GetByIdAsync(new MedicalRecordNumber(patientId));

			if (patient == null || doctor == null)
				throw new ArgumentException("Patient/Doctor doesn't exist.");

			var type = await this._typeRepo.GetByNameLatest(new OperationTypeName(operationType));
			if (type == null)
				throw new ArgumentException("Operation type doesn't exist.");

			OperationRequestBuilder builder = new OperationRequestBuilder();

			if (!CheckIfSpecialist(type.RequiredSpecialists, doctor.Staff.specializationId))
				throw new ArgumentException("Doctor specialization must be in operation type.");

			builder.WithDoctor(doctor)
				.WithPatient(patient)
				.WithType(type)
				.WithPriority(priority)
				.WithDeadline(DateTime.Parse(deadline));

			var request = await _requestRepo.AddAsync(builder.Build());
			await this._workUnit.CommitAsync();

			return request.toDTO();
		}

		public virtual async Task<OperationRequestDTO> EditRequest(string id, /* string name,  */string priority, string deadline, TokenDto token)
		{
			if (/* String.IsNullOrEmpty(name) &&  */String.IsNullOrEmpty(priority) && String.IsNullOrEmpty(deadline))
				throw new ArgumentException("No parameters to be changed were given.");

			OperationRequest request = await _requestRepo.GetRequestById(new OperationRequestId(id));

			Staff staff = await _staffRepo.GetStaffByUser(new Username(token.UserId));
			Doctor doc = await _doctorRepo.GetDoctorByLicenseNumber(staff.Id);

			if (!request.Doctor.Id.Equals(doc.Id))
				throw new ArgumentException("You can only update requests you have made.");

			// OperationTypeName otName = String.IsNullOrEmpty(name) ? null : new(name);
			// OperationType type = null;

			/* if (otName != null)
			{
				type = await _typeRepo.GetByNameLatest(otName);

				if (type == null)
					throw new ArgumentException("No operation by that name exists.");

				if (!CheckIfSpecialist(type.RequiredSpecialists, doc.Staff.specializationId))
					throw new ArgumentException("Doctor specialization must be in operation type.");
			} */

			OperationRequest old = await _requestRepo.GetByIdAsync(new OperationRequestId(id));

			LogsBuilder logsBuilder = new();

			Log log = logsBuilder
				.WithDateAndTime(new DateAndTime(DateTime.Now))
				.WithInformation(old.ToString())
				.WithID(old.Id.AsString())
				.WithObjectType(ObjectLoggedType.OPERATION_REQUEST.ToString())
				.Build();

			/* if (otName != null)
				old.OperationType = type; */

			if (!String.IsNullOrEmpty(priority))
			{
				if (!Enum.TryParse<OperationPriority>(priority, true, out _))
					throw new ArgumentException("Priority does not exist.");

				Enum.TryParse<OperationPriority>(priority, true, out OperationPriority prio);
				old.OperationPriority = prio;
			}

			if (!String.IsNullOrEmpty(deadline))
			{
				old.OperationDeadline = DateTime.Parse(deadline);
			}

			await _logRepo.AddAsync(log);

			OperationRequest ret = _requestRepo.Update(old);

			await _workUnit.CommitAsync();

			return old.toDTO();
		}

		public virtual async Task<OperationRequestDTO> DeleteOperationRequest(string id, TokenDto token)
		{
			OperationRequest request = await _requestRepo.GetRequestById(new OperationRequestId(id));

			if (request == null)
				return null;

			Staff staff = await _staffRepo.GetStaffByUser(new Username(token.UserId));
			Doctor doc = await _doctorRepo.GetDoctorByLicenseNumber(staff.Id);

			if (!request.Doctor.Id.Equals(doc.Id))
				throw new ArgumentException("You can only delete requests you have made.");

			_requestRepo.Remove(request);
			await _workUnit.CommitAsync();

			return request.toDTO();
		}

		public virtual async Task<List<OperationRequestDTO>> ListOperationRequest(string patientID, string operationName, string priority, string status, TokenDto token)
		{
			Staff staff = await _staffRepo.GetStaffByUser(new Username(token.UserId));
			Doctor doc = await _doctorRepo.GetDoctorByLicenseNumber(staff.Id);

			var dto = new QueryDataDto
			{
				Name = patientID,
				OperationType = operationName,
				Priority = priority,
				Status = status
			};

			List<OperationRequest> list = await _requestRepo.GetRequestByDoctor(doc.Id, dto);
			return list.ConvertAll(s => s.toDTO());
		}

		public virtual async Task<OperationRequestDTO> GetOperationRequestById(string id){
			var request = await _requestRepo.GetRequestById(new OperationRequestId(id));
			if (request == null)
				return null;

			return request.toDTO();
		}


		public async Task<OperationRequestDTO> GetByIdAsync(string opCode){
			return (await this._requestRepo.GetRequestById(new OperationRequestId(opCode))).toDTO();
		}

		public virtual async Task<List<OperationRequestDTO>> OperationRequestsForAdmin()
		{
			List<OperationRequest> list = await _requestRepo.GetRequestsForAdmin();
			return list.ConvertAll(s => s.toDTO());
		}

		private bool CheckIfSpecialist(List<RequiredSpecialist> specialists, SpecializationCode id)
		{
			foreach (RequiredSpecialist specialist in specialists)
				if (id.Equals(specialist.Specialization.Id))
					return true;

			return false;
		}

	}
}