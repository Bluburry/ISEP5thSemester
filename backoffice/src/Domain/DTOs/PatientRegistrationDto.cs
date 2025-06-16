using System;
using System.Collections.Generic;
using System.Linq;
using DDDSample1.Domain.ContactInformations;
using DDDSample1.Domain.HospitalAppointment;
using DDDSample1.Domain.Shared;
using DDDSample1.Domain.Users;
using DDDSample1.Domain.ValueObjects;

namespace DDDSample1.DTO 
{
    public class PatientRegistrationDto
    {
        public string firstName { get;  set; }        
        public string lastName { get;  set; }
        public string fullName { get;  set; }
        public string gender { get; set; }
        public string dateOfBirth { get; set;}
        public string email {get; set;}
        public string phone { get;  set; }
        public string emergencyContact { get; set;}
        public string clinicalDetails {get; set;}

        public PatientRegistrationDto()
        {
        }

        public PatientRegistrationDto(string firstName, string lastName, string fullName, string gender, string dateOfBirth, string email, string phone, string emergencyContact, string clinicalDetails)
        {
            this.firstName = firstName;
            this.lastName = lastName;
            this.fullName = fullName;
            this.gender = gender;
            this.dateOfBirth = dateOfBirth;
            this.email = email;
            this.phone = phone;
            this.emergencyContact = emergencyContact;
            this.clinicalDetails = clinicalDetails;
        }

    }
}
