using System;
using System.Collections.Generic;
using System.Linq;
using DDDSample1.Domain.AvailabilitySlots;
using DDDSample1.Domain.OperationRoomTypes;
using DDDSample1.Domain.Shared;
using DDDSample1.Domain.ValueObjects;

namespace DDDSample1.Domain.OperationRooms
{
    public class OperationRoom : Entity<OperationRoomId>, IAggregateRoot
    {
        public string Name { get; set; }
        public OperationRoomTypeId OperationRoomTypeId { get; set; }

        public OperationRoomType OperationRoomType { get; set; }  
        
        public List<AvailabilitySlot> AvailabilitySlots { get; set; }

        private OperationRoom()
        {
            // Parameterless constructor for ORM
        }

        public OperationRoom(string name, OperationRoomType roomType, List<AvailabilitySlot> availabilitySlots)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("Operation room name cannot be null or empty.", nameof(name));

            this.Id = new OperationRoomId(Guid.NewGuid());
            this.Name = name;
            this.OperationRoomType = roomType;
            this.OperationRoomTypeId = roomType.Id;
            this.AvailabilitySlots = availabilitySlots ?? new List<AvailabilitySlot>();
        }

        public void AddAvailabilitySlot(AvailabilitySlot slot)
        {
            if (slot == null)
                throw new ArgumentException("Availability slot cannot be null.", nameof(slot));

            this.AvailabilitySlots.Add(slot);
        }

        public void RemoveAvailabilitySlot(AvailabilitySlot slot)
        {
            if (slot == null)
                throw new ArgumentException("Availability slot cannot be null.", nameof(slot));

            this.AvailabilitySlots.Remove(slot);
        }

        public OperationRoomDto ToDto()
        {
            return new OperationRoomDto
            {
                Id = this.Id.AsString(),
                Name = this.Name,
                AvailabilitySlots = this.AvailabilitySlots.ConvertAll(s => s.toDto())
            };
        }
    }

    public class OperationRoomDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<AvailabilitySlotDto> AvailabilitySlots { get; set; }
    }

    
}
