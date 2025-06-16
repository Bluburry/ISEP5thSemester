
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DDDSample1.Domain.Logs;
using DDDSample1.Domain.Shared;
using DDDSample1.Domain.ValueObjects;
using Microsoft.AspNetCore.Routing;

namespace DDDSample1.Domain.Specializations
{
	public class SpecializationService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly ISpecializationRepository _repo;
		private readonly ILogRepository _logRepo;

		public SpecializationService() { }

		public SpecializationService(IUnitOfWork unitOfWork, ISpecializationRepository repo, ILogRepository logRepo)
		{
			_unitOfWork = unitOfWork;
			_repo = repo;
			_logRepo = logRepo;
		}

		public virtual async Task<List<SpecializationDTO>> GetAll()
		{
			List<Specialization> list = await _repo.GetAll();

			List<SpecializationDTO> ret = list.ConvertAll(sp => sp.ToDTO());

			return ret;
		}

		public virtual async Task<SpecializationDTO> FilteredGet(string code, string name)
		{
			if (string.IsNullOrEmpty(code) && string.IsNullOrEmpty(name))
				throw new ArgumentException("Can't filter specialization when both code and name are empty");

			Specialization ret;

			if (string.IsNullOrEmpty(code))
				ret = await _repo.GetByName(name);
			else
				ret = await _repo.GetByIdAsync(new SpecializationCode(code));

			return ret.ToDTO();
		}

		public virtual async Task<SpecializationDTO> CreateSpecialization(string code, string name, string description)
		{
			Specialization check = null;

			/* if (!string.IsNullOrEmpty(code))
				check = await _repo.GetByIdAsync(new SpecializationCode(code));
			if (check != null)
				throw new ArgumentException("A specialization with that code already exists"); */

			check = await _repo.GetByName(name);

			if (check != null)
				throw new ArgumentException("A specialization with that name already exists");

			SpecializationBuilder builder = new();

			Specialization sp = builder.WithSpecializationCode(code)
				.WithSpecializationName(name)
				.WithSpecializationDescription(description)
				.Build();

			Specialization ret = await _repo.AddAsync(sp);

			await _unitOfWork.CommitAsync();

			return ret.ToDTO();
		}

		public virtual async Task<SpecializationDTO> UpdateSpecialization(string code, string name, string description)
		{
			if (string.IsNullOrEmpty(name) && string.IsNullOrEmpty(description))
				throw new ArgumentException("Can't update specialization with null name and description");

			Specialization sp = await _repo.GetByIdAsync(new SpecializationCode(code));

			if (sp.SpecializationName.Equals(name) && (
				string.IsNullOrEmpty(description) || (
				!string.IsNullOrEmpty(sp.SpecializationDescription) &&
				!string.IsNullOrEmpty(description) &&
				sp.SpecializationDescription.Equals(description))))
				throw new ArgumentException("Update values are the same as the original specialization");

			LogsBuilder logsBuilder = new();

			Log log = logsBuilder
				.WithDateAndTime(new DateAndTime(DateTime.Now))
				.WithInformation(sp.ToString())
				.WithID(sp.Id.AsString())
				.WithObjectType(ObjectLoggedType.SPECIALIZATION.ToString())
				.Build();

			if (!string.IsNullOrEmpty(name))
			{
				Specialization test = await _repo.GetByName(name);

				if (test == null)
					sp.ChangeName(name);
				//throw new ArgumentException("Can't rename specialization with a name that is already in use.");
			}

			if (!string.IsNullOrEmpty(description))
				sp.ChangeDescription(description);

			await _logRepo.AddAsync(log);

			Specialization ret = _repo.Update(sp);

			await _unitOfWork.CommitAsync();

			return ret.ToDTO();
		}

		public virtual async Task<SpecializationDTO> DeleteSpecialization(string code)
		{
			Specialization sp = await _repo.GetByIdAsync(new SpecializationCode(code));

			if (sp == null)
				return null;

			_repo.Remove(sp);
			await _unitOfWork.CommitAsync();

			return sp.ToDTO();
		}
	}
}