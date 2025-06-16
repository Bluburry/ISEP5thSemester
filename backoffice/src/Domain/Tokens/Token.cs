using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using DDDSample1.Domain.Shared;
using DDDSample1.Domain.Users;
using DDDSample1.Domain.ValueObjects;
using Newtonsoft.Json;

namespace DDDSample1.Domain.Tokens
{
    public class Token : Entity<TokenId>, IAggregateRoot
    {
        public DateAndTime ExpirationDate { get; private set; }
        public Username UserId {get; set;}
        public User TheUser { get; private set; }
        public TokenType TokenValue { get; private set; }

        private Token() {}

        public Token(DateTime expirationDate, User user, TokenType type)
        {
            this.ExpirationDate = new DateAndTime(expirationDate);
            this.TheUser = user;
            this.UserId = user.Id;
            this.TokenValue = type;
            this.Id = new TokenId(Encrypt());
        }

        public Token(TokenId id, DateTime expirationDate, User user, TokenType type)
        {
            this.Id = id;
            this.ExpirationDate = new DateAndTime(expirationDate);
            this.TheUser = user;
            this.UserId = user.Id;
            this.TokenValue = type;
        }

        // Method to check if the token is expired
        public bool IsExpired()
        {
            return ExpirationDate.IsExpired();
        }

        // Method to check if a token matches a given value
        public bool MatchesTokenValue(string value)
        {
            Enum.TryParse(value, true, out TokenType tokenCheck);
            return TokenValue == tokenCheck;
        }

        // Method to convert Token to TokenDto
        public TokenDto ToDto()
        {
        // Check if the ID is valid
            if (this.Id == null)
            {
                throw new InvalidOperationException("Token ID cannot be null.");
            }

            // Check if the TokenValue is valid
            if (this.TokenValue == null)
            {
                throw new InvalidOperationException("Token value cannot be null.");
            }

            // Check if the ExpirationDate is valid
            if (this.ExpirationDate == default)
            {
                throw new InvalidOperationException("Expiration date is not set.");
            }

            // Check if TheUser's ID is valid
            if (this.UserId == null)
            {
                throw new InvalidOperationException("User ID cannot be null.");
            }

            // Create and return the DTO if all checks pass
            return new TokenDto(
                this.Id.AsString(),
                this.TokenValue.ToString(),
                this.ExpirationDate.ToString(),
                this.UserId.AsString()
            );
        }


        // Overriding ToString for easier logging or display
        public override string ToString()
        {
            return $"Token: {TokenValue}, Expiration: {ExpirationDate}, User: {TheUser.Id}";
        }

        // Example token value generation method (if needed)
        private string GenerateTokenValue()
        {
            return Guid.NewGuid().ToString(); // Or your custom logic to generate a token value
        }

        public string Encrypt()
        {
            string publicKeyPem = File.ReadAllText("../../RSAcertificates/public.pem");
            RSA publicKey = RSA.Create();
            publicKey.ImportFromPem(publicKeyPem.ToCharArray());

            var dataToEncrypt = new
            {
                UserId = this.UserId.AsString(),
                TokenValue = this.TokenValue.ToString(),
                ExpirationDate = this.ExpirationDate.ToString()
            };

            string json = JsonConvert.SerializeObject(dataToEncrypt);
            byte[] plaintextBytes = Encoding.UTF8.GetBytes(json);
            byte[] encryptedBytes = publicKey.Encrypt(plaintextBytes, RSAEncryptionPadding.Pkcs1);
            return Convert.ToBase64String(encryptedBytes);
        }


        public static string Decrypt(string encryptedData)
        {
            // Convert the Base64 string back to a byte array
            byte[] encryptedBytes = Convert.FromBase64String(encryptedData);

            // Read the private key from the PEM file
            string privateKeyPem = File.ReadAllText("../../RSAcertificates/private.key");

            // Create RSA object and import the private key from PEM
            RSA privateKey = RSA.Create();
            privateKey.ImportFromPem(privateKeyPem.ToCharArray()); // Use the actual PEM key content, not the file path

            // Decrypt the byte array using the private key
            byte[] decryptedBytes = privateKey.Decrypt(encryptedBytes, RSAEncryptionPadding.Pkcs1);

            // Convert the decrypted bytes back to a UTF-8 string (JSON format)
            return Encoding.UTF8.GetString(decryptedBytes);
        }


    }
}
