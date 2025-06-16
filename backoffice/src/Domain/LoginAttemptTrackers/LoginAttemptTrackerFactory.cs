using DDDSample1.Domain.Shared;
using DDDSample1.Domain.ValueObjects;
using DDDSample1.Domain.Users;

namespace DDDSample1.Domain.LoginAttemptTrackers
{
    public class LoginAttemptTrackerFactory
    {
        // Method to create a new LoginAttemptTracker with initial counter value
        public static LoginAttemptTracker Create(Username username, int initialAttemptCount = 0)
        {
            // Create an AttemptCounter with the provided initial count
            var attemptCounter = new AttemptCounter(initialAttemptCount);

            // Return a new LoginAttemptTracker with the username and counter
            return new LoginAttemptTracker(attemptCounter, username);
        }
    }
}
