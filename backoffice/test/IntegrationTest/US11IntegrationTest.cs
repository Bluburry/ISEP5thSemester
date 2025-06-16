using DDDSample1.Controllers;
using DDDSample1.Domain.ContactInformations;
using DDDSample1.Domain.HospitalAppointment;
using DDDSample1.Domain.HospitalPatient;
using DDDSample1.Domain.Logs;
using DDDSample1.Domain.Shared;
using DDDSample1.Domain.Tokens;
using DDDSample1.Domain.Users;
using DDDSample1.Domain.ValueObjects;
using DDDSample1.DTO;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DDDNetCore.test.IntegrationTest
{
    public class US11IntegrationTest
    {
        private readonly Mock<IPatientRepository> _mockPatientRepo;
        private readonly Mock<ILogRepository> _mockLogRepo;
        private readonly Mock<IUserRepository> _mockUserRepo;
        private readonly Mock<ITokenRepository> _mockTokenRepo;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;

        private readonly PatientService _patSvc;
        private readonly TokenService _tokenSvc;
        private readonly LogService _logSvc;
        private readonly AppointmentService _apptSvc;
        private readonly PatientController controller;
        private User user;

        public US11IntegrationTest()
        {
            _mockPatientRepo = new Mock<IPatientRepository>();
            _mockUserRepo = new Mock<IUserRepository>();
            _mockLogRepo = new Mock<ILogRepository>();
            _mockTokenRepo = new Mock<ITokenRepository>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();

            _tokenSvc = new TokenService(_mockUnitOfWork.Object, _mockUserRepo.Object, _mockTokenRepo.Object);
            _patSvc = new PatientService(_mockUnitOfWork.Object, _mockPatientRepo.Object, _mockUserRepo.Object, null, _tokenSvc, _apptSvc);
            _logSvc = new LogService(_mockUnitOfWork.Object, _mockLogRepo.Object);

            user = new User(
                "user@example.com",
                "!Password123",
                UserRole.PATIENT
                );
            controller = new PatientController(_patSvc, _tokenSvc, _logSvc);
        }

        [Fact]
        public void GetFilteredPatients_Sucess()
        {
            //Arrange
            Patient patient =
                new Patient(
                    new MedicalRecordNumber("MRN12345"),
                    new FirstName("Johnny"),
                    new LastName("Test"),
                    new FullName("Johnny Test"),
                    Gender.MALE,
                    new DateOfBirth("2001-04-01"),
                    new ContactInformation(new EmailAddress("johnny@email.com"), new PhoneNumber("")),
                     new PhoneNumber(""),
                     new List<Appointment>(),
                     user
                );

            QueryDataDto dto = new QueryDataDto();
            dto.Name = "Johnny";

            List<Patient> list = new List<Patient>();
            list.Add(patient);

            IEnumerable<Patient> enumerable = list;

            _mockPatientRepo.Setup(s => s.GetFilteredPatients(It.IsAny<QueryDataDto>()))
                .Returns(enumerable);

            //Act
            var result = controller.GetFilteredPatients(dto);

            //Assert
            _mockPatientRepo.Verify(s => s.GetFilteredPatients(It.IsAny<QueryDataDto>()), Times.Once);

            var okResult = Assert.IsAssignableFrom<ActionResult<IEnumerable<PatientDto>>>(result);
            var okValue = Assert.IsAssignableFrom<OkObjectResult>(result.Result);
        }

    }
}
