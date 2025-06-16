namespace DDDSample1.DTO
{
    public class QueryDataDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string MedicalRecordNumber { get; set; }
        public string DateOfBirth { get; set; }
        public string Gender { get; set; }

        // STAFF

        public string LicenseNumber { get; set; }
        public string Specialization { get; set; }

        // DOCTOR

        public string OperationType { get; set; }
        public string Priority { get; set; }
        public string Status { get; set; }
    }
}
