using System;
using System.Collections.Generic;
using DDDSample1.Domain.Shared;

namespace DDDSample1.Domain.ValueObjects{
	public class EntryVersion : ValueObject
	{
		public int Value { get; }

		public EntryVersion(int value)
		{
			if (value < 0)
				throw new ArgumentException("Version number must be non-negative");

			Value = value;
		}

		public EntryVersion(){
			//for ORM
		}

		// Override Equals and GetHashCode for value equality
		public override bool Equals(object obj)
		{
			return Equals(obj as EntryVersion);
		}

		public bool Equals(EntryVersion other)
		{
			if (other == null) return false;
			return Value == other.Value;
		}

		public override int GetHashCode()
		{
			return Value.GetHashCode();
		}

		public override string ToString()
		{
			return Value.ToString();
		}

		// Method to compare versions
		public bool IsGreaterThan(EntryVersion other)
		{
			if (other == null)
				throw new ArgumentNullException(nameof(other));

			return Value > other.Value;
		}

        protected override IEnumerable<object> GetAtomicValues()
        {
			return new object[] { Value };
        }
    }
}