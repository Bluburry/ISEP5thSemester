using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DDDSample1.Domain.Shared;
using DDDSample1.Domain.Tokens;
using DDDSample1.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;


namespace DDDSample1.Domain.ContactInformations
{
    public class ContactInformation : Entity<ContactInformationId>, IAggregateRoot
    {
        public EmailAddress Email { get; set;}
        public PhoneNumber Phone { get; set;}

        public ContactInformation(EmailAddress email, PhoneNumber phone)
        {
            if (email == null)
                throw new ArgumentNullException(nameof(email), "Email address cannot be null.");

            if (phone == null)
                throw new ArgumentNullException(nameof(phone), "Phone number cannot be null.");

            Email = email;
            Phone = phone;
            this.Id = new ContactInformationId(Guid.NewGuid());
        }

        public ContactInformation()
        {
        }

        public override string ToString()
        {
            return $"Email: {Email}, Phone: {Phone}";
        }

        public override bool Equals(object obj)
        {
            return obj is ContactInformation contactInfo &&
                   Email.Equals(contactInfo.Email) &&
                   Phone.Equals(contactInfo.Phone);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Email, Phone);
        }

    }
}
