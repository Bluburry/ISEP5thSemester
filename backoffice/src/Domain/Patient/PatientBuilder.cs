using System;
using System.Collections.Generic;
using DDDSample1.Domain.ContactInformations;
using DDDSample1.Domain.HospitalAppointment;
using DDDSample1.Domain.Shared;
using DDDSample1.Domain.Users;
using DDDSample1.Domain.ValueObjects;

namespace DDDSample1.Domain.HospitalPatient
{
    public class PatientBuilder
    {
        private MedicalRecordNumber _mrn;
        private FirstName _firstName;
        private LastName _lastName;
        private FullName _fullName;
        private Gender _gender;
        private DateOfBirth _dateOfBirth;
        private ContactInformation _contactInformation;
        private PhoneNumber _emergencyContact;
        private List<Appointment> _appointmentHistory = new List<Appointment>();
        private User _user;

        public PatientBuilder WithContactInformation(string phone, string email)
        {
            _contactInformation = new ContactInformation(new EmailAddress(email), new PhoneNumber(phone));
            return this;
        }

        public PatientBuilder WithFullName(string fullname)
        {
            _fullName = new FullName(fullname);
            return this;
        }
        public PatientBuilder WithFirstName(string firstName)
        {
            _firstName = new FirstName(firstName);
            return this;
        }
        public PatientBuilder WithLastName(string lastName)
        {
            _lastName = new LastName(lastName);
            return this;
        }

        public PatientBuilder WithGender(string gender)
        {
            if (gender.ToUpper().Equals("MALE")){_gender= Gender.MALE;}
            else if (gender.ToUpper().Equals("FEMALE")){_gender = Gender.FEMALE;}
            else if (gender.ToUpper().Equals("OTHER")){_gender = Gender.OTHER;}
            else if (gender.ToUpper().Equals("NONSPECIFIED")){_gender = Gender.NONSPECIFIED;}
            else { throw new ArgumentException("Gender must be valid."); }
            return this;
        }

        public PatientBuilder WithMedicalRecordNumber()
        {
            _mrn = MedicalRecordNumber.GenerateMRN();
            return this;
        }

        public PatientBuilder WithEmergencyContactNumber(string emergencyContact)
        {
            if(emergencyContact is null || emergencyContact == ""){
                throw new ArgumentException("EmergencyContact is required.");
            }
            _emergencyContact = new PhoneNumber(emergencyContact);
            return this;
        }

        public PatientBuilder WithPatient(Patient patient)
        {
            if (patient == null)
                throw new ArgumentNullException(nameof(patient));
            _mrn = patient.Id;
            _firstName = patient.firstName; 
            _lastName = patient.lastName;     
            _fullName = patient.fullName;     
            _gender = patient.gender;         
            _contactInformation = patient.ContactInformation; 
            _emergencyContact = patient.emergencyContact; 
            _appointmentHistory = patient.appointmentHistory; 
            _user = patient.TheUser; 
            return this;
        }


        public PatientBuilder WithDateOfBirth(string dateOfBirth){
            if (dateOfBirth is null || dateOfBirth == "")
            {
                throw new ArgumentException("Birth Date cannot be empty.");
            }
            _dateOfBirth = new DateOfBirth(dateOfBirth);
            return this;
        }

        // public PatientBuilder WithAppointments(List<AppointmentDto> appointments)
        // {
        //     for(int i = 0; i< appointments.Count; i++){
        //         _appointmentHistory.Add(appointments[i]);
        //     }
        //     return this;
        // }

        public Patient Build()
            {
                Patient patient;
                if (_contactInformation == null)
                    throw new ArgumentException("ContactInformation is required.");
                if (_fullName == null)
                    throw new ArgumentException("FullName is required.");
                if (_firstName == null || string.IsNullOrWhiteSpace(_firstName.ToString()))
                    throw new ArgumentException("FirstName is required.");
                if (_lastName == null || string.IsNullOrWhiteSpace(_lastName.ToString()))
                    throw new ArgumentException("LastName is required.");
                if (_emergencyContact == null || string.IsNullOrWhiteSpace(_emergencyContact.ToString()))
                    throw new ArgumentException("EmergencyContact is required.");
                if (_dateOfBirth == null)
                    throw new ArgumentException("DateOfBirth is required.");

                if(_user != null)
                {
                    patient = new Patient(
                        _mrn,
                        _firstName,
                        _lastName,
                        _fullName,
                        _gender,
                        _dateOfBirth,
                        _contactInformation,
                        _emergencyContact,
                        _appointmentHistory,
                        _user
                    );
            }
            else
            {
                patient = new Patient(
                        _mrn,
                        _firstName,
                        _lastName,
                        _fullName,
                        _gender,
                        _dateOfBirth,
                        _contactInformation,
                        _emergencyContact
                );
            }

                foreach (var appointment in _appointmentHistory)
                {
                    patient.AddAppointement(appointment);
                }

                return patient;
            }


        
    }
}
