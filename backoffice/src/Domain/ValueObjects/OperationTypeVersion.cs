using System;
using System.Collections.Generic;
using DDDSample1.Domain.Shared;

namespace DDDSample1.Domain.ValueObjects
{
	public class OperationTypeVersion : ValueObject, IComparable<OperationTypeVersion>
	{
		public int Version { get; }

		public OperationTypeVersion(int version)
		{
			if (version < 0)
				throw new ArgumentException("Operation type version must be 0 or greater", nameof(version));
			Version = version;
		}

		public OperationTypeVersion(string version)
		{
			if (!int.TryParse(version, out int v))
				throw new ArgumentException("Couldn't parse operation type version.", nameof(version));
			Version = v;
		}

		public override bool Equals(object obj)
		{
			return (obj is OperationTypeVersion v && v.Version.Equals(Version)) ||
					(obj is int i && i.Equals(Version)); // probably unnecessary ?
		}

		public override string ToString()
		{
			return Version.ToString();
		}

		protected override IEnumerable<object> GetAtomicValues()
		{
			return [Version];
		}

		public override int GetHashCode()
		{
			return Version.GetHashCode();
		}

		public int CompareTo(OperationTypeVersion other)
		{
			return other.Version.CompareTo(Version);
		}
	}
}