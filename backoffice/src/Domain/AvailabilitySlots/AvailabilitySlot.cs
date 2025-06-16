using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;
using DDDSample1.Application.AvailabilitySlots;
using DDDSample1.Domain.HospitalStaff;
using DDDSample1.Domain.OperationRooms;
using DDDSample1.Domain.Shared;

namespace DDDSample1.Domain.AvailabilitySlots
{
    public class AvailabilitySlot : Entity<AvailabilitySlotsId>, IAggregateRoot
    {
        
        public Staff theStaff { get; set; }
        public LicenseNumber StaffId { get; set; }
        public OperationRoom opRoom {get; set;}
        public OperationRoomId roomId {get; set;}
        public string Value { get; set;}

        public AvailabilitySlot(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Timeslot can not be empty.", nameof(value));

            this.Id = new AvailabilitySlotsId(Guid.NewGuid());
            Value = value;
        }

        public AvailabilitySlot()
        {
        }

        public override string ToString()
        {
            return Value;
        }

        public override bool Equals(object obj)
        {
            return obj is AvailabilitySlot AvailabilitySlot && Value.Equals(AvailabilitySlot.Value);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public AvailabilitySlotDto toDto(){
            return new AvailabilitySlotDto(this.Value);
        }

        
    }

    public class AvailabilitySlotDto
    {
        public string AvailabilitySlot { get; set; }

        public AvailabilitySlotDto(string value)
        {
            AvailabilitySlot = value; // Assign the value parameter to the property
        }
    }

}
