using System;
using System.Threading.Tasks;
using DDDSample1.Domain.Shared;
using DDDSample1.Domain.Tokens;
using DDDSample1.Domain.Users;
using DDDSample1.Domain.ValueObjects;

namespace DDDSample1.AppServices
{
    public class PasswordActivationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRepository _userRepo;
        private readonly TokenService _tokenSvc;

        public PasswordActivationService(IUnitOfWork unitOfWork, IUserRepository userRep, TokenService tokenSvc)
        {
            this._unitOfWork = unitOfWork;
            this._tokenSvc = tokenSvc;
            this._userRepo = userRep;
        }

        public PasswordActivationService()
        {
        
        }


        public virtual async Task<UserDto> ActivatePassword(string password, string tokenId){

            var retrievedToken = await _tokenSvc.GetByIdAsync(new TokenId(tokenId));

            if (retrievedToken == null)
            {
                throw new Exception("Token not found" + tokenId);
            }

            if (retrievedToken.TokenValue != TokenType.VERIFICATION_TOKEN.ToString())
            {
                throw new Exception("Token type does not match operation - " + retrievedToken.TokenValue + " - " + TokenType.VERIFICATION_TOKEN);
            }

            var currentUser = await _userRepo.GetByIdAsync(new Username(retrievedToken.UserId));

            if (currentUser == null)
            {
                throw new Exception("User not found");
            }

            UserFactory factory = new UserFactory();

            currentUser.Activate();
            var updatedUser = factory.ActivateUserPassword(currentUser, password);

            

            var newUser = this._userRepo.Update(updatedUser);
            await this._unitOfWork.CommitAsync();

            _tokenSvc.RemoveToken(tokenId);

            return newUser.ToDto();
        }

        public virtual async Task<UserDto> ResetPassword(string password, UserDto userDto)
        {

            User user = await _userRepo.GetByIdAsync(new Username(userDto.EmailAddress));

            user.ChangePassword(password);

            user = _userRepo.Update(user);

            await this._unitOfWork.CommitAsync();

            return user.ToDto();
        }
    }

}