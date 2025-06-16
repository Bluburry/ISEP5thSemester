using DDDSample1.Domain.ValueObjects;
using DDDSample1.Domain.Specializations;

namespace DDDSample1.Domain.OperationTypes.Tests
{
	public class OperationTypeTests
	{
		// private readonly List<Mock<Specialization>> _mockSpecialists;
		private readonly List<Specialization> _specialists;
		private readonly List<string> _specialistsCount;
		private readonly List<string> _specialistPhases;
		private readonly List<string> _phaseNames;
		private readonly List<string> _phasesDuration;
		private readonly OperationType _operation;

		public OperationTypeTests()
		{
			_specialistsCount = ["3", "2", "1"];
			_phaseNames = ["preparation", "surgery", "cleaning"];
			_phasesDuration = ["15", "20", "30"];

			_specialists = [];
			for (int i = 0; i < 3; i++)
			{
				_specialists.Add(new Specialization($"test{i}", ""));
			}
			_specialistPhases = [];
			_specialistPhases.Add("preparation");
			_specialistPhases.Add("surgery");
			_specialistPhases.Add("cleaning");
			/* _mockSpecialists = [];

			// Mock<Specialization> mockSpeHelper;

			for (int i = 0; i < 3; i++)
			{
				/* mockSpeHelper = new Mock<Specialization>();
				// mockSpeHelper.SetupProperty(h => h.Id.Value, $"test{i}"); // ???
				// mockSpeHelper.SetupProperty(h => h.Id, new SpecializationName($"test{i}"));
				mockSpeHelper.Setup(s => s.Id).Returns(new SpecializationName($"test{i}"));

				_mockSpecialists.Add(new Mock<Specialization>());
			} */

			OperationTypeBuilder builder = new();

			_operation = builder
				.WithOperationTypeName("test")
				.CreateOperationType()
				.WithEstimatedDuration("70")
				// .WithRequiredSpecialists(_mockSpecialists.ConvertAll(ms => ms.Object), _specialistsCount)
				.WithRequiredSpecialists(_specialists, _specialistsCount, _specialistPhases)
				.WithOperationPhases(_phaseNames, _phasesDuration)
				.Build();
			
		}

		[Fact]
		public void Test_OperationTypeBuilder_Success()
		{
			OperationTypeBuilder builder = new();

			OperationType ot = builder
				.WithOperationTypeName("test")
				.CreateOperationType()
				.WithEstimatedDuration("70")
				// .WithRequiredSpecialists(_mockSpecialists.ConvertAll(ms => ms.Object), _specialistsCount)
				.WithRequiredSpecialists(_specialists, _specialistsCount, _specialistPhases)
				.WithOperationPhases(_phaseNames, _phasesDuration)
				.Build();
			
			Assert.NotNull(ot);
			Assert.Equal("test", ot.OperationTypeName.OperationName);
			Assert.Equal(70, ot.EstimatedDuration.Duration);
		}

		[Fact]
		public void Test_OperationTypeBuilder_WithoutName()
		{
			OperationTypeBuilder builder = new();

			var exception = Assert.Throws<ArgumentException>(() => builder
				.CreateOperationType()
				.WithEstimatedDuration("70")
				// .WithRequiredSpecialists(_mockSpecialists.ConvertAll(ms => ms.Object), _specialistsCount)
				.WithRequiredSpecialists(_specialists, _specialistsCount, _specialistPhases)
				.WithOperationPhases(_phaseNames, _phasesDuration)
				.Build());
			
			Assert.Equal("Operation name is required", exception.Message);
		}

		[Fact]
		public void Test_OperationTypeBuilder_WithoutDuration()
		{
			OperationTypeBuilder builder = new();

			Assert.Throws<ArgumentException>(() => builder
				.WithOperationTypeName("test")
				.CreateOperationType()
				// .WithRequiredSpecialists(_mockSpecialists.ConvertAll(ms => ms.Object), _specialistsCount)
				.WithRequiredSpecialists(_specialists, _specialistsCount, _specialistPhases)
				.WithOperationPhases(_phaseNames, _phasesDuration)
				.Build());
		}

		[Fact]
		public void Test_OperationTypeBuilder_WithoutPhases()
		{
			OperationTypeBuilder builder = new();

			var exception = Assert.Throws<ArgumentException>(() => builder
				.WithOperationTypeName("test")
				.CreateOperationType()
				.WithEstimatedDuration("70")
				// .WithRequiredSpecialists(_mockSpecialists.ConvertAll(ms => ms.Object), _specialistsCount)
				.WithRequiredSpecialists(_specialists, _specialistsCount, _specialistPhases)
				.Build());
			
			Assert.Equal("Operation phases are required", exception.Message);
		}

		[Fact]
		public void Test_OperationTypeBuilder_WithoutSpecialists()
		{
			OperationTypeBuilder builder = new();

			Assert.Throws<ArgumentException>(() => builder
				.WithOperationTypeName("test")
				.CreateOperationType()
				.WithEstimatedDuration("70")
				.WithOperationPhases(_phaseNames, _phasesDuration)
				.Build());
		}

		[Fact]
		public void Test_OperationType_IncrementVersion()
		{
			Assert.Equal(1, _operation.VersionNumber);
			_operation.IncrementVersion();
			Assert.Equal(2, _operation.VersionNumber);
		}

		[Fact]
		public void Test_OperationType_DeactivateOperation()
		{
			Assert.Equal(ActivationStatus.ACTIVATED, _operation.ActivationStatus);
			_operation.DeactivateOperationType();
			Assert.Equal(ActivationStatus.DEACTIVATED, _operation.ActivationStatus);
		}
	}
}
