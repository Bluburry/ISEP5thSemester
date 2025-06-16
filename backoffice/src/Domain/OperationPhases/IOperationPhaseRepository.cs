using DDDSample1.Domain.Shared;

namespace DDDSample1.Domain.OperationPhases
{
	public interface IOperationPhaseRepository : IRepository<OperationPhase, OperationPhaseID>
	{
	}
}