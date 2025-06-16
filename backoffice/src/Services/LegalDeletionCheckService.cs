using System;
using System.Threading;
using System.Threading.Tasks;
using DDDSample1.Domain.HospitalPatient;
using DDDSample1.Domain.Logs;
using DDDSample1.Domain.Shared;
using DDDSample1.Domain.Users;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DDDSample1.AppServices
{
    public class LegalDeletionCheckService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public LegalDeletionCheckService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                    var patientRepo = scope.ServiceProvider.GetRequiredService<IPatientRepository>();
                    var userRepo = scope.ServiceProvider.GetRequiredService<IUserRepository>();
                    var logRepo = scope.ServiceProvider.GetRequiredService<ILogRepository>();

                    var logs = await logRepo.GetAllAsync();

                    foreach (var log in logs)
                    {
                        if(log.loggedType == ObjectLoggedType.PATIENT_DELETION && (DateTime.Now - log.LoggedDate.DateTime).TotalDays >= 28){
                            Patient patient = await patientRepo.GetByIdAsync(new MedicalRecordNumber(log.LoggedId));

                            if(patient.userId != null){
                                User user = await userRepo.GetByIdAsync(patient.userId);
                                userRepo.Remove(user);
                            }
                            
                        

                            patient.Anonymize();
                            patientRepo.Update(patient);
                            logRepo.Remove(log);
                            

                            foreach (var check in logs){
                                if(check.LoggedId == log.LoggedId && check.loggedType == log.loggedType){
                                    logRepo.Remove(check);
                                }
                            }

                            await unitOfWork.CommitAsync();
                        }
                    }
                }

                // Delay before the next check
                await Task.Delay(TimeSpan.FromMinutes(30), stoppingToken);
            }
        }


    }
}