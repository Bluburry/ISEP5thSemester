using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DDDSample1.Domain.LoginAttemptTrackers;
using DDDSample1.Domain.Shared;
using DDDSample1.Domain.Specializations;
using DDDSample1.Domain.Users;
using DDDSample1.Domain.ValueObjects;
using DDDSample1.DTO;
using DDDSample1.DTO.LoginAttemptTrackers;
using Microsoft.AspNetCore.Authentication;

namespace DDDSample1.Domain.HospitalStaff
{
    public class LoginService
    {
        const int MAXIMUM_LOGINS = 5;

        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRepository _usrRepo;
        private readonly ILoginAttemptTrackerRepository _loginRepo;

        public LoginService(IUnitOfWork unitOfWork, IUserRepository usrRepo, ILoginAttemptTrackerRepository loginRepo){
            
            _unitOfWork = unitOfWork;
            _usrRepo = usrRepo;
            _loginRepo = loginRepo;
        }

        public LoginService(){
        }

        public virtual async Task<LoginOutputDto> Login(LoginCredentialsDto credentials)
        {
            LoginOutputDto retDto = new LoginOutputDto();
            User user = await _usrRepo.GetByIdAsync(new Username(credentials.Username));

            // Check if the user exists
            if (user == null)
            {
                throw new Exception("User not found:\n\tUsername: "+credentials.Username+"\n\tPassword: "+credentials.Password);
            }

            if(user.ActivationStatus == ActivationStatus.DEACTIVATED){
                
                retDto.Result = LoginResult.DEACTIVATED.ToString();

                return retDto;
            }

            LoginAttemptTracker loginAttempts = await _loginRepo.GetByIdAsync(user.Id);
            


            if(PasswordMatches(user.Password, credentials.Password)){
                retDto.Result = LoginResult.Success.ToString();

                if(loginAttempts == null){
                    
                    loginAttempts = LoginAttemptTrackerFactory.Create(user.Id, 0);
                    await _loginRepo.AddAsync(loginAttempts);
                }else{
                    loginAttempts.AttemptCounterReset();
                     _loginRepo.Update(loginAttempts);
                }

            }else{
                retDto.Result = LoginResult.Failure.ToString();
                
                if(loginAttempts == null){
                    
                    loginAttempts = LoginAttemptTrackerFactory.Create(user.Id,1);
                    await _loginRepo.AddAsync(loginAttempts);
                }else{
                    loginAttempts.IncrementAttemptCounter();
                     _loginRepo.Update(loginAttempts);
                }

                if(loginAttempts.LoginAttempts() > MAXIMUM_LOGINS){
                    loginAttempts.AttemptCounterReset();
                    retDto.Result = LoginResult.AccountLocked.ToString();
                     _loginRepo.Update(loginAttempts);
                }
                
            }

            await _unitOfWork.CommitAsync();
            
            return retDto;

        }


        private bool PasswordMatches(Password actualPassword, string verifyPassword){
            return (new Password(verifyPassword).Equals(actualPassword));
        }

        public virtual LoginCredentialsDto TreatAuthenticateResult(string[] result)
        {
            string username = result[4].Split(':').Last();
            string password = "IAM-" + result[0].Split(':').Last();

            return new LoginCredentialsDto(username, password);
        }

    }
}