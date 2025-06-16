using DDDSample1.Domain.Doctors;
using DDDSample1.Domain.HospitalPatient;
using DDDSample1.Domain.ValueObjects;
using Moq;
using OperationType = DDDSample1.Domain.OperationTypes.OperationType;

namespace DDDSample1.Domain.OperationRequests.Tests
{
	public class OperationRequestTests
	{
		private readonly Mock<Doctor> _mockDoc;
		private readonly Mock<Patient> _mockPatient;
		private readonly Mock<OperationType> _mockOperation;
		private readonly OperationRequest _request;

		public OperationRequestTests()
		{
			_mockDoc = new Mock<Doctor>();
			_mockPatient = new Mock<Patient>();
			_mockOperation = new Mock<OperationType>();

			OperationRequestBuilder builder = new();

			_request = builder
				.WithDoctor(_mockDoc.Object)
				.WithPatient(_mockPatient.Object)
				.WithType(_mockOperation.Object)
				.WithPriority(OperationPriority.LOW.ToString())
				.WithDeadline(DateTime.Now)
				.Build();
		}

		[Fact]
		public void Test_OperationRequestBuilder_Success()
		{
			OperationRequestBuilder builder = new();

			OperationRequest request = builder
				.WithDoctor(_mockDoc.Object)
				.WithPatient(_mockPatient.Object)
				.WithType(_mockOperation.Object)
				.WithPriority(OperationPriority.LOW.ToString())
				.WithDeadline(DateTime.Now)
				.Build();

			Assert.NotNull(request);
			Assert.Equal(OperationPriority.LOW, request.OperationPriority);
		}

		[Fact]
		public void Test_OperationRequestBuilder_WithoutDoctor()
		{

			OperationRequestBuilder builder = new();

			var exception = Assert.Throws<ArgumentException>(() => builder
				.WithPatient(_mockPatient.Object)
				.WithType(_mockOperation.Object)
				.WithPriority(OperationPriority.LOW.ToString())
				.WithDeadline(DateTime.Now)
				.Build());

			Assert.Equal("Doctor is required.", exception.Message);
		}

		[Fact]
		public void Test_OperationRequestBuilder_WithoutPatient()
		{

			OperationRequestBuilder builder = new();

			var exception = Assert.Throws<ArgumentException>(() => builder
				.WithDoctor(_mockDoc.Object)
				.WithType(_mockOperation.Object)
				.WithPriority(OperationPriority.LOW.ToString())
				.WithDeadline(DateTime.Now)
				.Build());

			Assert.Equal("Patient is required.", exception.Message);
		}

		[Fact]
		public void Test_OperationRequestBuilder_WithoutOperationType()
		{

			OperationRequestBuilder builder = new();

			var exception = Assert.Throws<ArgumentException>(() => builder
				.WithDoctor(_mockDoc.Object)
				.WithPatient(_mockPatient.Object)
				.WithPriority(OperationPriority.LOW.ToString())
				.WithDeadline(DateTime.Now)
				.Build());

			Assert.Equal("Operation type is required.", exception.Message);
		}

		[Fact]
		public void Test_OperationRequest_ChangeStatus()
		{
			Assert.Equal(OperationStatus.PENDING, _request.OperationStatus);
			_request.ApproveRequest();
			Assert.Equal(OperationStatus.APPROVED, _request.OperationStatus);
			_request.RevokeRequest();
			Assert.Equal(OperationStatus.PENDING, _request.OperationStatus);
		}
	}
}
