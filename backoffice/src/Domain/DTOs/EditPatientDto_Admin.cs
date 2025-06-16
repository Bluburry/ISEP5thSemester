using System.Collections.Generic;
using DDDSample1.Domain.HospitalAppointment;
using DDDSample1.Domain.ValueObjects;

namespace DDDSample1.DTO{
    public class EditPatientDto_Admin
{
    public string patientId {get; set;}
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Fullname {get; set;}
    public string Email { get; set; }
    public string Phone { get; set; }
    public AppointmentDto MedicalHistory { get; set; }
    public string DateOfBirth {get; set;}

    public EditPatientDto_Admin()
    {
    }

    public EditPatientDto_Admin(string id, string firstName, string lastName, string fulName, string email, string phone, AppointmentDto medicalHistory, string dateOfBirth)
    {
        patientId = id;
        FirstName = firstName;
        LastName = lastName;
        Fullname = fulName;
        Email = email;
        Phone = phone;
        MedicalHistory = medicalHistory;
        DateOfBirth = dateOfBirth;
    }
}
}
