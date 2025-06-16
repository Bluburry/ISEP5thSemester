using System;

namespace DDDSample1.Domain.Users
{
    public class UserDto
    {
        public string EmailAddress { get; set; }
        public string Role { get; set; }
        public string ActivationStatus { get; set; }

        public UserDto() {}

        public UserDto(string EmailAddress, string role, string activationStatus)
        {
            this.EmailAddress = EmailAddress;
            this.Role = role;
            this.ActivationStatus = activationStatus;
        }

        public override string ToString()
        {
            return $"EmailAddress: {EmailAddress}, Role: {Role}, ActivationStatus: {ActivationStatus}";
        }
    }
}
