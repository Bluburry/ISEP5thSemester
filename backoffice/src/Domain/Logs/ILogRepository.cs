using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DDDSample1.Domain.Shared;
using DDDSample1.DTO;

namespace DDDSample1.Domain.Logs
{
    public interface ILogRepository : IRepository<Log, LogId>
    {        
    public Task<Log> GetLastUpdateLogByLoggedIdAsync(string loggedId);    
    }
}
