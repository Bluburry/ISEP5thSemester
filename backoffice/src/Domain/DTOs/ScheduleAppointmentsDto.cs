namespace DDDSample1.DTO
{
    public class ScheduleAppointmentsDto
    {
        // Property to hold the user's email address for password reset
        public string[] operationRooms { get; set; }

        // Constructor to initialize the email property
        public string[] mrnList {get; set;}

        public string[] opCodes {get; set;}

        public string day {get; set;}


        public ScheduleAppointmentsDto(string[] opRooms, string[] mrns, string[] opCodes, string day){
            this.operationRooms = opRooms;
            this.mrnList = mrns;
            this.opCodes = opCodes;
            this.day=day;
        }
    }
}
