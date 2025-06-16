using DDDSample1.Domain.Shared;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DDDSample1.Domain.Specializations
{
    public interface ISpecializationRepository : IRepository<Specialization, SpecializationCode>
    {
		Task<List<Specialization>> GetAll();
		Task<Specialization> GetByName(string specializationName);
    }
}
