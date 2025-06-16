using System;
using Newtonsoft.Json;
// using System.Text.Json.Serialization;
using DDDSample1.Domain.Shared;

namespace DDDSample1.Domain.OperationTypes
{
	public class OperationTypeID : EntityId
	{
		[JsonConstructor]
		public OperationTypeID(object value) : base(value) { }

		[JsonConstructor]
		public OperationTypeID(Guid value) : base(value) { }

		[JsonConstructor]
		public OperationTypeID(String value) : base(value) { }

		override
		public string AsString()
		{
			return base.ObjValue.ToString();
		}

		override
		protected Object createFromString(string text)
		{
			return new Guid(text);
		}
	}
}