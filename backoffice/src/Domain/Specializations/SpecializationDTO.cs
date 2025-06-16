
namespace DDDSample1.Domain.Specializations
{

	public class SpecializationDTO
	{
		public string SpecializationCode{ get; set; }
		public string SpecializationName{get; set;}
		public string SpecializationDescription {get; set;}

		public SpecializationDTO(string code, string name, string description)
		{
			SpecializationCode = code;
			SpecializationName = name;
			SpecializationDescription = description;
		}

		public SpecializationDTO(string code, string name)
		{
			SpecializationCode = code;
			SpecializationName = name;
		}

		public SpecializationDTO() { }
	}
}