using System;

namespace DDDSample1.Domain.Tokens
{
    public class TokenDto
    {
        public string TokenId { get; set; }
        public string TokenValue { get; set; }
        public string ExpirationDate { get; set; }
        public string UserId { get; set; }

        public TokenDto(string tokenId, string tokenValue, string expirationDate, string userId)
        {
            this.TokenId = tokenId;
            this.TokenValue = tokenValue;
            this.ExpirationDate = expirationDate;
            this.UserId = userId;
        }

        public TokenDto() { }
    }
}
