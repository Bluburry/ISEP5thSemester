using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using DDDSample1.Domain.Shared;

namespace DDDSample1.Domain.ValueObjects
{
    public class LastName : ValueObject
    {
        public string lastName { get; set;}

        public LastName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Last name cannot be empty.");

            if (!Regex.IsMatch(value, @"^[a-zA-Z]+$"))
                throw new ArgumentException("Last name can only contain alphabetic characters.");

            lastName = value;
        }

        public LastName()
        {
        }

        public override string ToString()
        {
            return lastName;
        }

        public override bool Equals(object obj)
        {
            return obj is LastName lastName && lastName.Equals(lastName.lastName);
        }

        public override int GetHashCode()
        {
            return lastName.GetHashCode();
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
        return new object[] { lastName };
        }
    }
}
