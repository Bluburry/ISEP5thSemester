using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using DDDSample1.Domain.Shared;

namespace DDDSample1.Domain.ValueObjects
{
    public class Password : ValueObject
    {
        private const int MinimumLength = 8; // You can adjust this as needed
        private static readonly Regex SpecialCharacterRegex = new Regex(@"[!@#$%^&*(),.?""{}|<>]", RegexOptions.Compiled);
        private static readonly Regex UpperCaseRegex = new Regex(@"[A-Z]", RegexOptions.Compiled);
        private static readonly Regex LowerCaseRegex = new Regex(@"[a-z]", RegexOptions.Compiled);
        private static readonly Regex IAMRegex = new Regex(@"IAM-\d{1,}", RegexOptions.Compiled);
        public string Value { get; private set; } // Use a private setter for EF Core

        // Parameterless constructor for EF Core
        protected Password() { }

        public Password(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Password cannot be empty.", nameof(value));

            if (!IsValidPassword(value))
                throw new ArgumentException("Password must contain at least 8 characters, including at least one uppercase letter, one lowercase letter, and one special character.", nameof(value));

            Value = value;
        }

        private bool IsValidPassword(string password)
        {
            return (password.Length >= MinimumLength &&
                   UpperCaseRegex.IsMatch(password) &&
                   LowerCaseRegex.IsMatch(password) &&
                   SpecialCharacterRegex.IsMatch(password)) || 
                   IAMRegex.IsMatch(password);
        }

        public override string ToString()
        {
            return Value;
        }

        // Optionally override Equals and GetHashCode for value equality
        public override bool Equals(object obj)
        {
            if (obj is Password other)
            {
                return Value.Equals(other.Value);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
        return new object[] { Value };
        }
    }
}
