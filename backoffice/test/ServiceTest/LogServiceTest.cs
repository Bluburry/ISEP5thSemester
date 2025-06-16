using DDDSample1.Domain.HospitalAppointment;
using DDDSample1.Domain.HospitalPatient;
using DDDSample1.Domain.HospitalStaff;
using DDDSample1.Domain.Logs;
using DDDSample1.Domain.Shared;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDDNetCore.test.ServiceTest
{
    public class LogServiceTest
    {

        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<ILogRepository> _mockLogRepo;
        private readonly LogService _logSvc;


        public LogServiceTest()
        {
            _mockLogRepo = new Mock<ILogRepository>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _logSvc = new LogService(_mockUnitOfWork.Object, _mockLogRepo.Object);
        }

        [Fact]
        public async Task LogPatientEditingAttempt_Success_WithValidPatientDTO()
        {
            var patientDto = new PatientDto
            {
                mrn = "123456",
                firstName = "John",
                lastName = "Doe",
                fullName = "John Doe",
                gender = "Male",
                dateOfBirth = "01-01-1990",
                email = "johndoe@example.com",
                phone = "123-456-7890",
                emergencyContact = "Jane Doe, 098-765-4321",
                appointmentHistory = new List<AppointmentDto>() // Empty list
            };

            LogsBuilder builder = new LogsBuilder();
            builder.WithDateAndTime()
                .WithID("user@domain.com")
                .WithInformation(patientDto.ToString())
                .WithObjectType(ObjectLoggedType.PATIENT_UPDATE_ATTEMPT.ToString());

            Log log = builder.Build();


            _mockLogRepo.Setup(l => l.AddAsync(It.IsAny<Log>()))
                .ReturnsAsync(log);

            _mockUnitOfWork.Setup(l => l.CommitAsync());


            LogDto retDto = await _logSvc.LogPatientEditingAttempt(patientDto, "user@domain.com");

            Assert.Equal(retDto.LoggedInformation, log.LoggedInformation);
            Assert.Equal(retDto.LoggedId, log.LoggedId);
            Assert.Equal(retDto.LoggedType, log.loggedType.ToString());

            _mockLogRepo.Verify(l => l.AddAsync(It.IsAny<Log>()), Times.Once);
            _mockUnitOfWork.Verify(l => l.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task LogPatientEditingAttempt_Failure_WithInvalidMedicalRecordNumber()
        {
            var patientDto = new PatientDto
            {
                mrn = "",
                firstName = "John",
                lastName = "Doe",
                fullName = "John Doe",
                gender = "Male",
                dateOfBirth = "01-01-1990",
                email = "johndoe@example.com",
                phone = "123-456-7890",
                emergencyContact = "Jane Doe, 098-765-4321",
                appointmentHistory = new List<AppointmentDto>() // Empty list
            };

            LogsBuilder builder = new LogsBuilder();
            builder.WithDateAndTime()
                .WithID("user@domain.com")
                .WithInformation(patientDto.ToString())
                .WithObjectType(ObjectLoggedType.PATIENT_UPDATE_ATTEMPT.ToString());

            Log log = builder.Build();


            _mockLogRepo.Setup(l => l.AddAsync(It.IsAny<Log>()))
                .ReturnsAsync(log);

            _mockUnitOfWork.Setup(l => l.CommitAsync());


            await Assert.ThrowsAsync<ArgumentException>(async () => await _logSvc.LogPatientEditingAttempt(patientDto, "user@domain.com"));


        }



        [Fact]
        public async Task LogPatientDeletion_Success_WithValidPatientDTO()
        {
            var patientDto = new PatientDto
            {
                mrn = "123456",
                firstName = "John",
                lastName = "Doe",
                fullName = "John Doe",
                gender = "Male",
                dateOfBirth = "01-01-1990",
                email = "johndoe@example.com",
                phone = "123-456-7890",
                emergencyContact = "Jane Doe, 098-765-4321",
                appointmentHistory = new List<AppointmentDto>() // Empty list
            };

            LogsBuilder builder = new LogsBuilder();
            builder.WithDateAndTime()
                .WithID("user@domain.com")
                .WithInformation(patientDto.ToString())
                .WithObjectType(ObjectLoggedType.PATIENT_UPDATE_ATTEMPT.ToString());

            Log log = builder.Build();


            _mockLogRepo.Setup(l => l.AddAsync(It.IsAny<Log>()))
                .ReturnsAsync(log);

            _mockUnitOfWork.Setup(l => l.CommitAsync());


            LogDto retDto = await _logSvc.LogPatientDeletion(patientDto);

            Assert.Equal(retDto.LoggedInformation, log.LoggedInformation);
            Assert.Equal(retDto.LoggedId, log.LoggedId);
            Assert.Equal(retDto.LoggedType, log.loggedType.ToString());

            _mockLogRepo.Verify(l => l.AddAsync(It.IsAny<Log>()), Times.Once);
            _mockUnitOfWork.Verify(l => l.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task LogPatientDeletion_Failure_WithInvalidMedicalRecordNumber()
        {
            var patientDto = new PatientDto
            {
                mrn = "",
                firstName = "John",
                lastName = "Doe",
                fullName = "John Doe",
                gender = "Male",
                dateOfBirth = "01-01-1990",
                email = "johndoe@example.com",
                phone = "123-456-7890",
                emergencyContact = "Jane Doe, 098-765-4321",
                appointmentHistory = new List<AppointmentDto>() // Empty list
            };

            LogsBuilder builder = new LogsBuilder();
            builder.WithDateAndTime()
                .WithID("user@domain.com")
                .WithInformation(patientDto.ToString())
                .WithObjectType(ObjectLoggedType.PATIENT_UPDATE_ATTEMPT.ToString());

            Log log = builder.Build();


            _mockLogRepo.Setup(l => l.AddAsync(It.IsAny<Log>()))
                .ReturnsAsync(log);

            _mockUnitOfWork.Setup(l => l.CommitAsync());


            await Assert.ThrowsAsync<ArgumentException>(async () => await _logSvc.LogPatientDeletion(patientDto));


        }

    }
}
