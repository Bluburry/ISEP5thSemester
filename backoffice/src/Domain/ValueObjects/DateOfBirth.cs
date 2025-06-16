using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace DDDSample1.Domain.Shared
{
    public class DateOfBirth : ValueObject
    {
        public string dateOfBirth { get; set;}

        public DateOfBirth(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Birth Date cannot be empty.", nameof(value));

            if (!Regex.IsMatch(value, @"^\d{4}-(0[1-9]|1[0-2])-(0[1-9]|[12]\d|3[01])$"))
                throw new ArgumentException("Birth Date can only contain numeric characters and special ones(yyyy-MM-dd).", nameof(value));

            this.dateOfBirth = value;
        }

        protected DateOfBirth()
        {
        }

        public override string ToString()
        {
            return this.dateOfBirth;
        }

        public override bool Equals(object obj)
        {
            return obj is DateOfBirth dateOfBirth && dateOfBirth.Equals(dateOfBirth.dateOfBirth);
        }

        public override int GetHashCode()
        {
            return this.dateOfBirth.GetHashCode();
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            return new object[] { this.dateOfBirth };
        }
    }
}