using System.Collections.Generic;
using System.Threading.Tasks;
using DDDSample1.Domain.Shared;
using DDDSample1.Domain.Specializations;
using DDDSample1.Domain.ValueObjects;

namespace DDDSample1.Domain.OperationTypes
{
	public interface IOperationTypeRepository : IRepository<OperationType, OperationTypeID>
	{
		Task<List<OperationType>> GetAllTest();
		Task<OperationType> GetByName(OperationTypeName otName);
		Task<OperationType> GetByNameLatest(OperationTypeName otName);
		Task<List<OperationType>> GetAllByName(OperationTypeName otName);
		Task<List<OperationType>> GetFiltered(OperationTypeName otName, string spName, ActivationStatus? status);
		List<OperationType> GetAllTest2();
		// Task<List<OperationType>> GetAll();
		// Task<OperationType> GetAll();
	}
}
