using DDDSample1.Domain.Logs;
using DDDSample1.Domain.Shared;
using DDDSample1.Domain.Specializations;
using DDDSample1.Domain.ValueObjects;
using Moq;


namespace DDDNetCore.test.ServiceTest
{
	public class SpecializationServiceTest
	{
		private readonly Mock<IUnitOfWork> _mockUnitOfWork;
		private readonly Mock<ILogRepository> _mockLogRepo;
		private readonly Mock<ISpecializationRepository> _mockSpecRepo;
		private readonly SpecializationService _service;
		private readonly Specialization _spec;
		private readonly Specialization _spec2;
		private readonly List<Specialization> _specList;

		public SpecializationServiceTest()
		{
			_mockUnitOfWork = new Mock<IUnitOfWork>();
			_mockSpecRepo = new Mock<ISpecializationRepository>();
			_mockLogRepo = new Mock<ILogRepository>();

			_specList = [];
			_spec = new Specialization("TestingSpecialization", "short description", "SpecCode123");
			_spec2 = new Specialization("SpecializationTesting", "short description", "321SpecCode");
			_specList.Add(_spec);
			_specList.Add(_spec2);

			_service = new SpecializationService(_mockUnitOfWork.Object, _mockSpecRepo.Object, _mockLogRepo.Object);
		}

		[Fact]
		public async Task RegisterSpecialization_Successful()
		{
			_mockSpecRepo.Setup(s => s.GetByName(It.IsAny<string>()));//.ReturnsAsync(null);
			_mockSpecRepo.Setup(s => s.AddAsync(It.IsAny<Specialization>())).ReturnsAsync(_spec);

			SpecializationDTO ret = await _service.CreateSpecialization(_spec.Id.AsString(), _spec.SpecializationName, _spec.SpecializationDescription);

			Assert.NotNull(ret);
			Assert.Equal(_spec.SpecializationName, ret.SpecializationName);

			_mockSpecRepo.Verify(r => r.GetByName(It.IsAny<string>()), Times.Once);
			_mockSpecRepo.Verify(r => r.AddAsync(It.IsAny<Specialization>()), Times.Once);
			_mockUnitOfWork.Verify(r => r.CommitAsync(), Times.Once);
		}

		[Fact]
		public async Task RegisterSpecialization_Unsuccessful()
		{
			_mockSpecRepo.Setup(s => s.GetByName(It.IsAny<string>())).ReturnsAsync(_spec);
			_mockSpecRepo.Setup(s => s.AddAsync(It.IsAny<Specialization>())).ReturnsAsync(_spec);

			var exception = await Assert.ThrowsAsync<ArgumentException>(async () =>
				await _service.CreateSpecialization(
					_spec.Id.AsString(), _spec.SpecializationName, _spec.SpecializationDescription
				)
			);

			Assert.Equal("A specialization with that name already exists", exception.Message);

			_mockSpecRepo.Verify(r => r.GetByName(It.IsAny<string>()), Times.Once);
			_mockSpecRepo.Verify(r => r.AddAsync(It.IsAny<Specialization>()), Times.Never);
			_mockUnitOfWork.Verify(r => r.CommitAsync(), Times.Never);
		}

		[Fact]
		public async Task FilteredGet_NameSuccessful()
		{
			_mockSpecRepo.Setup(s => s.GetByName(It.IsAny<string>())).ReturnsAsync(_spec);

			SpecializationDTO ret = await _service.FilteredGet("", _spec.SpecializationName);

			Assert.NotNull(ret);
			Assert.Equal(_spec.SpecializationName, ret.SpecializationName);

			_mockSpecRepo.Verify(r => r.GetByName(It.IsAny<string>()), Times.Once);
			_mockSpecRepo.Verify(r => r.GetByIdAsync(It.IsAny<SpecializationCode>()), Times.Never);
		}

		[Fact]
		public async Task FilteredGet_CodeSuccessful()
		{
			_mockSpecRepo.Setup(s => s.GetByIdAsync(It.IsAny<SpecializationCode>())).ReturnsAsync(_spec);

			SpecializationDTO ret = await _service.FilteredGet(_spec.Id.AsString(), _spec.SpecializationName);

			Assert.NotNull(ret);
			Assert.Equal(_spec.SpecializationName, ret.SpecializationName);

			_mockSpecRepo.Verify(r => r.GetByName(It.IsAny<string>()), Times.Never);
			_mockSpecRepo.Verify(r => r.GetByIdAsync(It.IsAny<SpecializationCode>()), Times.Once);
		}

		[Fact]
		public async Task Delete_Successful()
		{
			_mockSpecRepo.Setup(s => s.GetByIdAsync(It.IsAny<SpecializationCode>())).ReturnsAsync(_spec);

			SpecializationDTO ret = await _service.DeleteSpecialization(_spec.Id.AsString());

			_mockSpecRepo.Verify(r => r.GetByIdAsync(It.IsAny<SpecializationCode>()), Times.Once);
			_mockSpecRepo.Verify(r => r.Remove(It.IsAny<Specialization>()), Times.Once);
			_mockUnitOfWork.Verify(r => r.CommitAsync(), Times.Once);
		}

		[Fact]
		public async Task Edit_Unsuccessful1()
		{
			_mockSpecRepo.Setup(s => s.GetByIdAsync(It.IsAny<SpecializationCode>())).ReturnsAsync(_spec);
			_mockSpecRepo.Setup(s => s.GetByName(It.IsAny<string>())).ReturnsAsync(_spec);

			var exception = await Assert.ThrowsAsync<ArgumentException>(async () =>
				await _service.UpdateSpecialization(_spec.Id.AsString(), "", "")
			);

			Assert.Equal("Can't update specialization with null name and description", exception.Message);

			_mockSpecRepo.Verify(r => r.GetByName(It.IsAny<string>()), Times.Never);
			_mockSpecRepo.Verify(r => r.AddAsync(It.IsAny<Specialization>()), Times.Never);
			_mockUnitOfWork.Verify(r => r.CommitAsync(), Times.Never);
		}

		[Fact]
		public async Task Edit_Unsuccessful2()
		{
			_mockSpecRepo.Setup(s => s.GetByIdAsync(It.IsAny<SpecializationCode>())).ReturnsAsync(_spec);
			_mockSpecRepo.Setup(s => s.GetByName(It.IsAny<string>())).ReturnsAsync(_spec);

			var exception = await Assert.ThrowsAsync<ArgumentException>(async () =>
				await _service.UpdateSpecialization(
					_spec.Id.AsString(), _spec.SpecializationName, _spec.SpecializationDescription
					)
				);

			Assert.Equal("Update values are the same as the original specialization", exception.Message);

			_mockSpecRepo.Verify(r => r.GetByName(It.IsAny<string>()), Times.Never);
			_mockSpecRepo.Verify(r => r.AddAsync(It.IsAny<Specialization>()), Times.Never);
			_mockUnitOfWork.Verify(r => r.CommitAsync(), Times.Never);
		}

		[Fact]
		public async Task Edit_Successful()
		{
			LogsBuilder logsBuilder = new();

			Log log = logsBuilder
				.WithDateAndTime(new DateAndTime(DateTime.Now))
				.WithInformation(_spec.ToString())
				.WithID(_spec.Id.AsString())
				.WithObjectType(ObjectLoggedType.SPECIALIZATION.ToString())
				.Build();
				
			_mockSpecRepo.Setup(s => s.GetByIdAsync(It.IsAny<SpecializationCode>())).ReturnsAsync(_spec);
			_mockSpecRepo.Setup(s => s.Update(It.IsAny<Specialization>())).Returns(_spec);
			_mockSpecRepo.Setup(s => s.GetByName(It.IsAny<string>()));
			_mockLogRepo.Setup(s => s.AddAsync(It.IsAny<Log>())).ReturnsAsync(log);


			SpecializationDTO ret = await _service.UpdateSpecialization(_spec.Id.AsString(), "new name", "new desc");

			Assert.NotNull(ret);

			_mockSpecRepo.Verify(r => r.GetByName(It.IsAny<string>()), Times.Once);
			_mockSpecRepo.Verify(r => r.Update(It.IsAny<Specialization>()), Times.Once);
			_mockLogRepo.Verify(r => r.AddAsync(It.IsAny<Log>()), Times.Once);
			_mockUnitOfWork.Verify(r => r.CommitAsync(), Times.Once);
		}
	}
}