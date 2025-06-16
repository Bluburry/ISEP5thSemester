using System;
using DDDSample1.Domain.Shared;
using DDDSample1.Domain.Users;
using DDDSample1.Domain.ValueObjects;

namespace DDDSample1.Domain.Tokens
{
    public class TokenFactory
    {
        // Method to create a new Token
        public Token CreateToken(string expirationDate, User user)
        {
            var tokenId = new TokenId(Guid.NewGuid());

            DateTime.TryParse(expirationDate, out DateTime date);

            return new Token(tokenId, date, user, TokenType.GENERAL_ACCESS);
        }

        public Token CreateToken(DateTime expirationDate, User user)
        {
            var tokenId = new TokenId(Guid.NewGuid());

            return new Token(tokenId, expirationDate, user, TokenType.GENERAL_ACCESS);
        }

        public Token createPasswordActivationToken(DateTime expirationDate, User user)
        {
            var tokenId = new TokenId(Guid.NewGuid());

            return new Token(tokenId, expirationDate, user, TokenType.VERIFICATION_TOKEN);
        }

        public Token createPasswordResetToken(DateTime expirationDate, User user)
        {
            var tokenId = new TokenId(Guid.NewGuid());

            return new Token(tokenId, expirationDate, user, TokenType.PASSWORD_RESET_TOKEN);
        }

        public Token createUpdateConfirmationToken(DateTime expirationDate, User user)
        {
            var tokenId = new TokenId(Guid.NewGuid());

            return new Token(tokenId, expirationDate, user, TokenType.UPDATE_CONFIRMATION);
        }

        public Token createDeletionConfirmationToken(DateTime expirationDate, User user)
        {
            var tokenId = new TokenId(Guid.NewGuid());

            return new Token(tokenId, expirationDate, user, TokenType.DELETION_TOKEN);
        }

        public Token CreateLoginAuthToken(DateTime expirationDate, User user, UserRole role)
        {


            if(role == UserRole.STAFF){
                return new Token(expirationDate, user, TokenType.STAFF_AUTH_TOKEN);
            }else if(role == UserRole.PATIENT){
                return new Token(expirationDate, user, TokenType.PATIENT_AUTH_TOKEN);
            }else{
                return new Token(expirationDate, user, TokenType.ADMIN_AUTH_TOKEN);
            }

            
        }
    }
}
