using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DDDSample1.Domain.Shared;
using DDDSample1.DTO;
using DDDSample1.Domain.ValueObjects;
using DDDSample1.Domain.OperationRoomTypes;
using OperationRoomTypeDto = DDDSample1.Domain.OperationRoomTypes.OperationRoomType.OperationRoomTypeDto;
using DDDSample1.Domain.Factories;

namespace DDDSample1.AppServices.OperationRoomTypes
{
    public class OperationRoomTypeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOperationRoomTypeRepository _operationRoomTypeRepo;

        public OperationRoomTypeService(IUnitOfWork unitOfWork, IOperationRoomTypeRepository operationRoomTypeRepo)
        {
            _unitOfWork = unitOfWork;
            _operationRoomTypeRepo = operationRoomTypeRepo;
        }

        public OperationRoomTypeService() { }

        // Add a new operation room type
        public async Task<OperationRoomTypeDto> AddOperationRoomTypeAsync(OpRoomTypeDto dto)
        {

            OperationRoomType roomType = OperationRoomTypeFactory.Create(dto);
            await this._operationRoomTypeRepo.AddAsync(roomType);
            await _unitOfWork.CommitAsync();
            return roomType.ToDto();
        }

       
    }
}