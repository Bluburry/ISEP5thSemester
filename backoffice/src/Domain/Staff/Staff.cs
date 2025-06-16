using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using DDDSample1.Domain.AvailabilitySlots;
using DDDSample1.Domain.ContactInformations;
using DDDSample1.Domain.Shared;
using DDDSample1.Domain.Specializations;
using DDDSample1.Domain.Tokens;
using DDDSample1.Domain.Users;
using DDDSample1.Domain.ValueObjects; 

namespace DDDSample1.Domain.HospitalStaff 
{
    public class Staff : Entity<LicenseNumber>, IAggregateRoot
    {
        public SpecializationCode specializationId {get; set;}
        public Specialization theSpecialization {get; set;}
        public Username userId {get; set;}
		public User TheUser { get; set; }
        public ContactInformationId ContactInformationId {get; set;}
        public ContactInformation ContactInformation { get;  set; }
        public FullName FullName { get;  set; }
        public FirstName FirstName { get;  set; }
        public LastName LastName { get;  set; }
        public List<AvailabilitySlot> AvailabilitySlots { get;  set; }

        public ActivationStatus Status { get; set; }

        private Staff()
        {
            this.AvailabilitySlots = new List<AvailabilitySlot>();
        }

        public Staff(LicenseNumber licenseNumber, ContactInformation contactInfo, FullName fullName, Specialization specialization, User user, FirstName firstName, LastName LastName)
        {
            this.Id = licenseNumber;
            this.ContactInformation = contactInfo;
            this.FullName = fullName;
            this.FirstName = firstName;
            this.LastName = LastName;
            this.AvailabilitySlots = new List<AvailabilitySlot>();
            this.TheUser = user;
            this.theSpecialization = specialization;
            this.Status = ActivationStatus.ACTIVATED;
        }

        public void AddAvailabilitySlot(AvailabilitySlot slot)
        {

            this.AvailabilitySlots.Add(slot);
        }

        public void RemoveAvailabilitySlot(AvailabilitySlot slot)
        {

            this.AvailabilitySlots.Remove(slot);
        }

        public void ChangeSpecialization(Specialization specialization){
            this.specializationId = specialization.Id;
            this.theSpecialization = specialization;
        }

        public void ChangeContactInformation(ContactInformation contact){
            this.ContactInformationId = contact.Id;
            this.ContactInformation = contact;
        }
        
        public void DeactivateStatus(){
            this.Status = ActivationStatus.DEACTIVATED;
        }

        public override string ToString()
        {
            return $"{this.Id.ToString()}|{this.ContactInformation.Email.ToString()}|{this.ContactInformation.Phone.ToString()}|{this.FirstName.ToString()}|{this.LastName.ToString()}|{this.FullName.ToString()}|{this.theSpecialization.Id.ToString()}|{string.Join(";", this.AvailabilitySlots.ConvertAll(slot => slot.ToString()))}";
        }

        public StaffDto toDto()
        {
            return new StaffDto
            {
                LicenseNumber = this.Id.AsString(),
                Email = this.ContactInformation.Email.ToString(),
                Phone = this.ContactInformation.Phone.ToString(),
                FirstName = this.FirstName.ToString(),
                LastName = this.LastName.ToString(),
                Fullname = this.FullName.ToString(),
                Specialization = this.theSpecialization.SpecializationName,
                AvailabilitySlots = this.AvailabilitySlots.ConvertAll(slot => slot.ToString()), 
                Status = this.Status.ToString(),
                // Add any other properties needed from Staff
            };
        }
    }
}
