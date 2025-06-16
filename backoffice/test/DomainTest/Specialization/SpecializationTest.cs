namespace DDDSample1.Domain.Specializations.Tests
{
	public class SpecializationTests
	{
		private readonly SpecializationCode _specCode;
		private readonly Specialization _spec;

		public SpecializationTests()
		{
			_specCode = new SpecializationCode("testCode");
			_spec = new Specialization("testing", "for testing", "testingCode");
		}

		[Fact]
		public void Test_SpecializationBuilderSuccessFull()
		{
			SpecializationBuilder builder = new();

			Specialization sp = builder
				.WithSpecializationDescription("testing testing")
				.WithSpecializationCode("123")
				.WithSpecializationName("testing")
				.Build();
			
			Assert.NotNull(sp);
			Assert.Equal("123", sp.Id.AsString());
			Assert.Equal("testing", sp.SpecializationName);
		}

		[Fact]
		public void Test_SpecializationBuilderSuccessNoDescription()
		{
			SpecializationBuilder builder = new();

			Specialization sp = builder
				.WithSpecializationCode("123")
				.WithSpecializationName("testing")
				.Build();

			Assert.NotNull(sp);
			Assert.Equal("123", sp.Id.AsString());
			Assert.Equal("testing", sp.SpecializationName);
		}

		[Fact]
		public void Test_SpecializationBuilderSuccessNoCode()
		{
			SpecializationBuilder builder = new();

			Specialization sp = builder
				.WithSpecializationDescription("testing testing")
				.WithSpecializationName("testing")
				.Build();

			Assert.NotNull(sp);
			Assert.Equal("testing", sp.SpecializationName);
		}

		[Fact]
		public void Test_SpecializationBuilderUnsuccessful()
		{
			SpecializationBuilder builder = new();

			var exception = Assert.Throws<ArgumentException>(() => builder
				.WithSpecializationDescription("testing testing")
				.WithSpecializationCode("123")
				.Build());
				
			Assert.Equal("Specialization Name cannot be null or empty", exception.Message);
		}

		[Fact]
		public void Test_SpecializationChangeName()
		{
			string old = _spec.SpecializationName;

			_spec.ChangeName("newName");

			Assert.NotEqual(old, _spec.SpecializationName);
			Assert.Equal("newName", _spec.SpecializationName);
		}

		[Fact]
		public void Test_SpecializationChangeDescription()
		{
			string old = _spec.SpecializationDescription;

			_spec.ChangeDescription("new description");

			Assert.NotEqual(old, _spec.SpecializationDescription);
			Assert.Equal("new description", _spec.SpecializationDescription);
		}
	}
}