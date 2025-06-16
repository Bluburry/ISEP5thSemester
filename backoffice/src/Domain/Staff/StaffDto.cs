using System.Collections;
using System.Collections.Generic;
using DDDSample1.Domain.Shared;

namespace DDDSample1.Domain.HospitalStaff
{
    public class StaffDto
    {
        public string LicenseNumber { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string FirstName { get; set; }
        public string Fullname {get; set;}
        public string LastName { get; set; }
        public string Specialization {get; set;}
        public List<string> AvailabilitySlots { get; set; }

        public string Status { get; set; }
        // Parameterless constructor needed for deserialization
        public StaffDto() { }

        // Constructor to easily create a StaffDto with all properties
        public StaffDto(string licenseNumber, string email, string phone, string firstName, string lastName,  List<string> availabilitySlots, string specialization, string fullName)
        {
            LicenseNumber = licenseNumber;
            Email = email;
            Phone = phone;
            FirstName = firstName;
            LastName = lastName;
            AvailabilitySlots = availabilitySlots;
            Specialization = specialization;
            Fullname = fullName;
        }

        public override string ToString()
        {
            return $"{LicenseNumber}|{Email}|{Phone}|{FirstName}|{LastName}|{Fullname}|{Specialization}|{string.Join(";", AvailabilitySlots)}";
        }
    }
}
