using System;
using System.ComponentModel.DataAnnotations.Schema;
using DDDSample1.Domain.HospitalStaff;
using DDDSample1.Domain.Shared;
using DDDSample1.Domain.ValueObjects;


namespace DDDSample1.Domain.Users
{
    public class User : Entity<Username>, IAggregateRoot
    {
        public virtual EmailAddress EmailAddress { get; set; }
        public virtual Password Password { get; set; }
        public virtual UserRole Role { get; set; }
        public virtual ActivationStatus ActivationStatus { get; set; }

        public User()
        {
            this.ActivationStatus = ActivationStatus.DEACTIVATED; // Default to DEACTIVATED on creation
        }

        public User(string email, string password, UserRole role)
        {
            this.Id = new Username(email);
            this.EmailAddress = new EmailAddress(email);
            this.Password = new Password(password);
            this.Role = role;
            this.ActivationStatus = ActivationStatus.DEACTIVATED;
        }

        /*
            Constructor for creation of Users without a password setup, therefore account is inactive.
        */
        public User(string email,  UserRole role)
        {
            this.Id = new Username(email);
            this.EmailAddress = new EmailAddress(email);
            this.Password = new Password("!Placeholder0");
            this.Role = role;
            this.ActivationStatus = ActivationStatus.DEACTIVATED;
        }

        public void Activate()
        {
            if (this.ActivationStatus == ActivationStatus.ACTIVATED)
                throw new BusinessRuleValidationException("The user is already activated.");
            this.ActivationStatus = ActivationStatus.ACTIVATED;
        }

        public void Deactivate()
        {
            if (this.ActivationStatus == ActivationStatus.DEACTIVATED)
                throw new BusinessRuleValidationException("The user is already deactivated.");
            this.ActivationStatus = ActivationStatus.DEACTIVATED;
        }

        public void ChangePassword(string newPassword)
        {
            if (this.ActivationStatus == ActivationStatus.DEACTIVATED)
                throw new BusinessRuleValidationException("Cannot change password for a deactivated user.");
            this.Password = new Password(newPassword);
        }

        public void ChangeRole(UserRole newRole)
        {
            if (this.ActivationStatus == ActivationStatus.DEACTIVATED)
                throw new BusinessRuleValidationException("Cannot change role for a deactivated user.");
            this.Role = newRole;
        }

        public UserDto ToDto()
        {
            return new UserDto(
                this.EmailAddress.Value,
                this.Role.ToString(),
                this.ActivationStatus.ToString()
            );
        }
        
    }
}
