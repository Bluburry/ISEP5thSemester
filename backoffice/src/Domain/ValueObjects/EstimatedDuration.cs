using System;
using System.Collections.Generic;
using DDDSample1.Domain.Shared;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using Microsoft.VisualStudio.TextTemplating;

namespace DDDSample1.Domain.ValueObjects
{
	public class EstimatedDuration : ValueObject
	{

		public int Duration { get; set;}

		public EstimatedDuration(int duration)
		{
			if (duration < 5)
				throw new ArgumentException("Duration can't be less than 5 minutes.", nameof(duration));
			Duration = duration;
		}

		public EstimatedDuration(string duration)
		{
			if (!int.TryParse(duration, out int d))
				throw new ArgumentException("Couldn't parse estimated duration.", nameof(duration));
			Duration = d;
		}

		public EstimatedDuration(){
			//for ORM
		}

		override
		public string ToString()
		{ return Duration.ToString(); }

		override
		protected IEnumerable<object> GetAtomicValues()
		{
			return [Duration];
			// return new object[] { Duration };
		}
	}

}
