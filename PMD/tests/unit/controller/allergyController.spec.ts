import { expect } from 'chai';
import sinon, { SinonStubbedInstance } from 'sinon';
import { describe, it } from 'mocha';
import { Request, Response, NextFunction } from 'express';
import AllergyController from '../../../src/controllers/allergyController';
import IAllergyService from '../../../src/services/IServices/IAllergyService';
import { Result } from "../../../src/core/logic/Result";
import { RSADecryptionService } from '../../../src/services/RSADecryptionService';
import IAllergyDTO from '../../../src/dto/IAllergyDTO';

describe('AllergyController', () => {
  let controller: AllergyController;
  let mockService: sinon.SinonStubbedInstance<IAllergyService>;
  let mockRequest: Partial<Request>;
  let mockResponse: SinonStubbedInstance<Response>;
  let mockNext: NextFunction;

  beforeEach(() => {
    mockService = sinon.createStubInstance<IAllergyService>(Object);
    controller = new AllergyController(mockService as any);
  
    // Properly mock the Response methods
    mockRequest = { body: {}, query: {}, headers: {} };
    mockResponse = {
      status: sinon.stub().returnsThis(),
      send: sinon.stub().returnsThis(),
      json: sinon.stub().returnsThis(),
    } as unknown as SinonStubbedInstance<Response>; // Type assertion to make it compatible with Sinon
  
    mockNext = sinon.stub();
  
    // Mock service methods
    mockService.patchAllergies = sinon.stub();
    mockService.createAllergy = sinon.stub();
    mockService.queryAllergies = sinon.stub();
  
    sinon.stub(RSADecryptionService.prototype, 'decrypt').returns(JSON.stringify({ TokenValue: 'ADMIN_AUTH_TOKEN' }));
  });
  
  

  afterEach(() => {
    sinon.restore();
  });

  describe('patchAllergies', () => {
    it('should return 400 if token is missing', async () => {
      await controller.patchAllergies(null as any, '123', mockRequest as Request, mockResponse as Response, mockNext);
      sinon.assert.calledWith(mockResponse.status, 400);
      sinon.assert.calledWith(mockResponse.send, { message: 'No token in header' });
    });

    it('should return 401 if token is unauthorized', async () => {
      sinon.restore();
      sinon.stub(RSADecryptionService.prototype, 'decrypt').returns(JSON.stringify({ TokenValue: 'USER_AUTH_TOKEN' }));
      await controller.patchAllergies('token', '123', mockRequest as Request, mockResponse as Response, mockNext);
      sinon.assert.calledWith(mockResponse.status, 401);
      sinon.assert.calledWith(mockResponse.send, { message: 'Unauthorized' });
    });

    it('should return 400 if service fails', async () => {
        // Mock service to return a failure result
        mockService.patchAllergies.resolves(Result.fail<IAllergyDTO>('Service error'));
    
        // Call the controller method
        await controller.patchAllergies('token', '123', mockRequest as Request, mockResponse as Response, mockNext);
    
        // Assert that the response is a 400 and the correct error message is sent
        sinon.assert.calledWith(mockResponse.status, 400);
        sinon.assert.calledWith(mockResponse.send, { message: 'Service error' });
      });
    
      it('should return 201 if service succeeds', async () => {
        // Create a fake allergy object
        const fakeAllergy: IAllergyDTO = { id: '123', name: 'Peanut', description: 'Throat Close' };
    
        // Mock service to return a successful result with the fake allergy
        mockService.patchAllergies.resolves(Result.ok(fakeAllergy));
    
        // Call the controller method
        await controller.patchAllergies('token', '123', mockRequest as Request, mockResponse as Response, mockNext);
    
        // Assert that the response is a 201 and the correct allergy data is returned
        sinon.assert.calledWith(mockResponse.status, 201);
        sinon.assert.calledWith(mockResponse.json, fakeAllergy);
      });
    });

  describe('createAllergy', () => {
    it('should return 400 if token is missing', async () => {
      await controller.createAllergy(null as any, mockRequest as Request, mockResponse as Response, mockNext);
      sinon.assert.calledWith(mockResponse.status, 400);
      sinon.assert.calledWith(mockResponse.send, { message: 'No token in header' });
    });

    it('should return 401 if token is unauthorized', async () => {
      sinon.restore();
      sinon.stub(RSADecryptionService.prototype, 'decrypt').returns(JSON.stringify({ TokenValue: 'USER_AUTH_TOKEN' }));
      await controller.createAllergy('token', mockRequest as Request, mockResponse as Response, mockNext);
      sinon.assert.calledWith(mockResponse.status, 401);
      sinon.assert.calledWith(mockResponse.send, { message: 'Unauthorized' });
    });

    it('should return 400 if service fails', async () => {
        // Mock service to return a failure result
        mockService.createAllergy.resolves(Result.fail<IAllergyDTO>('Service error'));
    
        // Call the controller method
        await controller.createAllergy('token', mockRequest as Request, mockResponse as Response, mockNext);
    
        // Assert that the response is a 400 and the correct error message is sent
        sinon.assert.calledWith(mockResponse.status, 400);
        sinon.assert.calledWith(mockResponse.send, { message: 'Service error' });
      });
    
      it('should return 201 if service succeeds', async () => {
        // Create a fake allergy object
        const fakeAllergy: IAllergyDTO = { id: '123', name: 'Peanut', description: 'Throat Close' };
    
        // Mock service to return a successful result with the fake allergy
        mockService.createAllergy.resolves(Result.ok(fakeAllergy));
    
        // Call the controller method
        await controller.createAllergy('token', mockRequest as Request, mockResponse as Response, mockNext);
    
        // Assert that the response is a 201 and the correct allergy data is returned
        sinon.assert.calledWith(mockResponse.status, 201);
        sinon.assert.calledWith(mockResponse.json, fakeAllergy);
      });
  });

  describe('queryAllergies', () => {
    it('should return 400 if token is missing', async () => {
      await controller.queryAllergies(null as any, mockRequest as Request, mockResponse as Response, mockNext);
      sinon.assert.calledWith(mockResponse.status, 400);
      sinon.assert.calledWith(mockResponse.send, { message: 'No token in header' });
    });

    it('should return 401 if token is unauthorized', async () => {
      sinon.restore();
      sinon.stub(RSADecryptionService.prototype, 'decrypt').returns(JSON.stringify({ TokenValue: 'USER_AUTH_TOKEN' }));
      await controller.queryAllergies('token', mockRequest as Request, mockResponse as Response, mockNext);
      sinon.assert.calledWith(mockResponse.status, 401);
      sinon.assert.calledWith(mockResponse.send, { message: 'Unauthorized' });
    });

    it('should return 400 if service fails', async () => {
        // Mock service to return a failure result
        mockService.queryAllergies.resolves(Result.fail<IAllergyDTO[]>('Service error'));
    
        // Call the controller method
        await controller.queryAllergies('token', mockRequest as Request, mockResponse as Response, mockNext);
    
        // Assert that the response is a 400 and the correct error message is sent
        sinon.assert.calledWith(mockResponse.status, 400);
        sinon.assert.calledWith(mockResponse.send, { message: 'Service error' });
      });
    
      it('should return 200 if service succeeds', async () => {
        // Create a fake allergy object
        const fakeAllergy: IAllergyDTO = { id: '123', name: 'Peanut', description: 'Throat Close' };
    
        // Mock service to return a successful result with the fake allergy
        mockService.queryAllergies.resolves(Result.ok([fakeAllergy]));
    
        // Call the controller method
        await controller.queryAllergies('token', mockRequest as Request, mockResponse as Response, mockNext);
    
        // Assert that the response is a 200 and the correct allergy data is returned
        sinon.assert.calledWith(mockResponse.status, 200);
        sinon.assert.calledWith(mockResponse.json, [fakeAllergy]);
      });
  });
});
