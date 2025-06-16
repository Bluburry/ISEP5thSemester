namespace DDDSample1.DTO
{
    public class LoginCredentialsDto
    {
        // Property to hold the username
        public string Username { get; set; }

        // Property to hold the password
        public string Password { get; set; }

        // Constructor to initialize the properties
        public LoginCredentialsDto(string username, string password)
        {
            Username = username;
            Password = password;
        }
    }
}
