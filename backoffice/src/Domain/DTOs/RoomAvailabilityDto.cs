using System;


namespace DDDSample1.Application.AvailabilitySlots
{
    public class RoomAvailabilityDto
    {
        public string Value { get; set; } 
        public string OperationRoomName { get; set; } 
        public string OperationRoomID { get; set; }
    
        public RoomAvailabilityDto(){
        }

        public RoomAvailabilityDto(string value, string roomName, string roomID){
            this.Value = value;
            this.OperationRoomName = roomName;
            this.OperationRoomID = roomID;
        }

    }

}
