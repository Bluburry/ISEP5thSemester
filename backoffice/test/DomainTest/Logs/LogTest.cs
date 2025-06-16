using System;
using DDDSample1.Domain.ValueObjects;
using Xunit;

namespace DDDSample1.Domain.Logs.Tests
{
    public class LogsTest
    {
        [Fact]
        public void Test_Log_Creation_Success()
        {
            // Arrange
            var loggedDate = new DateAndTime(DateTime.Now);
            var loggedInformation = "User John Doe was created.";
            var loggedType = ObjectLoggedType.STAFF;
            var logId = "12345";

            // Act
            var log = new Log(loggedDate, loggedInformation, loggedType, logId);

            // Assert
            Assert.NotNull(log);
            Assert.Equal(loggedDate, log.LoggedDate);
            Assert.Equal(loggedInformation, log.LoggedInformation);
            Assert.Equal(loggedType, log.loggedType);
            Assert.Equal(logId, log.LoggedId);
        }

        [Fact]
        public void Test_Log_ToDto_Success()
        {
            // Arrange
            var loggedDate = new DateAndTime(DateTime.Now);
            var loggedInformation = "User John Doe was created.";
            var loggedType = ObjectLoggedType.STAFF;
            var logId = "12345";
            var log = new Log(loggedDate, loggedInformation, loggedType, logId);

            // Act
            var logDto = log.toDto();

            // Assert
            Assert.NotNull(logDto);
            Assert.Equal(loggedInformation, logDto.LoggedInformation);
            Assert.Equal(loggedType.ToString(), logDto.LoggedType);
            Assert.Equal(loggedDate.ToString(), logDto.LoggedDate);
            Assert.Equal(logId, logDto.LoggedId);
        }

        [Fact]
        public void Test_LogsBuilder_Build_Success()
        {
            // Arrange
            var loggedInformation = "Patient Jane Doe was updated.";
            var objectType = ObjectLoggedType.PATIENT.ToString();
            var id = "98765";
            var builder = new LogsBuilder();

            // Act
            var log = builder
                .WithInformation(loggedInformation)
                .WithObjectType(objectType)
                .WithDateAndTime()  // Use the default date and time
                .WithID(id)
                .Build();

            // Assert
            Assert.NotNull(log);
            Assert.Equal(loggedInformation, log.LoggedInformation);
            Assert.Equal(ObjectLoggedType.PATIENT, log.loggedType);
            Assert.Equal(id, log.LoggedId);
            Assert.NotNull(log.LoggedDate); // Ensure date is set
        }

        [Fact]
        public void Test_LogsBuilder_MissingInformation_ThrowsException()
        {
            // Arrange
            var builder = new LogsBuilder();

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                builder
                    .WithObjectType(ObjectLoggedType.STAFF.ToString())
                    .WithDateAndTime()
                    .WithID("12345")
                    .Build()
            );

            Assert.Equal("Information is required to log.", exception.Message);
        }

        [Fact]
        public void Test_LogsBuilder_MissingObjectType_ThrowsException()
        {
            // Arrange
            var builder = new LogsBuilder();

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                builder
                    .WithInformation("Sample log")
                    .WithDateAndTime(null)
                    .WithID("12345")
                    .Build()
            );

        }
    }
}
