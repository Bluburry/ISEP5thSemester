using System;

namespace DDDSample1.Domain.ValueObjects
{
    public class DateAndTime
    {
        public DateTime DateTime { get; private set; }

        public DateAndTime(DateTime dateTime)
        {
            if (dateTime.Equals(null))
                throw new ArgumentException("Date cannot be null.");

            this.DateTime = dateTime;
        }

        public bool IsExpired()
        {
            return DateTime.Now > DateTime;
        }

        public override string ToString()
        {
            return DateTime.ToString("yyyy-MM-dd HH:mm");
        }

        // Optional: equality override methods to compare ExpirationDate objects
        public override bool Equals(object obj)
        {
            if (obj is DateAndTime other)
            {
                return this.DateTime == other.DateTime;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return DateTime.GetHashCode();
        }
    }
}
