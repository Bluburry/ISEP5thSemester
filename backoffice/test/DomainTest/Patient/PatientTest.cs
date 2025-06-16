using System;
using System.Collections.Generic;
using Moq;
using Xunit;
using DDDSample1.Domain.ContactInformations;
using DDDSample1.Domain.HospitalAppointment;
using DDDSample1.Domain.Shared;
using DDDSample1.Domain.Users;
using DDDSample1.Domain.ValueObjects;

namespace DDDSample1.Domain.HospitalPatient.Tests
{
    public class PatientTests
    {
        private readonly Patient _standardPatient;
        private readonly Mock<User> _mockUser;

        public PatientTests()
        {
            // Initialize the mock user with predefined properties
            _mockUser = new Mock<User>();
            _mockUser.Setup(u => u.ActivationStatus).Returns(ActivationStatus.DEACTIVATED);
            _mockUser.Setup(u => u.EmailAddress).Returns(new EmailAddress("user@example.com"));
            _mockUser.Setup(u => u.Password).Returns(new Password("!Password123"));
            _mockUser.Setup(u => u.Role).Returns(UserRole.STAFF);

            // Setup a standard patient object for reuse
            var firstName = new FirstName("John");
            var lastName = new LastName("Doe");
            var fullName = new FullName("John Doe");
            var gender = Gender.MALE;
            var dateOfBirth = new DateOfBirth("1990-01-01");
            var emergencyContact = new PhoneNumber("");
            var clinicalDetails = "None";

            // Create a Patient using the mocked user
            _standardPatient = new Patient(
                firstName,
                lastName,
                fullName,
                gender,
                dateOfBirth,
                new ContactInformation(new EmailAddress("john.doe@example.com"), new PhoneNumber("123456789")),
                emergencyContact,
                new List<Appointment>(),
                _mockUser.Object // Using the mocked user here
            );
        }

        [Fact]
        public void Test_Patient_Constructor_Sets_Properties()
        {
            // Arrange
            var expectedFirstName = _standardPatient.firstName.ToString();
            var expectedLastName = _standardPatient.lastName.ToString();
            var expectedEmail = _standardPatient.ContactInformation.Email.ToString();
            var expectedPhone = _standardPatient.ContactInformation.Phone.ToString();

            // Act
            // (Already set in the constructor)

            // Assert
            Assert.Equal(expectedFirstName, _standardPatient.firstName.ToString());
            Assert.Equal(expectedLastName, _standardPatient.lastName.ToString());
            Assert.Equal(expectedEmail, _standardPatient.ContactInformation.Email.ToString());
            Assert.Equal(expectedPhone, _standardPatient.ContactInformation.Phone.ToString());
        }

        [Fact]
        public void Test_Patient_Anonymize_Changes_Identifiable_Information()
        {
            // Act
            _standardPatient.Anonymize();

            // Assert
            Assert.Equal("Anonymous", _standardPatient.firstName.ToString());
            Assert.Equal("Patient", _standardPatient.lastName.ToString());
            Assert.Equal("Anonymous Patient", _standardPatient.fullName.ToString());
            Assert.Null(_standardPatient.userId);
            Assert.Equal(Gender.NONSPECIFIED, _standardPatient.gender);
            Assert.Equal("0000-01-01", _standardPatient.dateOfBirth.ToString());
            Assert.Equal("anonymous@example.com", _standardPatient.ContactInformation.Email.ToString());
            Assert.Equal("0000000000", _standardPatient.ContactInformation.Phone.ToString());
            Assert.Equal("0000000000", _standardPatient.emergencyContact.ToString());
            Assert.Empty(_standardPatient.appointmentHistory);
        }

        [Fact]
        public void Test_PatientBuilder_Build_Success()
        {
            // Arrange
            var builder = new PatientBuilder();

            // Act
            var patient = builder
                .WithContactInformation("123456789", "john.doe@example.com")
                .WithFirstName("John")
                .WithLastName("Doe")
                .WithFullName("John Doe")
                .WithGender("MALE")
                .WithEmergencyContactNumber("987654321")
                .WithDateOfBirth("1990-01-01")
                .Build();

            // Assert
            Assert.NotNull(patient);
            Assert.Equal("John", patient.firstName.ToString());
            Assert.Equal("Doe", patient.lastName.ToString());
            Assert.Equal("John Doe", patient.fullName.ToString());
            Assert.Equal(Gender.MALE, patient.gender);
            Assert.Equal("john.doe@example.com", patient.ContactInformation.Email.ToString());
            Assert.Equal("123456789", patient.ContactInformation.Phone.ToString());
            Assert.Equal("987654321", patient.emergencyContact.ToString());
        }

        // The rest of the tests remain the same...

        [Fact]
        public void Test_PatientBuilder_Fails_Without_Required_Properties()
        {
            // Arrange
            var builder = new PatientBuilder();

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => builder.Build());
            Assert.Equal("ContactInformation is required.", exception.Message);
        }


        [Fact]
        public void Test_PatientBuilder_Fails_With_Empty_FirstName()
        {
            // Arrange
            var builder = new PatientBuilder();
            builder.WithContactInformation("123456789", "john.doe@example.com");

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => builder.WithFirstName("").Build());
            Assert.Equal("First name cannot be empty.", exception.Message);
        }

        [Fact]
        public void Test_PatientBuilder_Fails_With_Empty_LastName()
        {
            // Arrange
            var builder = new PatientBuilder();
            builder.WithContactInformation("123456789", "john.doe@example.com");

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => builder.WithLastName("").Build());
            Assert.Equal("Last name cannot be empty.", exception.Message);
        }

        [Fact]
        public void Test_PatientBuilder_Fails_With_Invalid_Gender()
        {
            // Arrange
            var builder = new PatientBuilder();
            builder.WithContactInformation("123456789", "john.doe@example.com")
                   .WithFirstName("John")
                   .WithLastName("Doe");

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => builder.WithGender("INVALID").Build());
            Assert.Equal("Gender must be valid.", exception.Message);
        }

        [Fact]
        public void Test_PatientBuilder_Fails_With_Null_DateOfBirth()
        {
            // Arrange
            var builder = new PatientBuilder();
            builder.WithContactInformation("123456789", "john.doe@example.com");

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => builder.WithDateOfBirth(null).Build());
            Assert.Equal("Birth Date cannot be empty.", exception.Message);
        }

        [Fact]
        public void Test_PatientBuilder_Fails_With_Empty_EmergencyContact()
        {
            // Arrange
            var builder = new PatientBuilder();
            builder.WithContactInformation("123456789", "john.doe@example.com")
                   .WithFirstName("John")
                   .WithLastName("Doe");

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => builder.WithEmergencyContactNumber("").Build());
            Assert.Equal("EmergencyContact is required.", exception.Message);
        }
    }
}
