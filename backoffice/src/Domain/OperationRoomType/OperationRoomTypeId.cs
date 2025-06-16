using System;
using DDDSample1.Domain.Shared;
using Newtonsoft.Json;

namespace DDDSample1.Domain.OperationRoomTypes
{
    public class OperationRoomTypeId : EntityId
    {
        [JsonConstructor]
        public OperationRoomTypeId(string value) : base(value)
        {
        }

        protected override object createFromString(string text)
        {
            return text;
        }

        public override string AsString()
        {
            return (string) Value;
        }
    }
}