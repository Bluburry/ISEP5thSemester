using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DDDSample1.Domain.Shared;
using DDDSample1.Domain.Logs;
using DDDSample1.DTO;
using DDDSample1.Domain.ValueObjects;
using DDDSample1.Domain.HospitalPatient;

namespace DDDSample1.Domain.Logs
{
    public class LogService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogRepository _logRepo;

        public LogService(IUnitOfWork unitOfWork, ILogRepository logRepo)
        {
            _unitOfWork = unitOfWork;
            _logRepo = logRepo;
        }

        public LogService(){}

        
        // Add a new log entry
        public async Task<LogDto> AddLogAsync(LogDto ToBeLogged)
        {

            LogsBuilder builder = new LogsBuilder();

            builder.WithDateAndTime()
                .WithID(ToBeLogged.LoggedId)
                .WithInformation(ToBeLogged.LoggedInformation)
                .WithObjectType(ToBeLogged.LoggedType);
            
            
            Log log = await _logRepo.AddAsync(builder.Build());
            await _unitOfWork.CommitAsync();

            return log.toDto();
        }

        // Get all logs
        public async Task<List<LogDto>> GetAllLogsAsync()
        {
            var logs = await _logRepo.GetAllAsync();
            return logs.ConvertAll(log => log.toDto());
        }

        // Get a specific log by ID
        public async Task<LogDto> GetLogByIdAsync(LogId id)
        {
            var log = await _logRepo.GetByIdAsync(id);
            return log?.toDto();
        }

        // Delete a log by ID
        public async Task<bool> DeleteLogAsync(LogId id)
        {
            var log = await _logRepo.GetByIdAsync(id);

            if (log == null)
            {
                return false;
            }

            _logRepo.Remove(log);
            await _unitOfWork.CommitAsync();
            return true;
        }


        public virtual async Task<LogDto> LogPatientDeletion(PatientDto patientDto)
            {
                // Check if the patientDto is null
                if (patientDto == null)
                {
                    throw new ArgumentNullException(nameof(patientDto), "PatientDto cannot be null.");
                }

                // Check if the MRN (Medical Record Number) is null or empty
                if (string.IsNullOrWhiteSpace(patientDto.mrn))
                {
                    throw new ArgumentException("Medical Record Number cannot be null or empty.", nameof(patientDto.mrn));
                }

                // Create the LogDto object
                LogDto dto = new LogDto
                {
                    LoggedId = patientDto.mrn,
                    LoggedInformation = patientDto.mrn,
                    LoggedType = ObjectLoggedType.PATIENT_DELETION.ToString(),
                    LoggedDate = DateTime.Now.AddSeconds(5).ToString()
                };

                // Call the asynchronous method to add the log
                return await AddLogAsync(dto);
            }

            public virtual async Task<LogDto> LogPatientEditing(PatientDto patientDto)
            {
                // Check if the patientDto is null
                if (patientDto == null)
                {
                    throw new ArgumentNullException(nameof(patientDto), "PatientDto cannot be null.");
                }

                // Check if the MRN (Medical Record Number) is null or empty
                if (string.IsNullOrWhiteSpace(patientDto.mrn))
                {
                    throw new ArgumentException("Medical Record Number cannot be null or empty.", nameof(patientDto.mrn));
                }

                // Create the LogDto object
                LogDto dto = new LogDto
                {
                    LoggedId = patientDto.mrn,
                    LoggedInformation = patientDto.ToString(),
                    LoggedType = ObjectLoggedType.PATIENT.ToString(),
                    LoggedDate = DateTime.Now.AddSeconds(5).ToString()
                };

                // Call the asynchronous method to add the log
                return await AddLogAsync(dto);
            }

            public virtual async Task<LogDto> LogPatientEditingAttempt(PatientDto patientDto, string userID)
            {
                if (patientDto == null)
                {
                    throw new ArgumentNullException(nameof(patientDto), "PatientDto cannot be null.");
                }

                if (string.IsNullOrWhiteSpace(patientDto.mrn))
                {
                    throw new ArgumentException("Medical Record Number cannot be null or empty.", nameof(patientDto.mrn));
                }

                LogDto dto = new LogDto
                {
                    LoggedId = userID,
                    LoggedInformation = patientDto.ToString(),
                    LoggedType = ObjectLoggedType.PATIENT_UPDATE_ATTEMPT.ToString(),
                    LoggedDate = DateTime.Now.AddSeconds(5).ToString()
                };

                return await AddLogAsync(dto);
            }

    }
}
