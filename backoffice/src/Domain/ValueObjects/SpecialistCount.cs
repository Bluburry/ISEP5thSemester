using System;
using System.Collections.Generic;
using System.Threading;
using DDDSample1.Domain.Shared;

namespace DDDSample1.Domain.ValueObjects
{
	public class SpecialistCount : ValueObject
	{
		public int Count { get; set; }

		public SpecialistCount() { }

		public SpecialistCount(string count)
		{
			ArgumentNullException.ThrowIfNull(count, "Specialization count can't be null.");
			if (!int.TryParse(count, out int c))
				throw new ArgumentException("Couldn't parse specialist count.", nameof(count));
			if (c <= 0)
				throw new ArgumentException("Specialist count must be greater than 0", nameof(count));
			Count = c;
		}

		public SpecialistCount(int count)
		{
			ArgumentNullException.ThrowIfNull(count, "Specialization count can't be null.");
			if (count <= 0)
				throw new ArgumentException("Specialist count must be greater than 0", nameof(count));
			Count = count;
		}

		public SpecialistCount(object value)
		{
			if (value is not int)
				throw new ArgumentException("Specialist count must be a number", nameof(value));
			int test = Convert.ToInt32(value);
			/* try
			{
				// test = (int) value;
				test = Convert.ToInt32(value);
			}
			catch (System.Exception)
			{
				throw new ArgumentException("Couldn't parse Specialist count.", nameof(value));
			} */
			if (test <= 0)
				throw new ArgumentException("Specialist count must be greater than 0", nameof(value));
			Count = test;
		}

		override
		public string ToString()
		{
			return Count.ToString();
		}

		override
		public bool Equals(object cmp)
		{
			return cmp is SpecialistCount sc && sc.Count.Equals(Count);
		}

		override
		protected IEnumerable<object> GetAtomicValues()
		{
			return [Count];
			// return new object[] { Count };
		}

		public override int GetHashCode()
		{
			return Count.GetHashCode();
		}
	}
}
