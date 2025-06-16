using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using DDDSample1.Domain.Shared;



    public class PhoneNumber : ValueObject
    {
        public string Value { get; set;}

        public PhoneNumber(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Phone number cannot be empty.", nameof(value));

            // Validate phone number format
            if (!Regex.IsMatch(value, @"^[\+\d\s\-\(\)]+$"))
                throw new ArgumentException("Phone number contains invalid characters.", nameof(value));

            Value = value;
        }

        public PhoneNumber()
        {
        }

        public override string ToString()
        {
            return Value;
        }

        public override bool Equals(object obj)
        {
            return obj is PhoneNumber phoneNumber && Value.Equals(phoneNumber.Value);
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

