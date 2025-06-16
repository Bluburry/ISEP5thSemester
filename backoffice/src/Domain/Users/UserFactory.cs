using System;
using DDDSample1.Domain.Shared;
using DDDSample1.Domain.ValueObjects;

namespace DDDSample1.Domain.Users
{
    public class UserFactory
    {
        public User getUserWithoutPassword(string email, UserRole role)
        {
            if (email == null){
                throw new ArgumentException("Email is Null");
            }

            if (role == null){
                throw new ArgumentException("Role is Null");
            }

            

            return new User(email, role);
        }

        public User CreateActivatedUser(string email, string password, UserRole role)
        {
            var user = new User(email, password, role);
            user.Activate();
            return user;
        }

        public User CreateDeactivatedUser(string email, string password, UserRole role)
        {
            return new User(email, password, role); // Already deactivated by default
        }


        public User ActivateUserPassword(User user, string password){
            Password password1 = new Password(password);

            user.ChangePassword(password);

            return user;
        }

    }
}
