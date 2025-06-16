using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using DDDSample1.Domain.Shared;

namespace DDDSample1.Domain.OperationRoomTypes
{
    public class OperationRoomTypeDescription : ValueObject
    {
        public string description { get; set;}

        public OperationRoomTypeDescription(string value)
        {
            if (string.IsNullOrEmpty(value))
                description = "";
            else if(value.Length <= 100)
                description = value;
            else
                throw new BusinessRuleValidationException("Operation room type description when provided cannot be longer than 100 characters.");
        }

        protected OperationRoomTypeDescription()
        {
        }

        public override string ToString()
        {
            return description;
        }

        public override bool Equals(object obj)
        {
            return obj is FirstName firstName && firstName.Equals(firstName.firstName);
        }

        public override int GetHashCode()
        {
            return description.GetHashCode();
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            return new object[] { description };
        }
    }
}
