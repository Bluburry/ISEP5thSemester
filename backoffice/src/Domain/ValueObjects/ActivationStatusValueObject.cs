using System;
using System.Collections.Generic;
using DDDSample1.Domain.Shared;

namespace DDDSample1.Domain.ValueObjects
{
    public class ActivationStatusValueObject : ValueObject
    {
        public ActivationStatus Value { get; }

        protected ActivationStatusValueObject(ActivationStatus value)
        {
            Value = value;
        }

        public ActivationStatusValueObject()
        {
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        // Optionally override Equals and GetHashCode for value equality
        public override bool Equals(object obj)
        {
            if (obj is ActivationStatusValueObject other)
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
