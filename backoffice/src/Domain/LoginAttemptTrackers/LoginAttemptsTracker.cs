using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DDDSample1.Domain.Shared;
using DDDSample1.Domain.Tokens;
using DDDSample1.Domain.Users;
using DDDSample1.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;


namespace DDDSample1.Domain.LoginAttemptTrackers
{
    public class LoginAttemptTracker : Entity<Username>
    {
        public AttemptCounter AttemptCounter { get; set;}
        

        public LoginAttemptTracker(AttemptCounter _attemptCounter, Username username)
        {
            this.Id = username;
            this.AttemptCounter = _attemptCounter;
        }

        public LoginAttemptTracker()
        {
        }

        public override string ToString()
        {
            return $"User: {Id}, COunter: {AttemptCounter}";
        }

        public override bool Equals(object obj)
        {
            return obj is LoginAttemptTracker tracker &&
                   Id.Equals(tracker.Id) &&
                   AttemptCounter.Equals(tracker.AttemptCounter);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, AttemptCounter);
        }

        public void IncrementAttemptCounter()
        {
            AttemptCounter.Increment();
        }

        public void AttemptCounterReset()
        {
            AttemptCounter.Reset();
        }

        public int LoginAttempts()
        {
            return AttemptCounter.Attempts();
        }

    }
}
