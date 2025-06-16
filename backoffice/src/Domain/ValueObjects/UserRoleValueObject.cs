using System;
using System.Collections.Generic;
using DDDSample1.Domain.Shared;
using DDDSample1.Domain.ValueObjects;

namespace DDDSample1.Domain.ValueObjects;

public class UserRoleValueObject : ValueObject
{
    public UserRole Value { get; }

    public UserRoleValueObject(UserRole value)
    {
        Value = value;
    }

    public override string ToString()
    {
        return Value.ToString();
    }

    // Optionally override Equals and GetHashCode for value equality
    public override bool Equals(object obj)
    {
        if (obj is UserRoleValueObject other)
        {
            return Value.Equals(other.Value);
        }
        return false;
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    protected override IEnumerable<object> GetAtomicValues()
    {
    return new object[] { Value };
    }
}
