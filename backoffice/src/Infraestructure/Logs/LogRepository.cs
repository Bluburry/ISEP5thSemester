using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DDDSample1.Domain.HospitalPatient;
using DDDSample1.Infrastructure;
using DDDSample1.Infrastructure.Shared;
using Microsoft.EntityFrameworkCore;

namespace DDDSample1.Domain.Logs
{
    public class LogRepository : BaseRepository<Log, LogId>, ILogRepository
    {
        private readonly HospitalDbContext _context;
        public LogRepository(HospitalDbContext context) : base(context.ShadowTable) {
            _context = context;
        }

        public async Task<Log> GetLastUpdateLogByLoggedIdAsync(string loggedId) {
            return await _context.ShadowTable
            .Where(log => log.LoggedId == loggedId && log.loggedType == ObjectLoggedType.PATIENT_UPDATE_ATTEMPT)
            .OrderByDescending(log => log.LoggedDate.DateTime) // Use the DateTime property for sorting
            .FirstOrDefaultAsync();
        }
    }
}