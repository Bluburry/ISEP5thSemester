using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using DDDSample1.Domain.Shared;

namespace DDDSample1.Domain.OperationRoomTypes
{
    public class OperationRoomName : ValueObject
    {
        public string Value { get; set;}

        public OperationRoomName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("First name cannot be empty.");

            if (!Regex.IsMatch(value, @"^[a-zA-Z]+$"))
                throw new ArgumentException("First name can only contain alphabetic characters.");

            Value = value;
        }

        protected OperationRoomName()
        {
        }

        public override string ToString()
        {
            return Value;
        }

        public override bool Equals(object obj)
        {
            return obj is FirstName firstName && firstName.Equals(firstName.firstName);
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
