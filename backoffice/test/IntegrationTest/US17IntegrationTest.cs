using DDDSample1.Controllers;
using DDDSample1.Domain.ContactInformations;
using DDDSample1.Domain.Doctors;
using DDDSample1.Domain.HospitalAppointment;
using DDDSample1.Domain.HospitalPatient;
using DDDSample1.Domain.HospitalStaff;
using DDDSample1.Domain.Logs;
using DDDSample1.Domain.OperationPhases;
using DDDSample1.Domain.OperationRequests;
using DDDSample1.Domain.OperationTypes;
using DDDSample1.Domain.RequiredSpecialists;
using DDDSample1.Domain.Shared;
using DDDSample1.Domain.Specializations;
using DDDSample1.Domain.Tokens;
using DDDSample1.Domain.Users;
using DDDSample1.Domain.ValueObjects;
using DDDSample1.DTO;
using DDDSample1.DTO.LoginAttemptTrackers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDDNetCore.test.IntegrationTest
{
    public class US17IntegrationTest
    {
		private readonly Mock<IUnitOfWork> _mockUnitOfWork;
		private readonly Mock<IOperationRequestRepository> _mockRequestRepo;
		private readonly Mock<IStaffRepository> _mockStaffRepo;
		private readonly Mock<IDoctorRepository> _mockDoctorRepo;
		private readonly Mock<IPatientRepository> _mockPatientRepo;
		private readonly Mock<IOperationTypeRepository> _mockTypeRepo;
		private readonly Mock<ILogRepository> _mockLogRepo;
        private readonly Mock<ITokenRepository> _mockTokenRepo;
        private readonly Mock<IUserRepository> _mockUserRepo;
		private readonly OperationRequestService _opReqSvc;

        private readonly UserService _usrSvc;
        private readonly Mock<TokenService> _mockTokenSvc;
        private readonly LogService _logSvc;

        private readonly OperationRequestController controller;
        private User user;

		private readonly List<OperationPhase> _opPhases;
		private readonly List<RequiredSpecialist> _reqSpec;
        private OperationRequest _opReq;
		public US17IntegrationTest()
        {
			_mockUnitOfWork = new Mock<IUnitOfWork>();

            _mockUserRepo = new Mock<IUserRepository>();
            _mockPatientRepo = new Mock<IPatientRepository>();
			_mockRequestRepo = new Mock<IOperationRequestRepository>();
			_mockStaffRepo = new Mock<IStaffRepository>();
			_mockDoctorRepo = new Mock<IDoctorRepository>();
			_mockTypeRepo = new Mock<IOperationTypeRepository>();
            _mockTokenRepo = new Mock<ITokenRepository>();
			_mockLogRepo = new Mock<ILogRepository>();

			_opReqSvc = new OperationRequestService(_mockUnitOfWork.Object, _mockRequestRepo.Object, _mockStaffRepo.Object,
			_mockDoctorRepo.Object, _mockPatientRepo.Object, _mockTypeRepo.Object, _mockLogRepo.Object);

            _mockTokenSvc = new Mock<TokenService>();
			_usrSvc = new UserService(_mockUnitOfWork.Object, _mockUserRepo.Object, _mockTokenSvc.Object);
            
            controller = new OperationRequestController(_opReqSvc, _usrSvc, _mockTokenSvc.Object, null, null);

			_reqSpec = [];
			_reqSpec.Add(new RequiredSpecialist(new Specialization("SpecializedInCopyright", ""), new SpecialistCount("10"), PhaseName.PREPARATION));
			_reqSpec.Add(new RequiredSpecialist(new Specialization("CopyrightedInSpecialization", ""), new SpecialistCount("10"), PhaseName.SURGERY));
			_reqSpec.Add(new RequiredSpecialist(new Specialization("CopyrightSpecialized", ""), new SpecialistCount("10"), PhaseName.CLEANING));

			_opPhases = [];
			_opPhases.Add(new OperationPhase(PhaseName.PREPARATION, 15));
			_opPhases.Add(new OperationPhase(PhaseName.SURGERY, 35));
			_opPhases.Add(new OperationPhase(PhaseName.CLEANING, 15));

			_opReq = new(_doctor, _patient,
			new OperationType(new OperationTypeName("operatedTypings"), _opPhases, _reqSpec, new EstimatedDuration(70)), OperationPriority.LOW, DateTime.Now);
        }

        private static User _userPatient = new User("joaogarrafas@nope.com", UserRole.PATIENT);

		private static User _userStaff = new User("no@nope.university.com", UserRole.STAFF);

		private static Patient _patient = new Patient(
			new MedicalRecordNumber("MRN12346"), new FirstName("John"), new LastName("Bottles"), new FullName("John Bottles"), Gender.MALE,
			new DateOfBirth("2004-02-24"), new ContactInformation(new EmailAddress("joaogarrafas@nope.com"), new PhoneNumber("968502397")),
			new PhoneNumber("968502397"), new List<Appointment>(), _userPatient);

		private static Staff _staff = new(new LicenseNumber("1"), new ContactInformation(new EmailAddress("no@nope.university.com"), new PhoneNumber("123456789")),
			new FullName("Titigas Silvations"), new Specialization("blehSpecialization", ""), _userStaff, new FirstName("Titigas"), new LastName("Silvations"));

		private static Doctor _doctor = new(_staff);
		

		private static Token _token = new Token(new TokenId("c185d517-d467-4ba5-a789-eb4f77a194b1"), DateTime.Now, _userStaff, TokenType.STAFF_AUTH_TOKEN);

        [Fact]
        public async Task EditOperationRequest_Successful(){
         
            _mockTokenSvc.Setup(s => s.GetByIdAsync(It.IsAny<TokenId>()))
				.ReturnsAsync(_token.ToDto());

			_mockRequestRepo.Setup(req => req.GetRequestById(It.IsAny<OperationRequestId>()))
				.ReturnsAsync(_opReq);

			_mockStaffRepo.Setup(s => s.GetStaffByUser(It.IsAny<Username>()))
				.ReturnsAsync(_staff);

			_mockDoctorRepo.Setup(d => d.GetDoctorByLicenseNumber(It.IsAny<LicenseNumber>()))
				.ReturnsAsync(_doctor);

			_mockRequestRepo.Setup(req => req.GetByIdAsync(It.IsAny<OperationRequestId>()))
				.ReturnsAsync(_opReq);

			//_mockLogRepo.Setup(l => l.AddAsync(It.IsAny))
			_mockRequestRepo.Setup(req => req.Update(It.IsAny<OperationRequest>()))
				.Returns(_opReq);

			OperationRequestDTO result = (await controller.EditOperationRequest(_opReq.Id.AsString(), "2025-01-20", "CRITICAL", _token.ToDto().TokenId)).Value;
		
			Assert.Equal("CRITICAL", result.OperationPriority.ToString());
			
			_mockTokenSvc.Verify(r => r.GetByIdAsync(It.IsAny<TokenId>()), Times.Once);
			_mockRequestRepo.Verify(r => r.GetRequestById(It.IsAny<OperationRequestId>()), Times.Once);
			_mockDoctorRepo.Verify(r => r.GetDoctorByLicenseNumber(It.IsAny<LicenseNumber>()), Times.Once);
			_mockRequestRepo.Verify(r => r.GetByIdAsync(It.IsAny<OperationRequestId>()), Times.Once);
			_mockRequestRepo.Verify(r => r.Update(It.IsAny<OperationRequest>()), Times.Once);
        }


		[Fact]
        public async Task EditOperationRequest_Unsuccessful(){
         
            _mockTokenSvc.Setup(s => s.GetByIdAsync(It.IsAny<TokenId>()))
				.ReturnsAsync(_token.ToDto());

			_mockRequestRepo.Setup(req => req.GetRequestById(It.IsAny<OperationRequestId>()))
				.ReturnsAsync(_opReq);

			_mockStaffRepo.Setup(s => s.GetStaffByUser(It.IsAny<Username>()))
				.ReturnsAsync(_staff);

			_mockDoctorRepo.Setup(d => d.GetDoctorByLicenseNumber(It.IsAny<LicenseNumber>()))
				.ReturnsAsync(_doctor);

			_mockRequestRepo.Setup(req => req.GetByIdAsync(It.IsAny<OperationRequestId>()))
				.ReturnsAsync(_opReq);

			//_mockLogRepo.Setup(l => l.AddAsync(It.IsAny))
			_mockRequestRepo.Setup(req => req.Update(It.IsAny<OperationRequest>()))
				.Returns(_opReq);

			await Assert.ThrowsAsync<ArgumentException>(async () => await controller.EditOperationRequest(_opReq.Id.AsString(), "2015-01-20", "WHAT", _token.ToDto().TokenId));			
			_mockTokenSvc.Verify(r => r.GetByIdAsync(It.IsAny<TokenId>()), Times.Once);
			_mockRequestRepo.Verify(r => r.GetRequestById(It.IsAny<OperationRequestId>()), Times.Once);
			_mockDoctorRepo.Verify(r => r.GetDoctorByLicenseNumber(It.IsAny<LicenseNumber>()), Times.Once);
			_mockRequestRepo.Verify(r => r.GetByIdAsync(It.IsAny<OperationRequestId>()), Times.Once);
			_mockRequestRepo.Verify(r => r.Update(It.IsAny<OperationRequest>()), Times.Never);
        }

    }
}
