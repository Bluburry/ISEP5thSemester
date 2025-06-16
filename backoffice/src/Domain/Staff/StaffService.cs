using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DDDSample1.Domain.AvailabilitySlots;
using DDDSample1.Domain.ContactInformations;
using DDDSample1.Domain.Doctors;
using DDDSample1.Domain.HospitalPatient;
using DDDSample1.Domain.Logs;
using DDDSample1.Domain.Shared;
using DDDSample1.Domain.Specializations;
using DDDSample1.Domain.Tokens;
using DDDSample1.Domain.Users;
using DDDSample1.Domain.ValueObjects;
using DDDSample1.DTO;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Logging.Abstractions;
using Org.BouncyCastle.Utilities;

namespace DDDSample1.Domain.HospitalStaff
{
    public class StaffService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStaffRepository _repo;
        private readonly IUserRepository _userRepo;
        private readonly ISpecializationRepository _specRepo;

        private readonly ITokenRepository _tknRepo;
        private readonly IContactInformationRepository _contactRepo;
        private readonly ILogRepository _logsRepo;
        private readonly IAvailabilitySlotsRepository _avalRepo;
        private readonly IDoctorRepository _docRepo;

        public StaffService(IUnitOfWork unitOfWork, IStaffRepository repo,
        ISpecializationRepository specRepo, IUserRepository userRepo,
        ILogRepository logsRepo, IAvailabilitySlotsRepository avalRepo,
        IContactInformationRepository contactRepo, ITokenRepository tokenRepo,
        IDoctorRepository doctorRepo)
        {
            this._unitOfWork = unitOfWork;
            this._repo = repo;
            this._specRepo = specRepo;
            this._userRepo = userRepo;
            this._logsRepo = logsRepo;
            this._avalRepo = avalRepo;
            this._contactRepo = contactRepo;
            this._tknRepo = tokenRepo;
            this._docRepo = doctorRepo;
        }

        public StaffService()
        {
        }

        public virtual async Task<List<StaffDto>> GetStaffList()
        {
            var list = this._repo.GetStaffList().Select(staff => staff.toDto()).ToList();

            /*List<StaffDto> listDto = list.ConvertAll<StaffDto>(staff => new StaffDto{
                LicenseNumber = staff.Id.AsString(),
                Email = staff.TheUser.EmailAddress.ToString(),
                Phone = staff.ContactInformation.ToString(),
                FirstName = staff.FirstName.ToString(),
                Fullname = staff.FullName.ToString(),
                LastName = staff.LastName.ToString(),
                Specialization = staff.theSpecialization.ToString(),
                });*/

            return list;
        }

        public virtual async Task<Staff> GetLatestVersion(string name){
            
            var staff = await this._repo.GetStaffByUser(new Username(name));

            return staff == null ? null : staff;
        }

        public virtual async Task<StaffDto> DisableStaff(string license){
            var staff = await this._repo.GetStaffByLicense(new LicenseNumber(license)); 
            if (staff == null)
                return null;
            
            staff.DeactivateStatus();

            await this._unitOfWork.CommitAsync();
            
            return staff.toDto();
        }

        public virtual async Task<StaffDto> GetByIdAsync(string id)
        {
            var staff = await this._repo.GetStaffByLicense(new LicenseNumber(id));

            return staff == null ? null : staff.toDto();
        }

        public virtual async Task<StaffDto> UpdateVersion(StaffDto dto, string logID, string TokenId)
        {
            Log information = await this._logsRepo.GetByIdAsync(new LogId(logID));
            var parts = information.LoggedInformation.ToString().Split('|');

            if (parts.Length != 8) throw new ArgumentException("The input string is not in the correct format.");

            /*
                LicenseNumber = parts[0],
                Email = parts[1],
                Phone = parts[2],
                FirstName = parts[3],
                LastName = parts[4],
                Fullname = parts[5],
                Specialization = parts[6],
                availabilitySlots = parts[7].Split(';').ToList();
            */
        
            var staff = await this._repo.GetStaffByLicense(new LicenseNumber(parts[0])); 
            if (staff == null)
                return null;

            LogsBuilder logsBuilder = new LogsBuilder();
            logsBuilder.WithObjectType(ObjectLoggedType.STAFF.ToString())
                .WithInformation(staff.ToString())
                .WithDateAndTime(new DateAndTime(DateTime.Now))
                .WithID(staff.Id.ToString());
            
            await this._logsRepo.AddAsync(logsBuilder.Build());

            var email = parts[1];
            var phone = parts[2];
            var specialization = parts[6];
            var availabilitySlots = parts[7].Split(';').ToList();

            this._contactRepo.Remove(staff.ContactInformation);
            staff.ChangeContactInformation(new ContactInformation(new EmailAddress(email), new PhoneNumber(phone)));

            var spec = await _specRepo.GetByName(specialization);
            
            if(spec != null) staff.ChangeSpecialization(spec);
            else throw new ArgumentException("The specialization given does not exist.");

            foreach (string slot in availabilitySlots)
            {

                AvailabilitySlot availability = new AvailabilitySlot(slot);
                var timeSlot = await this._avalRepo.GetAvailabilitySlots(staff.Id, availability.Value);
                if(timeSlot == null){
                    staff.AddAvailabilitySlot(availability);
                    await this._avalRepo.AddAsync(availability);
                }else if(!staff.AvailabilitySlots.Contains(timeSlot)){
                    staff.AddAvailabilitySlot(timeSlot);
                }
                
            } 

            _tknRepo.Remove(await _tknRepo.GetByIdAsync(new TokenId(TokenId)));
            _repo.Update(staff); 

            await this._unitOfWork.CommitAsync();
            return staff.toDto();
        }

        public virtual async Task<Log> UpdateAsync(StaffDto dto)
        {
            
            var staff = await this._repo.GetByIdAsync(new LicenseNumber(dto.LicenseNumber)); 
            if (staff == null)
                return null;

            LogsBuilder logsBuilder = new LogsBuilder();
            Log log = new Log();
            // Update fields
            if(!staff.ContactInformation.Email.Equals(new EmailAddress(dto.Email)) || !staff.ContactInformation.Phone.Equals(new PhoneNumber(dto.Phone))){
                logsBuilder.WithObjectType(ObjectLoggedType.STAFF.ToString())
                    .WithInformation(dto.ToString())
                    .WithDateAndTime(new DateAndTime(DateTime.Now))
                    .WithID(staff.Id.ToString());

                log = await this._logsRepo.AddAsync(logsBuilder.Build());
            }else{
                logsBuilder.WithObjectType(ObjectLoggedType.STAFF.ToString())
                    .WithInformation(staff.ToString())
                    .WithDateAndTime(new DateAndTime(DateTime.Now))
                    .WithID(staff.Id.ToString());

                log = await this._logsRepo.AddAsync(logsBuilder.Build());
                
                var spec = await _specRepo.GetByName(dto.Specialization);
                
                if(spec != null) staff.ChangeSpecialization(spec);
                else throw new ArgumentException("The specialization given does not exist.");
                
                foreach (string slot in dto.AvailabilitySlots)
                {
                    AvailabilitySlot availability = new AvailabilitySlot(slot);
                    var timeSlot = await this._avalRepo.GetAvailabilitySlots(staff.Id, availability.Value);
                    if(timeSlot == null){
                        await this._avalRepo.AddAsync(availability);
                        staff.AddAvailabilitySlot(availability);
                    }else if(!staff.AvailabilitySlots.Contains(timeSlot)){
                        staff.AddAvailabilitySlot(timeSlot);
                    }
                } 
                

                _repo.Update(staff); 
            }
            
            await this._unitOfWork.CommitAsync();

            return log;
        }


        public virtual async Task<bool> DeleteAsync(LicenseNumber id)
        {
            var staff = await this._repo.GetByIdAsync(id);

            this._repo.Remove(staff);
            await this._unitOfWork.CommitAsync();

            return true;
        }

        
        public virtual async Task<StaffDto> RegisterStaff(StaffDto dto)
        {
            var user = await this._userRepo.GetByIdAsync(new Username(dto.Email));

            if(user == null){
                StaffDto blah = new StaffDto();

                blah.Email = "User is null";
                return blah;
            }

            var existingStaff = await this._repo.GetStaffByContact(new ContactInformation(new EmailAddress(dto.Email), new PhoneNumber(dto.Phone)));

            if(existingStaff != null)
                throw new ArgumentException("The contact information is already in use.");
    
            var specialization = await this._specRepo.GetByName(dto.Specialization);

            if(specialization == null)
                throw new ArgumentException("Specialization doesn't exist.");

            StaffBuilder builder = new StaffBuilder();
            
            Staff staff =
            builder.WithFullName(dto.FirstName + " " +dto.LastName)
                .WithContactInformation(dto.Phone, dto.Email)
                .WithLicenseNumber(dto.LicenseNumber)
                .WithSpecialization(specialization)
                .WithUser(user)
                .WithFirstName(dto.FirstName)
                .WithLastNAme(dto.LastName)
                .Build();

            if(staff == null){
                StaffDto blah = new StaffDto();

                blah.Email = "Staff is null";
                return blah;
            }

            await this._repo.AddAsync(staff);
            //string.Equals(dto.Specialization, "Doctor", StringComparison.CurrentCultureIgnoreCase);
            if(dto.Specialization.Equals("Doctor")){
                DoctorFactory doctorFactory = new DoctorFactory();
                var doctor = doctorFactory.CreateDoctor(staff);
                await this._docRepo.AddAsync(doctor);
            }

            await this._unitOfWork.CommitAsync();

            return staff.toDto();
        }

        public virtual IEnumerable<StaffDto> GetFilteredStaff(string license, string name, 
        string email, string specialization, string status)
        {

            var queryData = new QueryDataDto
            {
                LicenseNumber = license, 
                Name = name,
                Email = email,
                Specialization = specialization,
                Status = status
            };

            IEnumerable<Staff> staff = _repo.GetFilteredStaff(queryData);
            
            return staff.Select(staff => staff.toDto()).ToList();
        }  
    }
}
