/* eslint-disable prettier/prettier */
/* eslint-disable @typescript-eslint/no-unused-vars */
/* eslint-disable prettier/prettier */
import "reflect-metadata";
import { it } from 'mocha';
import sinon from "sinon";
import { Container } from "typedi";
import { Request, Response, NextFunction } from 'express';
import MedicalConditionController from "../../src/controllers/medicalConditionController";
import { Result } from "../../src/core/logic/Result";
import IMedicalConditionDTO from "../../src/dto/IMedicalConditionDTO";
import ConditionService from "../../src/services/conditionService";
import { RSADecryptionService } from "../../src/services/RSADecryptionService";

describe('MedicalConditionController Unit Tests', function () {
  const sandbox = sinon.createSandbox();
  this.timeout(5000);

  let controller: MedicalConditionController;
  let conditionService: sinon.SinonStubbedInstance<ConditionService>;
  let req: Partial<Request>;
  let res: Partial<Response>;
  let next: sinon.SinonSpy;

  beforeEach(() => {
    // Create a mock instance of the ConditionService
    conditionService = sinon.createStubInstance(ConditionService);

    // Create the controller with the mocked service
    controller = new MedicalConditionController(conditionService as any);

    // Mock the req, res, and next objects
    req = {};
    res = {
      status: sinon.stub().returnsThis(),
      json: sinon.stub().returnsThis(),
      send: sinon.stub().returnsThis(),
    };
    next = sinon.spy();
  });

  afterEach(() => {
    sandbox.restore(); // Restore all stubs after each test
    sinon.restore();
  });

  describe('createCondition', () => {
    it('should return 201 and the created condition if token is valid', async () => {
      const conditionDTO: IMedicalConditionDTO = {
        code: 123456,
        designation: 'Hypertension',
        description: 'High blood pressure',
        symptoms: 'Headache, dizziness',
      };

      req.body = conditionDTO;
      const validToken = 'valid-token';

      // Stub RSA decryption to simulate valid token decryption
      sinon.stub(RSADecryptionService.prototype, 'decrypt').returns(JSON.stringify({ TokenValue: 'ADMIN_AUTH_TOKEN' }));

      // Stub the service method
      conditionService.createCondition.resolves(Result.ok(conditionDTO));

      // Call the controller method
      await controller.createCondition(validToken, req as Request, res as Response, next);

      // Assertions
      sinon.assert.calledOnce(conditionService.createCondition);
      sinon.assert.calledWith(res.json as sinon.SinonSpy, conditionDTO);
    });

    it('should return 500 if there is an error in the service', async () => {
      const conditionDTO: IMedicalConditionDTO = {
        code: 123456,
        designation: 'Hypertension',
        description: 'High blood pressure',
        symptoms: 'Headache, dizziness',
      };

      req.body = conditionDTO;
      const validToken = 'valid-token';

      // Stub RSA decryption to simulate valid token decryption
      sinon.stub(RSADecryptionService.prototype, 'decrypt').returns(JSON.stringify({ TokenValue: 'ADMIN_AUTH_TOKEN' }));

      // Stub the service method to simulate failure
      conditionService.createCondition.rejects(new Error('Database error'));

      // Call the controller method
      await controller.createCondition(validToken, req as Request, res as Response, next);

      // Assertions
      sinon.assert.calledOnce(conditionService.createCondition);
      sinon.assert.calledWith(res.status as sinon.SinonSpy, 500);
      sinon.assert.calledWith(res.send as sinon.SinonSpy, { message: 'Internal server error' });
    });
  });

  describe('searchCondition', () => {
    it('should return 200 and a list of conditions if token is valid', async () => {
      const queryDto = { code: 123456 };
      //req.query = queryDto;

      const validToken = 'valid-token';

      // Stub RSA decryption to simulate valid token decryption
      sinon.stub(RSADecryptionService.prototype, 'decrypt').returns(JSON.stringify({ TokenValue: 'ADMIN_AUTH_TOKEN' }));

      // Stub the service method
      const conditionDTO = {
        code: 123456,
        designation: 'Hypertension',
        description: 'High blood pressure',
        symptoms: 'Headache',
      };
      conditionService.searchCondition.resolves(Result.ok([conditionDTO]));

      // Call the controller method
      await controller.searchCondition(validToken, req as Request, res as Response, next);

      // Assertions
      sinon.assert.calledOnce(conditionService.searchCondition);
      sinon.assert.calledWith(res.json as sinon.SinonSpy, [conditionDTO]);
    });



    it('should return 401 if token is invalid', async () => {
      const invalidToken = 'invalid-token';

      // Stub RSA decryption to simulate invalid token decryption
      sinon
        .stub(RSADecryptionService.prototype, 'decrypt')
        .returns(JSON.stringify({ TokenValue: 'INVALID_AUTH_TOKEN' }));

      // Call the controller method
      await controller.searchCondition(invalidToken, req as Request, res as Response, next);

      // Assertions
      sinon.assert.calledOnce(res.status as sinon.SinonSpy);
      sinon.assert.calledWith(res.status as sinon.SinonSpy, 401);
      sinon.assert.calledWith(res.send as sinon.SinonSpy, { message: 'Unauthorized' });
    });

    it('should return 500 if there is an error in the service', async () => {
      const queryDto = { code: 123456 };
      //req.query = queryDto;
      const validToken = 'valid-token';

      // Stub RSA decryption to simulate valid token decryption
      sinon.stub(RSADecryptionService.prototype, 'decrypt').returns(JSON.stringify({ TokenValue: 'ADMIN_AUTH_TOKEN' }));

      // Stub the service method to simulate failure
      conditionService.searchCondition.rejects(new Error('Database error'));

      // Call the controller method
      await controller.searchCondition(validToken, req as Request, res as Response, next);

      // Assertions
      sinon.assert.calledOnce(conditionService.searchCondition);
      sinon.assert.calledWith(res.status as sinon.SinonSpy, 500);
      sinon.assert.calledWith(res.send as sinon.SinonSpy, { message: 'Internal server error' });
    });
  });
});

