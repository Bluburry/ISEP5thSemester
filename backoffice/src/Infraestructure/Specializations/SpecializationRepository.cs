using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DDDSample1.Domain.Specializations;
using DDDSample1.Infrastructure.Shared;
using Microsoft.EntityFrameworkCore;

namespace DDDSample1.Infrastructure.Specializations
{
	public class SpecializationRepository : BaseRepository<Specialization, SpecializationCode>, ISpecializationRepository
	{
		private readonly HospitalDbContext _context;

		public SpecializationRepository(HospitalDbContext context) : base(context.Specializations) { _context = context; }

		public async Task<List<Specialization>> GetAll()
		{
			IQueryable<Specialization> ret = _context.Specializations;

			return await ret.ToListAsync();
		}

		public async Task<Specialization> GetByName(string specializationName)
		{
			IQueryable<Specialization> ret = _context.Specializations
				/* .Include(sp => sp.SpecializationName)
				.Include(sp => sp.SpecializationDescription) */
				.Where(sp => sp.SpecializationName.Equals(specializationName));

			return await ret.FirstOrDefaultAsync();
		}
	}
}