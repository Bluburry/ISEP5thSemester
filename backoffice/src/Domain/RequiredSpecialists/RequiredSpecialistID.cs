using System;
using System.Text.Json.Serialization;
using DDDSample1.Domain.Shared;

namespace DDDSample1.Domain.RequiredSpecialists
{
	public class RequiredSpecialistID : EntityId
	{
		[JsonConstructor]
		public RequiredSpecialistID(object value) : base(value) { }

		[JsonConstructor]
		public RequiredSpecialistID(Guid value) : base(value) { }

		[JsonConstructor]
		public RequiredSpecialistID(String value) : base(value) { }

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
