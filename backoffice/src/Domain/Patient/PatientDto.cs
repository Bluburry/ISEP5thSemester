using System;
using System.Collections.Generic;
using System.Linq;
using DDDSample1.Domain.ContactInformations;
using DDDSample1.Domain.HospitalAppointment;
using DDDSample1.Domain.Shared;
using DDDSample1.Domain.Users;
using DDDSample1.Domain.ValueObjects;
using DDDSample1.DTO;

namespace DDDSample1.Domain.HospitalPatient 
{
    public class PatientDto
    {
        public string mrn { get; set; }
        public string firstName { get;  set; }        
        public string lastName { get;  set; }
        public string fullName { get;  set; }
        public string gender { get; set; }
        public string dateOfBirth { get; set;}
        public string email {get; set;}
        public string phone { get;  set; }
        public string emergencyContact { get; set;}
        public List<AppointmentDto> appointmentHistory { get; set;}
        public string userId {get;set;}

        public PatientDto()
        {
        }

        public PatientDto(string mrn, string firstName, string lastName, string fullName, string gender, string dateOfBirth, string email, string phone, string emergencyContact, List<AppointmentDto> appointmentHistory, string userId)
        {
            this.mrn = mrn;
            this.firstName = firstName;
            this.lastName = lastName;
            this.fullName = fullName;
            this.gender = gender;
            this.dateOfBirth = dateOfBirth;
            this.email = email;
            this.phone = phone;
            this.emergencyContact = emergencyContact;
            this.appointmentHistory = appointmentHistory;
            this.userId = userId;
        }

        public PatientDto(string mrn, string userId, EditPatientDto_Patient info)
        {
            this.mrn = mrn;
            this.firstName = info.FirstName;
            this.lastName = info.LastName;
            this.fullName = info.Fullname;
            this.gender = info.Gender;
            this.dateOfBirth = info.DateOfBirth;
            this.email = info.Email;
            this.phone = info.Phone;
            this.emergencyContact = info.EmergencyContact;
            this.appointmentHistory = new List<AppointmentDto>();
            this.userId = userId;
        }

        public PatientDto(string toStringDto)
        {
            var pairs = toStringDto.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var pair in pairs)
            {
                var keyValue = pair.Split(new[] { ':' }, 2);
                if (keyValue.Length < 2) continue; // Skip if not a valid pair

                var key = keyValue[0].Trim(); 
                var value = keyValue[1].Trim(); 

                switch (key)
                {
                    case "MRN":
                        this.mrn = value;
                        break;
                    case "FirstName":
                        this.firstName = value;
                        break;
                    case "LastName":
                        this.lastName = value;
                        break;
                    case "FullName":
                        this.fullName = value;
                        break;
                    case "Gender":
                        this.gender = value;
                        break;
                    case "DateOfBirth":
                        this.dateOfBirth = value;
                        break;
                    case "Email":
                        this.email = value;
                        break;
                    case "Phone":
                        this.phone = value;
                        break;
                    case "EmergencyContact":
                        this.emergencyContact = value;
                        break;
                    case "UserID":
                        this.userId = value;
                        break;
                    case "Appointments":
                        // Remove brackets and split by comma for appointments
                        var appointmentsString = value.TrimStart('[').TrimEnd(']').Trim();
                    var appointmentEntries = appointmentsString.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(a => a.Trim()).ToList();

                    this.appointmentHistory = new List<AppointmentDto>();
                    foreach (var appointmentEntry in appointmentEntries)
                    {
                        // Assuming the appointment format is something like "id: dateAndTime: status: reason: diagnosis: notes: staffId: patientId"
                        var appointmentDetails = appointmentEntry.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                        if (appointmentDetails.Length == 8) // Adjust based on the actual number of properties
                        {
                            var appointmentDto = new AppointmentDto
                            {
                                id = appointmentDetails[0].Trim(),
                                dateAndTime = appointmentDetails[1].Trim(),
                                appoitmentStatus = appointmentDetails[2].Trim(),
                                staffId = appointmentDetails[6].Trim(),
                                patientNumber = appointmentDetails[7].Trim()
                            };
                            this.appointmentHistory.Add(appointmentDto);
                        }
                    }
                        break;
                }
            }
        }


        public override string ToString()
        {
            string appointments = "";
            if(appointmentHistory != null){
                appointments = string.Join(", ", appointmentHistory);
            }
            return $"MRN: {mrn}, FirstName: {firstName}, \nLastName: {lastName}, \nFullName: {fullName}, " +
                   $"\nGender: {gender}, \nDateOfBirth: {dateOfBirth},\nEmail: {email}, \nPhone: {phone}, \nBirthDate: {dateOfBirth}," +
                   
                   $"\nEmergencyContact: {emergencyContact}, \nUserID: {userId}, \nAppointments: [{appointments}]";
        }

    }
}
