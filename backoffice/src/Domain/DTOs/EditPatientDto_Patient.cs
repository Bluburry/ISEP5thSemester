using System.Collections.Generic;
using DDDSample1.Domain.HospitalAppointment;
using DDDSample1.Domain.ValueObjects;

namespace DDDSample1.DTO{
    public class EditPatientDto_Patient
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Fullname {get; set;}
    public string Email { get; set; }
    public string Phone { get; set; }
    public AppointmentDto MedicalHistory { get; set; }
    public string DateOfBirth {get; set;}
    public string ClinicalDetails{get; set;}
    public string EmergencyContact {get;set;}
    public string Gender {get;set;}

    public EditPatientDto_Patient()
    {
    }

    public EditPatientDto_Patient(string firstName, string lastName, string fulName, string email, string phone, AppointmentDto medicalHistory, string dateOfBirth, string clinicalDetails, string emergencyContact, string gender)
    {
        FirstName = firstName;
        LastName = lastName;
        Fullname = fulName;
        Email = email;
        Phone = phone;
        MedicalHistory = medicalHistory;
        DateOfBirth = dateOfBirth;
        ClinicalDetails = clinicalDetails;
        EmergencyContact = emergencyContact;
        Gender = gender;
    }
    
}
}
