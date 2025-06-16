namespace DDDSample1.Domain.Logs
{
    public class LogDto
    {
        public string LoggedId { get; set; }
        public string LoggedInformation { get; set; }
        public string LoggedType { get; set; }
        public string LoggedDate { get; set; }

        public LogDto() { }

        public LogDto(string loggedId, string loggedInformation, string loggedType, string loggedDate)
        {
            LoggedId = loggedId;
            LoggedInformation = loggedInformation;
            LoggedType = loggedType;
            LoggedDate = loggedDate;
        }
    }
}
