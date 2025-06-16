using System;
using System.Collections.Generic;
using DDDSample1.Domain.Shared;

namespace DDDSample1.Domain.ValueObjects
{
	public class PhaseDuration : ValueObject
	{
		public int Duration { get; set;}

		public PhaseDuration(int duration)
		{
			if (duration < 5)
				throw new ArgumentException("Phase duration can't be less than 5 minutes.", nameof(duration));
			Duration = duration;
		}

		public PhaseDuration(string duration)
		{
			if (!int.TryParse(duration, out int d))
				throw new ArgumentException("Couldn't parse phase duration.", nameof(duration));
			if (d < 5)
				throw new ArgumentException("Phase duration can't be less than 5 minutes.", nameof(duration));
			Duration = d;
		}

		public PhaseDuration()
		{
			//for ORM
		}

		public override string ToString()
		{
			return Duration.ToString();
		}

		protected override IEnumerable<object> GetAtomicValues()
		{
			return [Duration];
		}
	}
}