using System;
using DDDSample1.Domain.Shared;
using Newtonsoft.Json;

namespace DDDSample1.Domain.Tokens
{
    public class TokenId : EntityId
    {
        [JsonConstructor]
        public TokenId(Guid value) : base(value)
        {
        }

        public TokenId(String value) : base(value)
        {
        }

        override
        protected  Object createFromString(String text){
            return text;
        }

        override
        public String AsString(){
            return base.ObjValue.ToString();
        }
        
       
        public Guid AsGuid(){
            return (Guid) base.ObjValue;
        }
    }
}