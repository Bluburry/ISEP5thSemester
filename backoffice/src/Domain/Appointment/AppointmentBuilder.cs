using System;
using System.Collections.Generic;
using DDDSample1.Domain.AssignedStaffs;
using DDDSample1.Domain.ContactInformations;
using DDDSample1.Domain.HospitalPatient;
using DDDSample1.Domain.HospitalStaff;
using DDDSample1.Domain.OperationRequests;
using DDDSample1.Domain.OperationRooms;
using DDDSample1.Domain.Shared;
using DDDSample1.Domain.ValueObjects;
using Humanizer;

namespace DDDSample1.Domain.HospitalAppointment
{
    public class AppointmentBuilder
    {
        public DateAndTime _dateAndTime { get; set; }
        public AppointmentStatus _appoitmentStatus { get; set; }
        public string _reason { get; set; }
        public string _diagnosis { get; set; }
        public string _notes { get; set; }
        public LicenseNumber _staffId { get; set; }
        public List<AssignedStaff> _designedStaff { get; set; }
        public Patient _patient { get; set; }
        public MedicalRecordNumber _patientNumber { get; set; }
        public OperationRequest _request { get; set; }
        public OperationRoom _room { get; set; }

        // Existing builder methods...

        public AppointmentBuilder WithDateAndTime(string dateAndTime)
        {
            DateTime.TryParse(dateAndTime, out DateTime date);
            _dateAndTime = new DateAndTime(date);
            return this;
        }

        public AppointmentBuilder WithStatus(string status)
        {
            if (status.Equals("SCHEDULED")) { _appoitmentStatus = AppointmentStatus.SCHEDULED; }
            else if (status.Equals("ONGOING")) { _appoitmentStatus = AppointmentStatus.ONGOING; }
            else if (status.Equals("COMPLETED")) { _appoitmentStatus = AppointmentStatus.COMPLETED; }
            else if (status.Equals("CANCELLED")) { _appoitmentStatus = AppointmentStatus.CANCELLED; }
            return this;
        }

        public AppointmentBuilder WithReason(string reason)
        {
            _reason = reason;
            return this;
        }

        public AppointmentBuilder WithDiagnosis(string diagnosis)
        {
            _diagnosis = diagnosis;
            return this;
        }

        public AppointmentBuilder WithNotes(string notes)
        {
            _notes = notes;
            return this;
        }

        public AppointmentBuilder WithPatient(Patient patient)
        {
            _patient = patient;
            return this;
        }

        public AppointmentBuilder WithStaff(List<AssignedStaff> staff)
        {
            _designedStaff = staff;
            return this;
        }

        // New method to include OperationRequest
        public AppointmentBuilder WithRequest(OperationRequest request)
        {
            _request = request;
            return this;
        }

        // New method to include SurgeryRoom (string for room name)
        public AppointmentBuilder WithRoom(OperationRoom room)
        {
            if (room == null)
                throw new ArgumentException("Room cannot be null or empty.", nameof(room));

            _room = room;
            return this;
        }

        public Appointment Build()
        {
            if (_dateAndTime == null)
                throw new ArgumentException("Date and Time is required.");
            if (_patient == null)
                throw new ArgumentException("Patient is required.");
            //if (_designedStaff == null)
            //    throw new ArgumentException("Designed Staff is required.");


            var appointment = new Appointment(_dateAndTime, _appoitmentStatus, _designedStaff, _patient, _request, _room);
            
            return appointment;
        }
    }
}
