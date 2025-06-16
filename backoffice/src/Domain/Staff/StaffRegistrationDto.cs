using System;
using System.Collections.Generic;
using DDDSample1.Domain.Users;

namespace DDDSample1.Domain.HospitalStaff
{
    public class StaffRegistrationDto
    {
        // User-related properties
        public string EmailAddress { get; set; }
        public string Role { get; set; }
        public string ActivationStatus { get; set; }

        // Staff-related properties
        public string LicenseNumber { get; set; }
        public string Phone { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string Specialization { get; set; }
        public List<string> AvailabilitySlots { get; set; }

        // Parameterless constructor needed for deserialization
        public StaffRegistrationDto() { }

        // Constructor to initialize all properties
        public StaffRegistrationDto(
            string emailAddress, 
            string role, 
            string activationStatus, 
            string licenseNumber, 
            string phone, 
            string firstName, 
            string lastName, 
            List<string> availabilitySlots, 
            string specialization, 
            string fullName)
        {
            EmailAddress = emailAddress;
            Role = role;
            ActivationStatus = activationStatus;
            LicenseNumber = licenseNumber;
            Phone = phone;
            FirstName = firstName;
            LastName = lastName;
            AvailabilitySlots = availabilitySlots;
            Specialization = specialization;
            FullName = fullName;
        }

        // Method to generate a UserDto from UserStaffDto
        public UserDto ToUserDto()
        {
            return new UserDto(EmailAddress, Role, ActivationStatus);
        }

        // Method to generate a StaffDto from UserStaffDto
        public StaffDto ToStaffDto()
        {
            return new StaffDto(
                LicenseNumber, 
                EmailAddress, // Email is also in UserDto, so we can use it here
                Phone, 
                FirstName, 
                LastName, 
                AvailabilitySlots, 
                Specialization, 
                FullName);
        }
    }
}
