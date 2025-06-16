using DDDSample1.Domain.HospitalPatient;
using DDDSample1.Domain.HospitalStaff;
using DDDSample1.Domain.Shared;
using DDDSample1.Domain.ValueObjects;

namespace DDDSample1.Domain.HospitalAppointment 
{
    public class AppointmentDto
    {
        public string id { get; set;}
        public string dateAndTime { get; set;}
        public string appoitmentStatus { get; set;}
        public string staffId {get; set;}
        public string patientNumber {get; set;}

        public string operationRoom {get; set;}
        
        public string OperationRequestId {get; set;}

        public AppointmentDto()
        {
        }

        public AppointmentDto(string id, string dateAndTime, string status, string staffId, string patientId, string operationRoom, string request)
        {
            this.id = id;
            this.dateAndTime = dateAndTime;
            this.appoitmentStatus = status;
            this.staffId = staffId;
            this.patientNumber = patientId;
            this.operationRoom = operationRoom;
            this.OperationRequestId = request;
            // Remember to add Cirurgy Information
        }
        public override string ToString()
        {
            return $"Id: {id}, DateAndTime: {dateAndTime}, AppoitmentStatus: {appoitmentStatus}, " +
                   $"StaffId: {staffId}, PatientNumber: {patientNumber}, OperationRoom: {operationRoom}";}
    }
}
