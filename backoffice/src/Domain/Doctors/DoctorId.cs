using System;
using System.Text.Json.Serialization;
using DDDSample1.Domain.Shared;

namespace DDDSample1.Domain.Doctors
{
    public class DoctorId : EntityId
    {
           
    [JsonConstructor]
    public DoctorId(string value) : base(value)
    {
    }

    [JsonConstructor]
    public DoctorId(Guid value) : base(value)
    {
    }

    override
    protected Object createFromString(String text)
    {
        return text;
    }

    override
    public String AsString()
    {
        return base.ObjValue.ToString();
    }
    }
}