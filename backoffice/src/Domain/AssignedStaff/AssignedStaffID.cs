using System;
using System.Text.Json.Serialization;
using DDDSample1.Domain.Shared;

namespace DDDSample1.Domain.AssignedStaffs
{
	public class AssignedStaffID : EntityId
	{
		[JsonConstructor]
		public AssignedStaffID(object value) : base(value) { }

		[JsonConstructor]
		public AssignedStaffID(Guid value) : base(value) { }

		[JsonConstructor]
		public AssignedStaffID(String value) : base(value) { }

		override
		public string AsString()
		{
			return base.ObjValue.ToString();
		}

		override
		protected object createFromString(string text)
		{
			return new Guid(text);
		}
	}
}
