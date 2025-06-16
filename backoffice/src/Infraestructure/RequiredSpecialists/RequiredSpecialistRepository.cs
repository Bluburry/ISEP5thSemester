using DDDSample1.Domain.RequiredSpecialists;
using DDDSample1.Infrastructure.Shared;

namespace DDDSample1.Infrastructure.RequiredSpecialists
{
	public class RequiredSpecialistRepository : BaseRepository<RequiredSpecialist, RequiredSpecialistID>, IRequiredSpecialistRepository
	{
		private readonly HospitalDbContext _context;
		public RequiredSpecialistRepository(HospitalDbContext context) : base(context.RequiredSpecialists) { }
	}
}
