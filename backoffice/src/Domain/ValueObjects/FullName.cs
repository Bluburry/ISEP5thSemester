using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;
using DDDSample1.Domain.Shared;

namespace DDDSample1.Domain.ValueObjects
{
    public class FullName :  ValueObject
    {
        public string fullname { get; set;}

        public FullName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("First name cannot be empty.", nameof(value));

        
            fullname = value;
        }

        protected FullName()
        {
        }

        public override string ToString()
        {
            return fullname;
        }

        public override bool Equals(object obj)
        {
            return obj is FullName other && fullname.Equals(other.fullname, StringComparison.OrdinalIgnoreCase);
        }
        public override int GetHashCode()
        {
            return fullname.GetHashCode();
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            return new object[] { fullname };
        }
    }
}
