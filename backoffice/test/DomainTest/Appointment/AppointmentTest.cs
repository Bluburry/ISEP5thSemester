using System;
using Xunit;
using Moq;
using DDDSample1.Domain.HospitalAppointment;
using DDDSample1.Domain.HospitalPatient;
using DDDSample1.Domain.HospitalStaff;
using DDDSample1.Domain.ValueObjects;
using DDDSample1.Domain.Shared;
using DDDSample1.Domain.Users;
using DDDSample1.Domain.Specializations;
using DDDSample1.Domain.AssignedStaffs;

namespace DDDSample1.Domain.HospitalAppointment.Tests
{
    public class AppointmentTests
    {
        private readonly Mock<Staff> _mockStaff;
        private readonly Mock<Patient> _mockPatient;
        private readonly DateAndTime _mockDateAndTime = new DateAndTime(DateTime.Now);
        private readonly AppointmentStatus _mockStatus = AppointmentStatus.ONGOING;
        private readonly string _mockReason = "Routine check-up";
        private readonly string _mockDiagnosis = "Healthy";
        private readonly string _mockNotes = "No follow-up needed";
        Patient patient;

        public AppointmentTests()
        {   
            
            var patientBuilder = new PatientBuilder()
            .WithFirstName("Jane")
            .WithLastName("Doe")
            .WithFullName("Jane Doe")
            .WithGender("FEMALE")
            .WithDateOfBirth("1990-01-01")
            .WithContactInformation("+123456789", "jane.doe@example.com")
            .WithEmergencyContactNumber("+987654321")
            .WithMedicalRecordNumber();

      
            patient = patientBuilder.Build();
        }

        [Fact]
        public void Test_Appointment_Constructor_Success()
        {
            List<AssignedStaff> designedS = new List<AssignedStaff>();
            // Build the Appointment object
            var appointmentBuilder = new AppointmentBuilder()
                .WithDateAndTime("2024-10-25 10:30")
                .WithStatus("SCHEDULED")
                .WithReason("Routine check-up")
                .WithDiagnosis("N/A")
                .WithNotes("First appointment in 6 months")
                .WithPatient(patient);
            // Act
            var appointment = appointmentBuilder.Build();

            // Assert
            Assert.NotNull(appointment);
            Assert.Equal(patient.Id, appointment.patiendID);  // Verify that the patient is correctly assigned
            Assert.Equal(AppointmentStatus.SCHEDULED, appointment.appoitmentStatus);  // Ensure status is SCHEDULED
            Assert.Equal(new DateTime(2024, 10, 25, 10, 30, 0), appointment.dateAndTime.DateTime);  // Verify appointment date and time

        }


    }
}
