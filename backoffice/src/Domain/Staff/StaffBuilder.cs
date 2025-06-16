using System;
using System.Collections.Generic;
using DDDSample1.Domain.AvailabilitySlots;
using DDDSample1.Domain.ContactInformations;
using DDDSample1.Domain.Shared;
using DDDSample1.Domain.Specializations;
using DDDSample1.Domain.Users;
using DDDSample1.Domain.ValueObjects;

namespace DDDSample1.Domain.HospitalStaff
{
    public class StaffBuilder
    {
        private LicenseNumber _licenseNumber;
        private ContactInformation _contactInformation;
        private FullName _fullName;
        private FirstName _firstName;
        private LastName _lastName;
        private Specialization _specialization;
        private User _user;
        private List<AvailabilitySlot> _availabilitySlots = new List<AvailabilitySlot>();

        public StaffBuilder WithLicenseNumber(string licenseNumber)
        {
            _licenseNumber = new LicenseNumber(licenseNumber);
            return this;
        }

        public StaffBuilder WithContactInformation(string phone, string email)
        {
            _contactInformation = new ContactInformation(new EmailAddress(email), new PhoneNumber(phone));
            return this;
        }

        public StaffBuilder WithFullName(string fullname)
        {
            _fullName = new FullName(fullname);
            return this;
        }
        public StaffBuilder WithFirstName(string firstName)
        {
            _firstName = new FirstName(firstName);
            return this;
        }
        public StaffBuilder WithLastNAme(string lastName)
        {
            _lastName = new LastName(lastName);
            return this;
        }

        public StaffBuilder WithSpecialization(Specialization specialization)
        {
            _specialization = specialization;
            return this;
        }

        public StaffBuilder WithUser(User user)
        {
            // Here it's assumed user has been created beforehand as creating a staff profile for no user is against system logic.
            _user = user;
            return this;
        }

        public StaffBuilder WithAvailabilitySlots(List<string> slots)
        {
            for(int i = 0; i< slots.Count; i++){
                _availabilitySlots.Add(new AvailabilitySlot(slots[i]));
            }
            return this;
        }
        public Staff Build()
        {
            if (_licenseNumber == null)
                throw new ArgumentException("LicenseNumber is required.");
            if (_contactInformation == null)
                throw new ArgumentException("ContactInformation is required.");
            if (_fullName == null)
                throw new ArgumentException("FullName is required.");
            if (_specialization == null)
                throw new ArgumentException("Specialization is required.");


            var staff = new Staff(_licenseNumber, _contactInformation, _fullName, _specialization, _user, _firstName, _lastName);

            foreach (var slot in _availabilitySlots)
            {
                staff.AddAvailabilitySlot(slot);
            }

            return staff;
        }
    }
}
