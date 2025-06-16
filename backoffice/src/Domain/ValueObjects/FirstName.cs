using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace DDDSample1.Domain.Shared
{
    public class FirstName : ValueObject
    {
        public string firstName { get; set;}

        public FirstName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("First name cannot be empty.");

            if (!Regex.IsMatch(value, @"^[a-zA-Z]+$"))
                throw new ArgumentException("First name can only contain alphabetic characters.");

            firstName = value;
        }

        protected FirstName()
        {
        }

        public override string ToString()
        {
            return firstName;
        }

        public override bool Equals(object obj)
        {
            return obj is FirstName firstName && firstName.Equals(firstName.firstName);
        }

        public override int GetHashCode()
        {
            return firstName.GetHashCode();
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            return new object[] { firstName };
        }
    }
}
