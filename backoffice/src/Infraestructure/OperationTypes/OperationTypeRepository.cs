using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DDDSample1.Domain.OperationTypes;
using DDDSample1.Domain.Specializations;
using DDDSample1.Domain.ValueObjects;
using DDDSample1.Infrastructure.Shared;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Esf;

namespace DDDSample1.Infrastructure.OperationTypes
{
	public class OperationTypeRepository : BaseRepository<OperationType, OperationTypeID>, IOperationTypeRepository
	{
		private readonly HospitalDbContext _context;

		public OperationTypeRepository(HospitalDbContext context) : base(context.OperationTypes) { _context = context; }

		public async Task<List<OperationType>> GetAllTest()
		{
			IQueryable<OperationType> ret = _context.OperationTypes
				.Include(ot => ot.OperationTypeName)
				.Include(ot => ot.OperationPhases)
				.Include(ot => ot.RequiredSpecialists)
					.ThenInclude(rs => rs.Specialization)
				.Include(ot => ot.EstimatedDuration)
				.Include(ot => ot.OperationTypeEndDate)
				.Include(ot => ot.OperationTypeStartDate);
			/* .Select(ot => new {
				OperationType = ot,
				Specializations =  ot.RequiredSpecialists.Select(rs => rs.Specialization)
			}); */
			// .Include(ot => ot.Id)
			// .Select(rs => rs.Specialization))
			// .Include(ot => ot.ActivationStatus)
			// .Include(ot => ot.VersionNumber);

			return await ret.ToListAsync();
		}

		public async Task<OperationType> GetByName(OperationTypeName otName)
		{
			IQueryable<OperationType> ret = _context.OperationTypes
				.Include(ot => ot.OperationTypeName)
				.Include(ot => ot.OperationPhases)
				.Include(ot => ot.RequiredSpecialists)
					.ThenInclude(rs => rs.Specialization)
				.Include(ot => ot.EstimatedDuration)
				.Include(ot => ot.OperationTypeEndDate)
				.Include(ot => ot.OperationTypeStartDate)
				.Where(ot => ot.OperationTypeName.OperationName.Equals(otName.OperationName));
			// ot => ot.OperationTypeName.ToString().Equals(otName.ToString())
			// ot => otName.ToString().Equals(ot.OperationTypeName.ToString())
			// ot => ot.OperationTypeName.Equals(otName)
			/* .Include(ot => ot.OperationTypeName)
			.Include(ot => ot.OperationPhases)
			.Include(ot => ot.RequiredSpecialists)
			.Include(ot => ot.EstimatedDuration)
			.Include(ot => ot.OperationTypeStartDate)
			.Include(ot => ot.OperationTypeEndDate)
			.Include(ot => ot.ActivationStatus)
			.Include(ot => ot.VersionNumber);*/

			return await ret.FirstOrDefaultAsync();
		}

		public async Task<OperationType> GetByNameLatest(OperationTypeName otName)
		{
			IQueryable<OperationType> ret = _context.OperationTypes
				.Include(ot => ot.OperationTypeName)
				.Include(ot => ot.OperationPhases)
				.Include(ot => ot.RequiredSpecialists)
					.ThenInclude(rs => rs.Specialization)
				.Include(ot => ot.EstimatedDuration)
				.Include(ot => ot.OperationTypeEndDate)
				.Include(ot => ot.OperationTypeStartDate)
				.Where(ot => ot.OperationTypeName.OperationName.Equals(otName.OperationName))
				.OrderByDescending(ot => ot.VersionNumber);
				
			return await ret.FirstOrDefaultAsync();
		}

		public async Task<List<OperationType>> GetAllByName(OperationTypeName otName)
		{
			IQueryable<OperationType> ret = _context.OperationTypes
				.Include(ot => ot.OperationTypeName)
				.Include(ot => ot.OperationPhases)
				.Include(ot => ot.RequiredSpecialists)
					.ThenInclude(rs => rs.Specialization)
				.Include(ot => ot.EstimatedDuration)
				.Include(ot => ot.OperationTypeEndDate)
				.Include(ot => ot.OperationTypeStartDate)
				.Where(ot => ot.OperationTypeName.OperationName.Equals(otName.OperationName));

			return await ret.ToListAsync();
		}

		public async Task<List<OperationType>> GetFiltered(OperationTypeName otName, string spName, ActivationStatus? status)
		{
			IQueryable<OperationType> ret = _context.OperationTypes
				.Include(ot => ot.OperationTypeName)
				.Include(ot => ot.OperationPhases)
				.Include(ot => ot.RequiredSpecialists)
					.ThenInclude(rs => rs.Specialization)
				.Include(ot => ot.EstimatedDuration)
				.Include(ot => ot.OperationTypeEndDate)
				.Include(ot => ot.OperationTypeStartDate);
				/* .Where(ot => ot.OperationTypeName.OperationName.Equals(otName.OperationName))
				.Where(ot => ot.ActivationStatus.Equals(status))
				.Where(ot => ot.RequiredSpecialists.Any(rs => rs.SpecializationName.Equals(spName))); */
			if (otName != null)
				ret = ret.Where(ot => ot.OperationTypeName.OperationName.Equals(otName.OperationName));
			if (status != null)
				ret = ret.Where(ot => ot.ActivationStatus.Equals(status));
			if (spName != null)
				ret = ret.Where(ot => ot.RequiredSpecialists.Any(rs => rs.Specialization.SpecializationName.Equals(spName)));
				

			return await ret.ToListAsync();
		}

		public List<OperationType> GetAllTest2()
		{
			IEnumerable<OperationType> ret = _context.OperationTypes
				// .Include(ot => ot.Id)
				.Include(ot => ot.OperationTypeName)
				.Include(ot => ot.OperationPhases)
				.Include(ot => ot.RequiredSpecialists)
				.ThenInclude(rs => rs.Specialization)
				.Include(ot => ot.EstimatedDuration)
				.Include(ot => ot.OperationTypeEndDate)
				.Include(ot => ot.OperationTypeStartDate)
				// .Select(rs => rs.Specialization))
				// .Include(ot => ot.ActivationStatus)
				// .Include(ot => ot.VersionNumber)
				.AsEnumerable();

			return ret.ToList();
			// throw new System.NotImplementedException();
		}
	}
}