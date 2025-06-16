using System;
using System.Collections.Generic;
using DDDSample1.Domain.Shared;

namespace DDDSample1.Domain.LoginAttemptTrackers
{
    public class AttemptCounter : ValueObject
    {
        // Counter property should be initialized with the provided number
        public int Counter { get; set;}

        // Constructor to initialize the Counter
        public AttemptCounter(int number)
        {
            // Validate the input if needed
            if (number < 0)
                throw new ArgumentOutOfRangeException(nameof(number), "Attempt counter cannot be negative.");

            Counter = number;
        }

        public AttemptCounter(){
            // for ORM
        }

        // Method to get the current value of the counter
        public int GetValue() => Counter;

        // Method to increment the counter
        public virtual void Increment()
        {
            Counter++;
        }

        public virtual void Reset()
        {
            Counter = 0;
        }

        // Override GetAtomicValues to return the Counter value
        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Counter; // Return the Counter value for equality checks
        }

        public virtual int Attempts()
        {
            return Counter;
        }
    }
}
