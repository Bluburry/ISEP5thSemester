using System;
using System.Collections.Generic;
using System.Linq;
using DDDSample1.Domain.HospitalPatient;
using DDDSample1.Domain.HospitalStaff;
using DDDSample1.Domain.OperationRequests;
using DDDSample1.Domain.OperationRooms;
using DDDSample1.Domain.Shared;
using DDDSample1.Domain.ValueObjects;
using DDDSample1.Domain.AssignedStaffs;

namespace DDDSample1.Domain.HospitalAppointment 
{
    public class Appointment : Entity<AppointmentID>, IAggregateRoot
    {
        public DateAndTime dateAndTime { get; set;}
        public AppointmentStatus appoitmentStatus { get; set;}
        public List<AssignedStaff> designedStaff {get; set;}
        public Patient patient {get; set;}
        public MedicalRecordNumber patiendID {get; set;}
        public OperationRequest request {get; set;}
        public OperationRequestId requestId {get; set;}

        public OperationRoomId OpRoomId {get;set;}
        public OperationRoom OperationRoom {get; set;}


        private Appointment()
        {
        }

        public Appointment(DateAndTime dateAndTime, AppointmentStatus status, List<AssignedStaff> staff, Patient patient, OperationRequest operationRequest, OperationRoom room)
        {
            this.dateAndTime = dateAndTime;
            this.appoitmentStatus = status;
            this.designedStaff = staff;
            this.patient = patient;
            this.patiendID = patient.Id;
            this.Id = new AppointmentID(Guid.NewGuid());
            this.request = operationRequest;
            this.OperationRoom = room;
        }

        public AppointmentDto toDto()
        {
            string staffassigned = "";
            if(designedStaff.Count != 0){
                staffassigned = string.Join(",", designedStaff.Select(s => s.Id.Value));
            
                return new AppointmentDto
                {
                    id = this.Id.AsString(),
                    dateAndTime = this.dateAndTime.ToString(),
                    appoitmentStatus = this.appoitmentStatus.ToString(),
                    staffId = staffassigned,
                    patientNumber = this.patient.Id.AsString(),
                    operationRoom = this.OperationRoom.Name,
                    OperationRequestId = this.request.Id.Value
                    // Remember to add Cirurgy Information
                };
            }else{
                //String a = this.dateAndTime.ToString();

                //return new AppointmentDto(this.Id.AsString(), "2025-01-05 09:00", "0", "No Staff", this.patient.Id.Value, this.OperationRoom.Name, this.requestId.Value);

                return new AppointmentDto
                {
                    id = this.Id.AsString(),
                    dateAndTime = this.dateAndTime.ToString(),
                    appoitmentStatus = "SCHEDULED",
                    staffId = "No Staff",
                    patientNumber = this.patient.Id.Value,
                    operationRoom = this.OperationRoom.Name,
                    OperationRequestId = this.request.Id.Value
                    // Remember to add Cirurgy Information
                };
            }
        }

        public void DeleteTeam(){
            this.designedStaff = new List<AssignedStaff>();
        }
        
        public void ChangeTeam(List<AssignedStaff> staff){
            this.designedStaff = new List<AssignedStaff>();
            
            foreach(AssignedStaff asgStaff in staff)
                if(asgStaff.staff != null) designedStaff.Add(asgStaff);
        }     
    
        public void ChangeRoom(OperationRoom room){
            this.OperationRoom = room;
            this.OpRoomId = room.Id;
        }

        public void ChangeDate(DateAndTime time){
            this.dateAndTime = time;
        }
    }
}
