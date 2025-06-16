using System.Collections.Generic;
using DDDSample1.Domain.Shared;

namespace DDDSample1.Domain.ValueObjects
{
	public class OperationTypeName : ValueObject
	{
		public string OperationName { get; set;}

		public OperationTypeName(string name) { OperationName = name; }

		public override bool Equals(object other)
		{
			return (other is OperationTypeName on && on.OperationName.Equals(OperationName)) ||
				(other is string name && name.Equals(OperationName));
		}

		private OperationTypeName(){
			//for ORM
		}

		override
		public string ToString() { return OperationName.ToString(); }

		protected override IEnumerable<object> GetAtomicValues()
		{
			return [OperationName];
		}

		public override int GetHashCode()
		{
			return OperationName.GetHashCode();
		}
	}
}