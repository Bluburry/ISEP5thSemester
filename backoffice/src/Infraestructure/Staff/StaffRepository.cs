using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using DDDSample1.Domain.ContactInformations;
using DDDSample1.Domain.HospitalStaff;
using DDDSample1.Domain.Specializations;
using DDDSample1.Domain.Users;
using DDDSample1.Domain.ValueObjects;
using DDDSample1.DTO;
using DDDSample1.Infrastructure.Shared;
using Microsoft.AspNetCore.JsonPatch.Internal;
using Microsoft.EntityFrameworkCore;


namespace DDDSample1.Infrastructure.HospitalStaff
{
    public class StaffRepository : BaseRepository<Staff, LicenseNumber>, IStaffRepository
    {

        private readonly HospitalDbContext _context;
        public StaffRepository(HospitalDbContext context) : base(context.Staff)
        {
            _context = context;
        }

        public IEnumerable<Staff> GetStaffList()
        {
            var staff = _context.Staff
                .Include(s => s.FullName)
                .Include(s => s.ContactInformation)
                .Include(s => s.TheUser)
                .Include(s => s.theSpecialization)
                .AsEnumerable(); 

            return staff.ToList(); 
        }

        public async Task<Staff> GetStaffByContact(ContactInformation contact)
        {
            IQueryable<Staff> staffEmailInUse = _context.Staff
                .Include(s => s.ContactInformation)
                .Include(s => s.TheUser)
                .Include(s => s.theSpecialization)
                .Where(s => s.ContactInformation.Email.Value.Equals(contact.Email.Value));

            IQueryable<Staff> staffPhoneInUse = _context.Staff
                .Include(s => s.ContactInformation)
                .Include(s => s.TheUser)
                .Include(s => s.theSpecialization)
                .Where(s => s.ContactInformation.Phone.Value.Equals(contact.Phone.Value));

            if(staffPhoneInUse.FirstOrDefaultAsync().Result != null)
                return await staffPhoneInUse.FirstOrDefaultAsync();
            else if (staffEmailInUse.FirstOrDefaultAsync().Result != null)
                return await staffEmailInUse.FirstOrDefaultAsync();

            return null;
        }

        public async Task<Staff> GetStaffByLicense(LicenseNumber num)
        {
            IQueryable<Staff> staff = _context.Staff.Where(s => s.Id.Equals(num))
                .Include(s => s.ContactInformation)
                .Include(s => s.TheUser)
                .Include(s => s.theSpecialization)
                .Include(s => s.AvailabilitySlots);
                
            return await staff.FirstOrDefaultAsync();
        }


        public async Task<Staff> GetTemporaryVersion(Username name)
        {
            IQueryable<Staff> staff = _context.Staff.Where(s => s.userId.Equals(name)).OrderByDescending(s => 0);
            return await staff.FirstOrDefaultAsync();
        }

        public async Task<Staff> GetStaffByUser(Username user)
        {
            IQueryable<Staff> staff = _context.Staff.Where(s => s.userId.Equals(user))
                .Include(s => s.ContactInformation)
                .Include(s => s.TheUser)
                .Include(s => s.theSpecialization);
                
            Staff bleh = await staff.FirstOrDefaultAsync();
            return await staff.FirstOrDefaultAsync();
        }

        
        public IEnumerable<Staff> GetFilteredStaff(QueryDataDto queryData)
        {
            // Fetch the staff and include related data
            var staff = _context.Staff
                .Include(s => s.FullName)
                .Include(s => s.TheUser)
                .Include(s => s.ContactInformation)
                .Include(s => s.theSpecialization)
                .AsEnumerable(); // Force client-side evaluation here

            // Filter the staff based on the provided query data
            if (queryData.LicenseNumber != null)
            {
                staff = staff.Where(s => s.Id.Equals(new LicenseNumber(queryData.LicenseNumber)));
            }
            if (queryData.Name != null)
            {
                staff = staff.Where(s => s.FullName.fullname.Equals(queryData.Name));
            }
            if (queryData.Email != null)
            {
                staff = staff.Where(s => s.ContactInformation.Email.Equals(new EmailAddress(queryData.Email)));
            }
            if (queryData.PhoneNumber != null)
            {
                staff = staff.Where(s => s.ContactInformation.Phone.Equals(new PhoneNumber(queryData.PhoneNumber)));
            }
            if (queryData.Specialization != null)
            {
                staff = staff.Where(s => s.theSpecialization.SpecializationName.Equals(queryData.Specialization));
            }
            if(queryData.Status != null && Int32.Parse(queryData.Status) == 0)
            {
                staff = staff.Where(s => s.Status.Equals(ActivationStatus.DEACTIVATED));
            }else if (queryData.Status != null && Int32.Parse(queryData.Status) == 1)
                staff = staff.Where(s => s.Status.Equals(ActivationStatus.ACTIVATED));

            return staff.ToList(); // Convert to a list before returning
        }
    }
}