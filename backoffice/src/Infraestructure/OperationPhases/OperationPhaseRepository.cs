using DDDSample1.Domain.OperationPhases;
using DDDSample1.Infrastructure.Shared;

namespace DDDSample1.Infrastructure.OperationPhases
{
	public class OperationPhaseRepository : BaseRepository<OperationPhase, OperationPhaseID>, IOperationPhaseRepository
	{
		private readonly HospitalDbContext _context;
		public OperationPhaseRepository(HospitalDbContext context) : base(context.OperationPhases) { }
	}
}