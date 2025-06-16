namespace DDDSample1.DTO
{
    public class ResetPasswordDto
    {
        // Property to hold the user's email address for password reset
        public string Email { get; set; }

        // Constructor to initialize the email property
        public ResetPasswordDto(string email)
        {
            Email = email;
        }
    }
}
