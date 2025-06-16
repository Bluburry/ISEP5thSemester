using System;
using DDDSample1.Domain.OperationRoomTypes;
using DDDSample1.Domain.ValueObjects;
using DDDSample1.DTO;
using Microsoft.AspNetCore.JsonPatch.Operations;

namespace DDDSample1.Domain.Factories
{
    public static class OperationRoomTypeFactory
    {
        public static OperationRoomType Create(OpRoomTypeDto dto)
        {
            if (string.IsNullOrEmpty(dto.OpCode))
                throw new ArgumentException("Operation room type id cannot be null or empty.", nameof(dto.OpCode));
            if (string.IsNullOrEmpty(dto.Name))
                throw new ArgumentException("Operation room name cannot be null or empty.", nameof(dto.Name));

            var operationRoomName = new OperationRoomName(dto.Name);
            var opRoomTypeId = new OperationRoomTypeId(dto.OpCode);
            var opRoomTypeDescription = new OperationRoomTypeDescription(dto.Description);
            return new OperationRoomType(opRoomTypeId, operationRoomName, opRoomTypeDescription);
        }
    }
}