

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DDDSample1.Domain.Doctors;
using DDDSample1.Domain.HospitalPatient;
using DDDSample1.Domain.OperationRequests;
using DDDSample1.Domain.ValueObjects;
using DDDSample1.DTO;
using DDDSample1.Infrastructure.Shared;
using Microsoft.EntityFrameworkCore;

namespace DDDSample1.Infrastructure.OperationRequests
{
	public class OperationRequestRepository : BaseRepository<OperationRequest, OperationRequestId>, IOperationRequestRepository
	{
		private readonly HospitalDbContext _context;

		public OperationRequestRepository(HospitalDbContext context) : base(context.OperationRequests) { _context = context; }

		public async Task<OperationRequest> GetRequestById(OperationRequestId id)
		{
			var ret = _context.OperationRequests
				.Include(or => or.Doctor)
				.Include(or => or.Patient)
				.Include(or => or.OperationType)
				.ThenInclude(type => type.RequiredSpecialists)
				.ThenInclude(spec => spec.Specialization)
				.Where(or => or.Id.Equals(id));

			return await ret.FirstOrDefaultAsync();
		}

		public async Task<List<OperationRequest>> GetRequestByDoctor(DoctorId id, QueryDataDto queryData)
		{
			var req = _context.OperationRequests
				.Include(or => or.Doctor)
				.Include(or => or.Patient)
				.Include(or => or.OperationType)
				.ThenInclude(type => type.RequiredSpecialists)
				.ThenInclude(spec => spec.Specialization)
				.Where(or => or.Doctor.Id.Equals(id));

			if (queryData.Name != null)
			{
				req = req.Where(s => s.Patient.Id.Equals(new MedicalRecordNumber(queryData.Name)));
			}
			if (queryData.OperationType != null)
			{
				req = req.Where(s => s.OperationType.OperationTypeName.OperationName.Equals(queryData.OperationType));
			}
			if (queryData.Priority != null)
			{
				Enum.TryParse<OperationPriority>(queryData.Priority, true, out OperationPriority op);
				req = req.Where(s => s.OperationPriority.Equals(op));
			}
			if (queryData.Status != null)
			{
				Enum.TryParse<OperationStatus>(queryData.Priority, true, out OperationStatus os);
				req = req.Where(s => s.OperationStatus.Equals(os));
			}

			return await req.ToListAsync(); // Convert to a list before returning
		}

		public async Task<List<OperationRequest>> GetRequestsForAdmin()
		{
			var req = _context.OperationRequests
				.Include(or => or.Doctor)
				.Include(or => or.Patient)
				.Include(or => or.OperationType)
				.ThenInclude(type => type.RequiredSpecialists)
				.ThenInclude(spec => spec.Specialization);

			return await req.ToListAsync(); 
		}
	}
}