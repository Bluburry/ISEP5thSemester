
using System;
using DDDSample1.Domain.Shared;
using Newtonsoft.Json;

namespace DDDSample1.Domain.Specializations
{
	public class SpecializationCode : EntityId
	{
		[JsonConstructor]
		public SpecializationCode(string value) : base(value) { }

		/* public SpecializationCode(Guid value) : base(value) { } */

		override
		protected object createFromString(string text)
		{
			return text;
		}

		override
		public string AsString()
		{
			return base.ObjValue.ToString();
		}

		/* override
		public string ToString()
		{ return base.ObjValue.ToString(); } */
	}
}
