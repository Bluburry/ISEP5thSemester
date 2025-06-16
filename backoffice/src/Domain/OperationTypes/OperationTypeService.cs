using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DDDSample1.Domain.OperationPhases;
using DDDSample1.Domain.RequiredSpecialists;
using DDDSample1.Domain.Shared;
using DDDSample1.Domain.Specializations;
using DDDSample1.Domain.ValueObjects;

namespace DDDSample1.Domain.OperationTypes
{

	public class OperationTypeService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IOperationTypeRepository _repo;
		private readonly ISpecializationRepository _specialRepo;
		private readonly IRequiredSpecialistRepository _reqSpecialRepo;
		private readonly IOperationPhaseRepository _opPhaseRepo;

		public OperationTypeService() { }

		public OperationTypeService(IUnitOfWork unitOfWork, IOperationTypeRepository repo,
			ISpecializationRepository specialRepo, IRequiredSpecialistRepository reqSpecialRepo,
			IOperationPhaseRepository opPhaseRepo)
		{
			_unitOfWork = unitOfWork;
			_repo = repo;
			_specialRepo = specialRepo;
			_reqSpecialRepo = reqSpecialRepo;
			_opPhaseRepo = opPhaseRepo;
		}

		public virtual async Task<List<OperationTypeDTO>> GetAllAsync()
		{
			List<OperationType> list = await _repo.GetAllTest();

			// List<OperationType> list2 = _repo.GetAllTest2();

			// List<OperationTypeDTO> listDTO2 = list2.ConvertAll(ot => ot.ToDTO());

			List<OperationTypeDTO> listDTO = list.ConvertAll(ot => ot.ToDTO());

			return listDTO;
		}

		public virtual async Task<OperationTypeDTO> GetByNameAsync(OperationTypeName operationName)
		{
			OperationType ot = await _repo.GetByNameLatest(operationName);

			return ot?.ToDTO();
		}

		public virtual async Task<List<OperationTypeDTO>> GetAllByNameAsync(OperationTypeName operationName)
		{
			List<OperationType> list = await _repo.GetAllByName(operationName);

			List<OperationTypeDTO> listDTO = list.ConvertAll(ot => ot.ToDTO());

			return listDTO;
		}

		public virtual async Task<OperationTypeDTO> GetByIDAsync(OperationTypeID id)
		{
			OperationType ot = await _repo.GetByIdAsync(id);

			return ot?.ToDTO();
		}

		public virtual async Task<List<OperationTypeDTO>> FilteredGet(string operationName, string specialization, string activeStatus)
		{
			OperationTypeName otName = String.IsNullOrEmpty(operationName) ? null : new OperationTypeName(operationName);
			// SpecializationName spName = String.IsNullOrEmpty(specialization) ? null : new SpecializationName(specialization);
			List<OperationType> list = [];
			/* if (!string.IsNullOrEmpty(activeStatus) && Enum.TryParse<ActivationStatus>(activeStatus, true, out ActivationStatus status))
				list = await _repo.GetFiltered(otName, spName, status);
			else */
			list = await _repo.GetFiltered(otName, specialization, null);

			List<OperationTypeDTO> listDTO = [];
			foreach (OperationType ot in list)
			{
				if (ot.ActivationStatus.Equals(ActivationStatus.DEACTIVATED))
					continue;
				listDTO.Add(ot.ToDTO());
			}
			foreach (OperationType ot in list)
			{
				if (ot.ActivationStatus.Equals(ActivationStatus.DEACTIVATED) &&
					!listDTO.Any(dto => dto.OperationName.Equals(ot.OperationTypeName.ToString())))
					listDTO.Add(ot.ToDTO());
			}

			if (Enum.TryParse<ActivationStatus>(activeStatus, true, out ActivationStatus status))
				listDTO.RemoveAll(dto => !dto.ActivationStatus.Equals(status.ToString()));
			// List<OperationTypeDTO> listDTO = list.ConvertAll(ot => ot.ToDTO());

			return listDTO;
		}

		public virtual async Task<OperationTypeDTO> RegisterOperationType(OperationTypeDTO opDTO)
		{
			OperationType check = await _repo.GetByName(new OperationTypeName(opDTO.OperationName));

			if (check != null)
				throw new ArgumentException("An operation with that name already exists.");

			List<Specialization> sp = [];

			Specialization spTest;

			foreach (string specialization in opDTO.SpecialistNames)
			{
				spTest = await _specialRepo.GetByName(specialization);
				if (spTest == null)
					throw new ArgumentException("Couldn't find a specialization by that name.", specialization);
				sp.Add(spTest);
			}

			OperationType ot = NewOperation(opDTO, sp);
			/* OperationTypeBuilder builder = new();

			List<Specialization> sp = [];

			foreach (string specialization in opDTO.SpecialistNames)
			{
				sp.Add(await _specialRepo.GetByIdAsync(new SpecializationName(specialization)));
			}

			OperationType ot =
			builder.WithOperationTypeName(opDTO.OperationName)
				.WithVersion(opDTO.VersionNumber)
				.CreateOperationType()
				.WithEstimatedDuration(opDTO.EstimatedDuration)
				.WithRequiredSpecialists(sp, opDTO.SpecialistsCount)
				.WithOperationPhases(opDTO.PhaseNames, opDTO.PhasesDuration)
				.Build(); */

			OperationType ret = await _repo.AddAsync(ot);
			await _unitOfWork.CommitAsync();

			return ret.ToDTO();
		}

		public virtual async Task<OperationTypeDTO> UpdateOperationType(OperationTypeName name, OperationTypeDTO opDTO)
		{
			if (!name.OperationName.Equals(opDTO.OperationName))
				throw new ArgumentException("Name specified and name passed in dto do not match.");

			// TODO: check if dto name and querry name are equal
			OperationType old = await _repo.GetByNameLatest(new OperationTypeName(opDTO.OperationName));

			if (old == null)
				throw new ArgumentException("There must already be an old version of an operation to edit.");

			List<Specialization> sp = [];

			Specialization spTest;

			foreach (string specialization in opDTO.SpecialistNames)
			{
				spTest = await _specialRepo.GetByName(specialization);
				if (spTest == null)
					throw new ArgumentException("Couldn't find a specialization by that name.", specialization);
				sp.Add(spTest);
			}

			opDTO.VersionNumber = (old.VersionNumber + 1).ToString();

			OperationType ot = NewOperation(opDTO, sp);

			old.DeactivateOperationType();
			old.ChangeEndDateNow();

			_repo.Update(old);
			OperationType ret = await _repo.AddAsync(ot);
			await _unitOfWork.CommitAsync();

			return ret.ToDTO();
		}

		public virtual async Task<OperationTypeDTO> DeactivateByName(OperationTypeName name)
		{
			OperationType old = await _repo.GetByNameLatest(name);

			old.DeactivateOperationType();

			OperationType ret = _repo.Update(old);
			await _unitOfWork.CommitAsync();

			return ret.ToDTO();
		}

		public virtual async Task<List<string>> GetSpecializations()
		{
			List<Specialization> sp = await _specialRepo.GetAll();
			List<string> ret = sp.ConvertAll(ot => ot.ToString());

			return ret;
		}

		private OperationType NewOperation(OperationTypeDTO opDTO, List<Specialization> sp)
		{
			OperationTypeBuilder builder = new();

			OperationType ot =
			builder.WithOperationTypeName(opDTO.OperationName)
				.WithVersion(opDTO.VersionNumber)
				.CreateOperationType()
				.WithEstimatedDuration(opDTO.EstimatedDuration)
				.WithRequiredSpecialists(sp, opDTO.SpecialistsCount, opDTO.SpecialistPhases)
				.WithOperationPhases(opDTO.PhaseNames, opDTO.PhasesDuration)
				.Build();

			return ot;
		}
	}
}