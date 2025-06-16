using System;
using System.Collections.Generic;
using Xunit;
using Moq;
using DDDSample1.Domain.HospitalStaff;
using DDDSample1.Domain.ValueObjects;
using DDDSample1.Domain.Users;
using DDDSample1.Domain.ContactInformations;
using DDDSample1.Domain.Specializations;

namespace DDDSample1.Domain.HospitalStaff.Tests
{
    public class StaffBuilderTests
    {
        [Fact]
        public void Test_StaffBuilder_Build_Success()
        {
            // Arrange
            var userMock = new Mock<User>();
            var builder = new StaffBuilder();

            // Act
            var staff = builder
                .WithLicenseNumber("MRN123456")
                .WithContactInformation("123456789", "john.doe@hospital.com")
                .WithFirstName("John")
                .WithLastNAme("Doe")
                .WithFullName("John Doe")
                .WithSpecialization(new Specialization("General Medicine", ""))
                .WithUser(userMock.Object)
                .WithAvailabilitySlots(new List<string> { "Monday", "Wednesday" })
                .Build();

            // Assert
            Assert.NotNull(staff);
            Assert.Equal("John Doe", staff.FullName.ToString());
            Assert.Equal("MRN123456", staff.Id.AsString());
        }

        [Fact]
        public void Test_StaffBuilder_Fails_Without_LicenseNumber()
        {
            // Arrange
            var builder = new StaffBuilder();

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => builder
                .WithContactInformation("123456789", "john.doe@hospital.com")
                .WithFullName("John Doe")
                .WithSpecialization(new Specialization("General Medicine", ""))
                .WithUser(new Mock<User>().Object)
                .Build());

            Assert.Equal("LicenseNumber is required.", exception.Message);
        }

        [Fact]
        public void Test_StaffBuilder_Fails_Without_ContactInformation()
        {
            // Arrange
            var builder = new StaffBuilder();

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => builder
                .WithLicenseNumber("A123456")
                .WithFullName("John Doe")
                .WithSpecialization(new Specialization("General Medicine", ""))
                .WithUser(new Mock<User>().Object)
                .Build());

            Assert.Equal("ContactInformation is required.", exception.Message);
        }

        // More tests to cover other edge cases, such as missing full name, user, specialization, etc.
    }
}
