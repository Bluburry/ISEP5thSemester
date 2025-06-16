using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Text.RegularExpressions;
using DDDSample1.Domain.Shared;


    public class EmailAddress : ValueObject
    {
        private static readonly Regex EmailRegex = new Regex(
            @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase
        );

        public string Value { get; set;}

        protected EmailAddress(){ }

        public EmailAddress(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Email address cannot be empty.", nameof(value));

            if (!IsValidEmail(value))
                throw new ArgumentException("Invalid email address format.", nameof(value));

            Value = value;
        }

        private bool IsValidEmail(string email)
        {
            return EmailRegex.IsMatch(email);
        }

        public override string ToString()
        {
            return Value;
        }

        // Optionally override Equals and GetHashCode for value equality
        public override bool Equals(object obj)
        {
            if (obj is EmailAddress other)
            {
                return Value.Equals(other.Value, StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode(StringComparison.OrdinalIgnoreCase);
        }

    protected override IEnumerable<object> GetAtomicValues()
    {
    return new object[] { Value };
    }

}

