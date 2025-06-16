using System;
using DDDSample1.Domain.Shared;
using Newtonsoft.Json;

namespace DDDSample1.Domain.Users
{
    public class Username : EntityId
{
    [JsonConstructor]
    public Username(string value) : base(value)
    {
    }

    override
    protected Object createFromString(String text)
    {
        // Directly return the email string without GUID conversion
        return text;
    }

    override
    public String AsString()
    {
        return base.ObjValue.ToString();
    }

        public static implicit operator Username(string v)
        {
            throw new NotImplementedException();
        }
    }

}