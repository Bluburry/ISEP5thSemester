
using System;
using Org.BouncyCastle.Crypto.Agreement;

namespace DDDSample1.Domain.Specializations
{
	public class SpecializationBuilder
	{
		private string _code;
		private string _name;
		private string _description;

		public SpecializationBuilder WithSpecializationCode(string code)
		{
			if (string.IsNullOrEmpty(code))
			{
				_code = "";
				return this;
			}
			_code = code;
			return this;
		}

		public SpecializationBuilder WithSpecializationName(string name)
		{
			_name = name;
			return this;
		}

		public SpecializationBuilder WithSpecializationDescription(string description)
		{
			if (string.IsNullOrEmpty(description))
			{
				_description = "";
				return this;
			}
			_description = description;
			return this;
		}

		public Specialization Build()
		{
			if (string.IsNullOrEmpty(_name))
				throw new ArgumentException("Specialization Name cannot be null or empty");

			if (!string.IsNullOrEmpty(_code) && !string.IsNullOrEmpty(_description))
				return new Specialization(_name, _description, _code);
			else if (!string.IsNullOrEmpty(_code))
				return new Specialization(_name, "", _code);
			else if (!string.IsNullOrEmpty(_code))
				return new Specialization(_name, _description);
			else
				return new Specialization(_name);

		}
	}
}