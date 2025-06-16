using DDDSample1.Domain.Tokens;

namespace DDDSample1.DTO.LoginAttemptTrackers
{
    public class LoginOutputDto
    {
        // Property to hold the result of the login attempt
        public string Result { get; set; }

        // Property to hold the authentication token
        public string Token { get; set; }

        public string Type {get; set;}

        // Constructor to initialize the properties
        public LoginOutputDto(LoginResult result, string token, TokenType type)
        {
            Result = result.ToString();
            Token = token;
            this.Type=type.ToString();
        }

        public LoginOutputDto(){}
    }

    public enum LoginResult 
    {
        Success,        
        Failure,        
        AccountLocked,
        DEACTIVATED   
    }
}
