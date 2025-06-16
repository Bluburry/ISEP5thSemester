namespace DDDSample1.DTO
{
    public class DeletePatientProfile
    {
        // Property to hold the username
        public string PatientId { get; set; }

        // Property to hold the password
        public string TokenId { get; set; }

        // Constructor to initialize the properties
        public DeletePatientProfile(string patientId, string tokenId)
        {
            PatientId = patientId;
            TokenId = tokenId;
        }
    }
}
