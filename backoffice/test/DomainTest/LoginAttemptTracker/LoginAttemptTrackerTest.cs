using System;
using Xunit;
using Moq;
using DDDSample1.Domain.LoginAttemptTrackers;
using DDDSample1.Domain.Users;
using DDDSample1.Domain.ValueObjects;

namespace DDDSample1.Domain.Logs.Tests
{
    public class LoginAttemptTrackerTest
    {
        private readonly Mock<AttemptCounter> _mockAttemptCounter;
        private readonly Username _username;
        private int _attemptsCount;

        public LoginAttemptTrackerTest()
        {
            _mockAttemptCounter = new Mock<AttemptCounter>();
            _username = new Username("user@example.com");
            _attemptsCount = 0;

            // Setup the mock to return the current count
            _mockAttemptCounter.Setup(ac => ac.Attempts()).Returns(() => _attemptsCount);
            _mockAttemptCounter.Setup(ac => ac.Increment()).Callback(() => _attemptsCount++);
        }

        [Fact]
        public void Constructor_ShouldInitializePropertiesCorrectly()
        {
            // Arrange & Act
            var tracker = new LoginAttemptTracker(_mockAttemptCounter.Object, _username);

            // Assert
            Assert.Equal(_username, tracker.Id);
            Assert.Equal(_mockAttemptCounter.Object, tracker.AttemptCounter);
        }

        [Fact]
        public void IncrementAttemptCounter_ShouldCallIncrementOnAttemptCounter()
        {
            // Arrange
            var tracker = new LoginAttemptTracker(_mockAttemptCounter.Object, _username);

            // Act
            tracker.IncrementAttemptCounter();

            // Assert
            _mockAttemptCounter.Verify(ac => ac.Increment(), Times.Once);
        }

        [Fact]
        public void AttemptCounterReset_ShouldCallResetOnAttemptCounter()
        {
            // Arrange
            var tracker = new LoginAttemptTracker(_mockAttemptCounter.Object, _username);

            // Act
            tracker.AttemptCounterReset();

            // Assert
            _mockAttemptCounter.Verify(ac => ac.Reset(), Times.Once);
        }

        [Fact]
        public void IncrementAttemptCounter_ShouldIncreaseAttempts()
        {
            // Arrange
            var tracker = new LoginAttemptTracker(_mockAttemptCounter.Object, _username);

            // Act
            tracker.IncrementAttemptCounter(); // Increment 1
            tracker.IncrementAttemptCounter(); // Increment 2
            tracker.IncrementAttemptCounter(); // Increment 3

            // Assert
            var result = tracker.LoginAttempts(); // Should return 3
            Assert.Equal(3, result);
        }


        [Fact]
        public void Equals_ShouldReturnFalse_WhenObjectsAreNotEqual()
        {
            // Arrange
            var tracker1 = new LoginAttemptTracker(_mockAttemptCounter.Object, _username);
            var tracker2 = new LoginAttemptTracker(_mockAttemptCounter.Object, new Username("otheruser@example.com"));

            // Act & Assert
            Assert.False(tracker1.Equals(tracker2));
        }

        [Fact]
        public void ToString_ShouldReturnCorrectStringRepresentation()
        {
            // Arrange
            var tracker = new LoginAttemptTracker(_mockAttemptCounter.Object, _username);
            _mockAttemptCounter.Setup(ac => ac.ToString()).Returns("3");

            // Act
            var result = tracker.ToString();

            // Assert
            Assert.Equal($"User: {_username}, COunter: 3", result);
        }
    }
}
