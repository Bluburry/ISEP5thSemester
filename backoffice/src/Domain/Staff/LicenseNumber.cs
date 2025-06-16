using System;
using DDDSample1.Domain.Shared;
using Newtonsoft.Json;

namespace DDDSample1.Domain.HospitalStaff
{
    public class LicenseNumber : EntityId
    {
        [JsonConstructor]
        public LicenseNumber(string value) : base(value)
        {
        }

        override
        protected Object createFromString(String text)
        {
            // Directly return the email string without GUID conversion
            return text;
        }
        
        public override string ToString()
        {
            return Value;
        }

        override
        public String AsString()
        {
            return base.ObjValue.ToString();
        }
    }
}