using System;
using DDDSample1.Domain.Shared;
using DDDSample1.Domain.ValueObjects;

namespace DDDSample1.Domain.OperationRoomTypes
{
    public class OperationRoomType : Entity<OperationRoomTypeId>, IAggregateRoot
    {
        public OperationRoomName Name { get; private set; }

        public OperationRoomTypeDescription description {get; set;}

        private OperationRoomType()
        {
            // Parameterless constructor for ORM
        }

        public class OperationRoomTypeDto
        {
            public string Id { get; set; }
            public string Name { get; set; }

            public OperationRoomTypeDto ToDto(OperationRoomType operationRoomType)
            {
            return new OperationRoomTypeDto
            {
                Id = operationRoomType.Id.AsString(),
                Name = operationRoomType.Name.Value
            };
            }
        }
        

        public OperationRoomType(OperationRoomTypeId code, OperationRoomName name, OperationRoomTypeDescription description)
        {
            
            this.Id = code;
            this.Name = name;
            this.description = description;
        }

        public void ChangeName(OperationRoomName newName)
        {
            if (newName == null)
                throw new ArgumentException("New operation name cannot be null.", nameof(newName));

            this.Name = newName;
        }

        public OperationRoomTypeDto ToDto()
        {
            return new OperationRoomTypeDto
            {
                Id = this.Id.AsString(),
                Name = this.Name.Value
            };
        }
    }

    public class OperationRoomTypeDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}