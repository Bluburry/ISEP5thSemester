using System;
using System.Collections.Generic;
using DDDSample1.Domain.Shared;

namespace DDDSample1.Domain.ValueObjects
{
	public class OperationTypeStartDate : ValueObject, IComparable<DateTime>
	{
		public DateTime StartDate { get; set;}

		public OperationTypeStartDate(DateTime startDate)
		{
			if (startDate <= DateTime.Today)
				throw new ArgumentException("End date can't be same day or older.", nameof(startDate));
			StartDate = startDate;
		}

		public OperationTypeStartDate(string date)
		{
			// = DateTime.Parse(date, new CultureInfo());
			if (!DateTime.TryParse(date, out DateTime startDate))
				throw new ArgumentException("Couldn't parse date from string.", nameof(date));
			if (startDate <= DateTime.Today)
				throw new ArgumentException("End date can't be same day or older.", nameof(date));
			StartDate = startDate;
		}

		protected OperationTypeStartDate(){
			//for ORM
		}

		public override string ToString()
		{
			return StartDate.ToString("yyyy-MM-dd");
		}

		protected override IEnumerable<object> GetAtomicValues()
		{
			return [StartDate];
			// return new object[] { StartDate };
		}

		public int CompareTo(DateTime other)
		{
			return StartDate.CompareTo(other);
		}
	}
}