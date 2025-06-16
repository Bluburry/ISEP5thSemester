using DDDSample1.Domain.Tokens;

namespace DDDSample1.DTO.PasswordActivationDto
{
    public class PasswordActivationDto
    {
        public string Password { get; set; }
        public string Token { get; set; }

        public PasswordActivationDto(string password, string token)
        {
            Password = password;
            Token = token;
        }
    }
}
