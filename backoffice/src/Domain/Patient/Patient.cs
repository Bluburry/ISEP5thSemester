using System;
using System.Collections.Generic;
using DDDSample1.Domain.ContactInformations;
using DDDSample1.Domain.HospitalAppointment;
using DDDSample1.Domain.Shared;
using DDDSample1.Domain.Users;
using DDDSample1.Domain.ValueObjects;

namespace DDDSample1.Domain.HospitalPatient 
{
    public class Patient : Entity<MedicalRecordNumber>, IAggregateRoot
    {
        public FirstName firstName { get;  set; }        
        public LastName lastName { get;  set; }
        public FullName fullName { get;  set; }
        public Gender gender { get; set; }
        public DateOfBirth dateOfBirth {get; set;}
        public ContactInformationId ContactInformationId {get; set;}
        public ContactInformation ContactInformation { get;  set; }
        public PhoneNumber emergencyContact { get; set;}
        public List<Appointment> appointmentHistory { get; set;}
        public Username userId {get; set;}
		public User TheUser { get; set; }
        

        public Patient()
        {
        }

        public Patient(FirstName firstName,
         LastName lastName,
          FullName fullName,
           Gender gender,
            DateOfBirth dateOfBirth,
             ContactInformation contactInformation,
               PhoneNumber emergencyContact,
                List<Appointment> appointmentHistory,
                 User user)
        {
            this.firstName = firstName;
            this.lastName = lastName;
            this.fullName = fullName;
            this.gender = gender;
            this.dateOfBirth = dateOfBirth;
            this.ContactInformationId = contactInformation.Id;
            this.ContactInformation = contactInformation;
            this.emergencyContact = emergencyContact;
            this.appointmentHistory = appointmentHistory;
            this.userId = user.Id;
            this.TheUser = user;
        }
        

        public Patient(
            MedicalRecordNumber mrn,
                FirstName firstName,
                 LastName lastName,
                  FullName fullName,
                   Gender gender,
                   DateOfBirth dateOfBirth,
                    ContactInformation contactInformation,
                      PhoneNumber emergencyContact,
                       List<Appointment> appointmentHistory,
                        User user)
        {
            this.firstName = firstName;
            this.lastName = lastName;
            this.fullName = fullName;
            this.gender = gender;
            this.dateOfBirth = dateOfBirth;
            this.ContactInformationId = contactInformation.Id;
            this.ContactInformation = contactInformation;
            this.emergencyContact = emergencyContact;
            this.appointmentHistory = appointmentHistory;
            
            this.TheUser = user;

            if(user != null)
            {
                this.userId = user.Id;
            }
            this.Id = mrn;
        }


        public Patient(FirstName firstName, LastName lastName, FullName fullName, Gender gender, DateOfBirth dateOfBirth, ContactInformation contactInformation, PhoneNumber emergencyContact, User user)
        {
            this.firstName = firstName;
            this.lastName = lastName;
            this.fullName = fullName;
            this.gender = gender;
            this.ContactInformationId = contactInformation.Id;
            this.ContactInformation = contactInformation;
            this.emergencyContact = emergencyContact;
            this.dateOfBirth = dateOfBirth;
            this.userId = user.Id;
            this.TheUser = user;
            this.appointmentHistory = new List<Appointment>();
        }

        public Patient(Patient patient)
        {
            this.firstName = patient.firstName;
            this.lastName = patient.lastName;
            this.fullName = patient.fullName;
            this.gender = patient.gender;
            this.ContactInformationId = patient.ContactInformation.Id;
            this.ContactInformation = patient.ContactInformation;
            this.emergencyContact = patient.emergencyContact;
            this.dateOfBirth = patient.dateOfBirth;
            this.userId = patient.userId;
            this.TheUser = patient.TheUser;
            this.appointmentHistory = patient.appointmentHistory;
        }

        public Patient(MedicalRecordNumber nr, FirstName firstName, LastName lastName, FullName fullName, Gender gender, DateOfBirth dateOfBirth, ContactInformation contactInformation, PhoneNumber emergencyContact)
        {
            this.Id = nr;
            this.firstName = firstName;
            this.lastName = lastName;
            this.fullName = fullName;
            this.gender = gender;
            this.ContactInformationId = contactInformation.Id;
            this.ContactInformation = contactInformation;
            this.emergencyContact = emergencyContact;
            this.appointmentHistory = new List<Appointment>();
            this.dateOfBirth = dateOfBirth;

        }


        public PatientDto toDto()
        {
            List<AppointmentDto> appointmentHistory = null;
            if(appointmentHistory != null){
                appointmentHistory =this.appointmentHistory?.ConvertAll(appointment => appointment.toDto());
            }
            return new PatientDto
            {
                mrn = this.Id.AsString(),
                email = this.ContactInformation?.Email?.Value.ToString(),
                phone = this.ContactInformation?.Phone?.Value.ToString(),
                firstName = this.firstName?.ToString(),
                lastName = this.lastName?.ToString(),
                fullName = this.fullName?.ToString(),
                gender= this.gender.ToString(),
                dateOfBirth = this.dateOfBirth?.ToString(),
                emergencyContact = this.emergencyContact?.ToString(),
                appointmentHistory =appointmentHistory,
                userId = this.userId?.AsString()
            };
        }
        public override string ToString()
        {
            return $"Patient [MRN: {this.Id.AsString()}, " +
                $"FirstName: {this.firstName.ToString()}, " +
                $"LastName: {this.lastName.ToString()}, " +
                $"FullName: {this.fullName.ToString()}, " +
                $"Gender: {this.gender.ToString()}, " +
                $"Email: {this.ContactInformation.Email.ToString()}, " +
                $"Phone: {this.ContactInformation.Phone.ToString()}, " +
                $"EmergencyContact: {this.emergencyContact.ToString()}, " +
                $"Appointments: {this.appointmentHistory.Count} appointment(s)]";
        }


        internal void AddAppointement(Appointment appointment)
        {
            this.appointmentHistory.Add(appointment);
        }

        internal void UpdateAppointment(Appointment prevAppointment, Appointment posAppointment)
        {
            this.appointmentHistory.Remove(prevAppointment);
            this.appointmentHistory.Add(posAppointment);
        }

        internal void AddUser(Username username, User user){
            this.TheUser = user;
            this.userId = username;
        }

        public void Anonymize()
            {
                // Clear or replace identifiable information
                this.firstName = new FirstName("Anonymous");
                this.lastName = new LastName("Patient");
                this.fullName = new FullName("Anonymous Patient");
                this.userId = null;
                this.gender = Gender.NONSPECIFIED; // Clear Gender
                this.dateOfBirth = new DateOfBirth("0000-01-01");
                this.ContactInformation.Email = new EmailAddress("anonymous@example.com");
                this.ContactInformation.Phone = new PhoneNumber("0000000000"); // Clear phone number
                this.emergencyContact = new PhoneNumber("0000000000"); // Clear emergency contact
                this.appointmentHistory.Clear(); // Optionally clear appointment history
            }
                public bool IsAnonymized()
        {
            return this.firstName.Equals(new FirstName("Anonymous")) &&
                this.lastName.Equals(new LastName("Patient")) &&
                this.fullName.Equals(new FullName("Anonymous Patient")) &&
                this.userId == null &&
                this.gender == Gender.NONSPECIFIED;
        }

    }
}
