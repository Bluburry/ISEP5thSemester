using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DDDSample1.Domain.Shared;
using DDDSample1.Domain.Specializations;
using DDDSample1.Domain.Tokens;
using DDDSample1.Domain.Users;
using DDDSample1.Domain.ValueObjects;
using DDDSample1.DTO;
using Microsoft.AspNetCore.Mvc;

namespace DDDSample1.Domain.Tokens
{
    public class TokenService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRepository _userRepo;
        private readonly ITokenRepository _tokenRepo;

        public TokenService(IUnitOfWork unitOfWork, IUserRepository userRepo, ITokenRepository tokenRepo)
        {
            this._unitOfWork = unitOfWork;
            this._tokenRepo = tokenRepo;
            this._userRepo = userRepo;
        }

        public TokenService()
        {
        }

        public virtual async Task<TokenDto> GeneratePasswordValidationTokenAsync(UserDto userDto)
        {
            TokenFactory factory = new TokenFactory();

            var user = await this._userRepo.GetByIdAsync(new Username(userDto.EmailAddress));
            Token token = factory.createPasswordActivationToken(DateTime.Now.AddDays(1), user);

            token = await this._tokenRepo.AddAsync(token);

            if(token.TheUser == null){
                throw new ArgumentException("User of Token in DB is null lawl");
            }

            await this._unitOfWork.CommitAsync();

            return token.ToDto();
        }

        public virtual async Task<TokenDto> CreateAuthToken(LoginCredentialsDto userDto)
        {
            TokenFactory factory = new TokenFactory();

            var user = await this._userRepo.GetByIdAsync(new Username(userDto.Username));
            Token token = factory.CreateLoginAuthToken(DateTime.Now.AddMinutes(10), user, user.Role);

            token = await this._tokenRepo.AddAsync(token);

            if(token.TheUser == null){
                throw new ArgumentException("User of Token in DB is null lawl");
            }

            await this._unitOfWork.CommitAsync();

            return token.ToDto();
        }

        public virtual async Task<TokenDto> GetByIdAsync(TokenId id){

            Token token = await _tokenRepo.GetByIdAsync(id);

            if(token == null){
                throw new Exception("Token does not exist");
            }

            return token.ToDto();
        }

        public virtual async Task<UserDto> GetTokenUserById(string tokenId)
        {
            TokenId id = new TokenId(tokenId);
            var token = await this._tokenRepo.GetByIdAsync(id);

            if (token == null)
            {
                throw new Exception("Token not found");
            }

            User user = await _userRepo.GetByIdAsync(token.UserId);

            return user.ToDto();
        }

        // New method to get all tokens
        public virtual async Task<List<TokenDto>> GetAllTokensAsync()
        {
            List<Token> tokens = await this._tokenRepo.GetAllAsync();

            if(tokens[0] == null){
                throw new ArgumentException("Tokens are Null");
            }

            if(tokens[0].TheUser == null){
                throw new ArgumentException("Users are Null");
            }


            return tokens.ConvertAll(token => token.ToDto());
        }

        internal virtual async Task<TokenDto> GeneratePasswordResetToken(string resetPasswordDto)
        {
            TokenFactory factory = new TokenFactory();

            var user = await this._userRepo.GetByIdAsync(new Username(resetPasswordDto));
            Token token = factory.createPasswordResetToken(DateTime.Now.AddDays(1), user);

            

            token = await this._tokenRepo.AddAsync(token);

            if(token.TheUser == null){
                throw new ArgumentException("User of Token in DB is null lawl");
            }

            await this._unitOfWork.CommitAsync();

            return token.ToDto();
        }

        public virtual async void RemoveToken(string TokenId){
            Token token = await _tokenRepo.GetByIdAsync(new TokenId(TokenId));
            _tokenRepo.Remove(token);
        }

        public virtual async Task<TokenDto> GenerateUpdateConfirmationToken(string userId){
            TokenFactory factory = new TokenFactory();

            var user = await this._userRepo.GetByIdAsync(new Username(userId));
            if (user == null)
                throw new ArgumentException("User not found for the given userId.");
            Token token = factory.createUpdateConfirmationToken(DateTime.Now.AddDays(1), user);

            token = await this._tokenRepo.AddAsync(token);

            if(token.TheUser == null){
                throw new ArgumentException("User of Token in DB is null lawl");
            }

            await this._unitOfWork.CommitAsync();

            return token.ToDto();
        }

        public virtual async Task<TokenDto> GenerateDeletionConfirmationToken(string userId){
            TokenFactory factory = new TokenFactory();

            var user = await this._userRepo.GetByIdAsync(new Username(userId));
            if (user == null)
                throw new ArgumentException("User not found for the given userId.");
            Token token = factory.createDeletionConfirmationToken(DateTime.Now.AddDays(1), user);

            token = await this._tokenRepo.AddAsync(token);

            if(token.TheUser == null){
                throw new ArgumentException("User of Token in DB is null lawl");
            }

            await this._unitOfWork.CommitAsync();

            return token.ToDto();
        }
    }
}

