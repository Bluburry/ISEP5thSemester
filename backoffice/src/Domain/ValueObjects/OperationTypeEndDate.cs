using System;
using System.Collections.Generic;
using DDDSample1.Domain.Shared;

namespace DDDSample1.Domain.ValueObjects
{
	public class OperationTypeEndDate : ValueObject, IComparable<DateTime>
	{

		// https://learn.microsoft.com/en-us/dotnet/api/system.datetime.date?view=net-8.0
		public DateTime EndDate { get; set;}

		public OperationTypeEndDate(DateTime endDate)
		{
			// https://stackoverflow.com/questions/32849320/how-to-compare-a-given-date-from-today
			if (endDate <= DateTime.Today)
				throw new ArgumentException("End date can't be same day or older.", nameof(endDate));
			EndDate = endDate;
		}

		public OperationTypeEndDate(string date)
		{
			// NOTE: will need checking for string format if it ends up being used
			// = DateTime.Parse(date, new CultureInfo());
			if (!DateTime.TryParse(date, out DateTime endDate))
				throw new ArgumentException("Couldn't parse date from string.", nameof(date));
			if (endDate <= DateTime.Today)
				throw new ArgumentException("End date can't be same day or older.", nameof(date));
			EndDate = endDate;
		}

		public OperationTypeEndDate(){
			//for ORM
		}

		public override string ToString()
		{
			return EndDate.ToString("yyyy-MM-dd");
		}

		protected override IEnumerable<object> GetAtomicValues()
		{
			throw new NotImplementedException();
		}

		public int CompareTo(DateTime other)
		{
			return EndDate.CompareTo(other);
		}
	}
}