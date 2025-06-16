using System;
using System.Text.Json.Serialization;
using DDDSample1.Domain.Shared;

namespace DDDSample1.Domain.OperationPhases
{
	public class OperationPhaseID : EntityId
	{
		[JsonConstructor]
		public OperationPhaseID(object value) : base(value) { }

		[JsonConstructor]
		public OperationPhaseID(Guid value) : base(value) { }

		[JsonConstructor]
		public OperationPhaseID(String value) : base(value) { }

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

