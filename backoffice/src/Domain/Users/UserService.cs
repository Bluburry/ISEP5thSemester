using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using DDDSample1.Domain.Shared;
using DDDSample1.Domain.ValueObjects;
using DDDSample1.DTO;
using DDDSample1.Domain.Tokens;

namespace DDDSample1.Domain.Users
{
    public class UserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRepository _repo;
        private readonly TokenService _tkSvc;

        public UserService(IUnitOfWork unitOfWork, IUserRepository repo, TokenService tokenService)
        {
            this._unitOfWork = unitOfWork;
            this._repo = repo;
            this._tkSvc = tokenService;
        }

        public UserService()
        {
        }

        public async Task<List<UserDto>> GetAllAsync()
        {
            List<User> list = await this._repo.GetAllAsync();
            
            

            List<UserDto> listDto = list.ConvertAll<UserDto>(user => new UserDto
            {
                EmailAddress = user.Id.AsString(),
                Role = user.Role.ToString(),
                ActivationStatus = user.ActivationStatus.ToString()
            });

            return listDto;
        }

        public virtual async Task<UserDto> GetByIdAsync(Username id)
        {
            var user = await this._repo.GetByIdAsync(id);
            
            if(user == null)
                return null;

            return new UserDto
            {
                EmailAddress = user.Id.AsString(),
                Role = user.Role.ToString(),
                ActivationStatus = user.ActivationStatus.ToString()
            };
        }

        public virtual async Task<UserDto> RegisterBackofficeUserAsync(string emailAddress)
        {
            UserFactory factory = new UserFactory();


            var user = await this._repo.AddAsync(factory.getUserWithoutPassword(emailAddress, UserRole.STAFF));
            await this._unitOfWork.CommitAsync();

            return new UserDto
            {
                EmailAddress = user.Id.AsString(),
                Role = user.Role.ToString(),
                ActivationStatus = user.ActivationStatus.ToString()
            };
        }

        public async Task<UserDto> AddAsync(UserDto dto)
        {
            UserFactory factory = new UserFactory();

            Enum.TryParse(dto.Role, true, out UserRole role);
            //var user = factory.getUserWithoutPassword(dto.EmailAddress, dto.Role);

            var user = await this._repo.AddAsync(factory.getUserWithoutPassword(dto.EmailAddress, role));
            await this._unitOfWork.CommitAsync();

            return new UserDto
            {
                EmailAddress = user.Id.AsString(),
                Role = user.Role.ToString(),
                ActivationStatus = user.ActivationStatus.ToString()
            };
        }

        public virtual async Task<UserDto> AddWithPasswordAsync(LoginCredentialsDto dto)
        {
            UserFactory factory = new UserFactory();

            var user = await this._repo.AddAsync(factory.CreateDeactivatedUser(dto.Username, dto.Password, UserRole.PATIENT));
            await this._unitOfWork.CommitAsync();

            return new UserDto
            {
                EmailAddress = user.Id.AsString(),
                Role = user.Role.ToString(),
                ActivationStatus = user.ActivationStatus.ToString()
            };
        }

        public async Task<UserDto> AddIAMAsync(LoginCredentialsDto dto)
        {
            UserFactory factory = new UserFactory();

            var user = await this._repo.AddAsync(factory.CreateActivatedUser(dto.Username, dto.Password, UserRole.PATIENT));
            await this._unitOfWork.CommitAsync();

            return new UserDto
            {
                EmailAddress = user.Id.AsString(),
                Role = user.Role.ToString(),
                ActivationStatus = user.ActivationStatus.ToString()
            };
        }

        public async Task<UserDto> UpdateAsync(UserDto dto)
        {
            var user = await this._repo.GetByIdAsync(new Username(dto.EmailAddress)); 

            if (user == null)
                return null;

            // Update fields

            user.ChangeRole(Enum.Parse<UserRole>(dto.Role));

            
            await this._unitOfWork.CommitAsync();

            return new UserDto
            {
                EmailAddress = user.Id.AsString(),
                Role = user.Role.ToString(),
                ActivationStatus = user.ActivationStatus.ToString()
            };
        }

        public async Task<UserDto> ActivateAsync(Username id)
        {
            var user = await this._repo.GetByIdAsync(id); 

            if (user == null)
                return null;

            user.Activate();
            await this._unitOfWork.CommitAsync();

            return new UserDto
            {
                EmailAddress = user.Id.AsString(),
                Role = user.Role.ToString(),
                ActivationStatus = user.ActivationStatus.ToString()
            };
        }

        public async Task<UserDto> ActivatePatientAccount(string tokenId){

            var retrievedToken = await _tkSvc.GetByIdAsync(new TokenId(tokenId));

            if (retrievedToken == null)
            {
                throw new Exception("Token not found" + tokenId);
            }

            if (retrievedToken.TokenValue != TokenType.VERIFICATION_TOKEN.ToString())
            {
                throw new Exception("Token type does not match operation - " + retrievedToken.TokenValue + " - " + TokenType.VERIFICATION_TOKEN);
            }

            var currentUser = await _repo.GetByIdAsync(new Username(retrievedToken.UserId));

            if (currentUser == null)
            {
                throw new Exception("User not found");
            }

            currentUser.Activate();

            var newUser = this._repo.Update(currentUser);
            await this._unitOfWork.CommitAsync();

            _tkSvc.RemoveToken(tokenId);

            return newUser.ToDto();
        }


        public async Task<UserDto> DeactivateAsync(Username id)
        {
            var user = await this._repo.GetByIdAsync(id);

            if (user == null)
                return null;

            user.Deactivate();
            await this._unitOfWork.CommitAsync();

            return new UserDto
            {
                EmailAddress = user.Id.AsString(),
                Role = user.Role.ToString(),
                ActivationStatus = user.ActivationStatus.ToString()
            };
        }

        public async Task<UserDto> DeleteAsync(Username id)
        {
            var user = await this._repo.GetByIdAsync(id); 

            if (user == null)
                return null;   

            if (user.ActivationStatus == ActivationStatus.ACTIVATED)
                throw new BusinessRuleValidationException("Cannot delete an active user.");

            this._repo.Remove(user);
            await this._unitOfWork.CommitAsync();

            return new UserDto
            {
                EmailAddress = user.Id.AsString(),
                Role = user.Role.ToString(),
                ActivationStatus = user.ActivationStatus.ToString()
            };
        }
    }
}
