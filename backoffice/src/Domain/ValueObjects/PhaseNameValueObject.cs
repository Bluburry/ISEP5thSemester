using System;
using System.Collections.Generic;
using DDDSample1.Domain.Shared;

namespace DDDSample1.Domain.ValueObjects
{
	public class PhaseNameValueObject : ValueObject
	{
		public PhaseName Name { get; }

		public PhaseNameValueObject(PhaseName name) { Name = name; }

		public PhaseNameValueObject(string name) 
		{
			if (!Enum.TryParse(name, true, out PhaseName pn))
				throw new ArgumentException("Couldn't parse phase name.", nameof(name));
			Name = pn;
		}

		override
		public string ToString()
		{
			return Name.ToString();
		}

		override
		public bool Equals(object cmp)
		{
			return cmp is PhaseNameValueObject pn && pn.Name.Equals(Name);
		}

		override
		public int GetHashCode()
		{ return Name.GetHashCode(); }

		override
		protected IEnumerable<object> GetAtomicValues()
		{
			return [Name];
			// return new object[] {Name};
		}
	}
}
