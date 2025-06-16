/* eslint-disable prettier/prettier */
/* eslint-disable @typescript-eslint/no-unused-vars */
/* eslint-disable prettier/prettier */
import "reflect-metadata";
import { it } from 'mocha';
import sinon from "sinon";
import { Container } from "typedi";
import { Request, Response, NextFunction } from 'express';
import AllergyController from "../../src/controllers/allergyController";
import { Result } from "../../src/core/logic/Result";
import IAllergyDTO from "../../src/dto/IAllergyDTO";
import AllergyService from "../../src/services/allergyService";
import { RSADecryptionService } from "../../src/services/RSADecryptionService";

describe('AllergyController Integration Tests', function () {
    const sandbox = sinon.createSandbox();
    this.timeout(5000);

    let controller: AllergyController;
    let allergyService: sinon.SinonStubbedInstance<AllergyService>;
    let req: Partial<Request>;
    let res: Partial<Response>;
    let next: sinon.SinonSpy;

    beforeEach(() => {
        // Create a mock instance of the AllergyService
        allergyService = sinon.createStubInstance(AllergyService);

        // Create the controller with the mocked service
        controller = new AllergyController(allergyService as any);

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

  describe('createAllergy', () => {
    it('should return 201 and the created allergy if token is valid', async () => {
      const allergyDTO: IAllergyDTO = {
        id: '123',
        name: 'Peanut',
        description: 'Peanut allergy',
      };

      req.body = allergyDTO;
      const validToken = 'valid-token';

      // Stub RSA decryption to simulate valid token decryption
      sinon.stub(RSADecryptionService.prototype, 'decrypt').returns(JSON.stringify({ TokenValue: 'ADMIN_AUTH_TOKEN' }));

      // Stub the service method
      allergyService.createAllergy.resolves(Result.ok(allergyDTO));

      // Call the controller method
      await controller.createAllergy(validToken, req as Request, res as Response, next);

      // Assertions
      sinon.assert.calledOnce(allergyService.createAllergy);
      sinon.assert.calledWith(res.json as sinon.SinonSpy, allergyDTO);
    });

    it('should return 500 if there is an error in the service', async () => {
      const allergyDTO: IAllergyDTO = {
        id: '123',
        name: 'Peanut',
        description: 'Peanut allergy',
      };

      req.body = allergyDTO;
      const validToken = 'valid-token';

      // Stub RSA decryption to simulate valid token decryption
      sinon.stub(RSADecryptionService.prototype, 'decrypt').returns(JSON.stringify({ TokenValue: 'ADMIN_AUTH_TOKEN' }));

      // Stub the service method to simulate failure
      allergyService.createAllergy.rejects(new Error('Database error'));

      // Call the controller method
      await controller.createAllergy(validToken, req as Request, res as Response, next);

      // Assertions
      sinon.assert.calledOnce(allergyService.createAllergy);
      sinon.assert.calledWith(res.status as sinon.SinonSpy, 500);
      sinon.assert.calledWith(res.send as sinon.SinonSpy, { message: 'Internal server error' });
    });
  });

  describe('queryAllergies', () => {
    it('should return 200 and a list of allergies if token is valid', async () => {
      const queryDto = { name: 'Peanut' };
      //req.query = queryDto;

      const validToken = 'valid-token';

      // Stub RSA decryption to simulate valid token decryption
      sinon.stub(RSADecryptionService.prototype, 'decrypt').returns(JSON.stringify({ TokenValue: 'ADMIN_AUTH_TOKEN' }));

      // Stub the service method
      const allergyDTO = {
        name: 'Peanut',
        description: 'Peanut allergy',
      };
      allergyService.queryAllergies.resolves(Result.ok([allergyDTO]));

      // Call the controller method
      await controller.queryAllergies(validToken, req as Request, res as Response, next);

      // Assertions
      sinon.assert.calledOnce(allergyService.queryAllergies);
      sinon.assert.calledWith(res.json as sinon.SinonSpy, [allergyDTO]);
    });

    it('should return 401 if token is invalid', async () => {
      const invalidToken = 'invalid-token';

      // Stub RSA decryption to simulate invalid token decryption
      sinon
        .stub(RSADecryptionService.prototype, 'decrypt')
        .returns(JSON.stringify({ TokenValue: 'INVALID_AUTH_TOKEN' }));

      // Call the controller method
      await controller.queryAllergies(invalidToken, req as Request, res as Response, next);

      // Assertions
      sinon.assert.calledOnce(res.status as sinon.SinonSpy);
      sinon.assert.calledWith(res.status as sinon.SinonSpy, 401);
      sinon.assert.calledWith(res.send as sinon.SinonSpy, { message: 'Unauthorized' });
    });

    it('should return 500 if there is an error in the service', async () => {
      const queryDto = { name: 'Peanut' };
      //req.query = queryDto;
      const validToken = 'valid-token';

      // Stub RSA decryption to simulate valid token decryption
      sinon.stub(RSADecryptionService.prototype, 'decrypt').returns(JSON.stringify({ TokenValue: 'ADMIN_AUTH_TOKEN' }));

      // Stub the service method to simulate failure
      allergyService.queryAllergies.rejects(new Error('Database error'));

      // Call the controller method
      await controller.queryAllergies(validToken, req as Request, res as Response, next);

      // Assertions
      sinon.assert.calledOnce(allergyService.queryAllergies);
      sinon.assert.calledWith(res.status as sinon.SinonSpy, 500);
      sinon.assert.calledWith(res.send as sinon.SinonSpy, { message: 'Internal server error' });
    });
  });

  describe('patchAllergies', () => {
    it('should return 200 and the updated allergy if token is valid', async () => {
      const allergyDTO: IAllergyDTO = {
        id: '123',
        name: 'Peanut',
        description: 'Updated description of peanut allergy',
      };

      req.body = allergyDTO;
      const validToken = 'valid-token';
      const allergyId = 'existing-allergy-id';

      // Stub RSA decryption to simulate valid token decryption
      sinon.stub(RSADecryptionService.prototype, 'decrypt').returns(JSON.stringify({ TokenValue: 'ADMIN_AUTH_TOKEN' }));

      // Stub the service method
      allergyService.patchAllergies.resolves(Result.ok(allergyDTO));

      // Call the controller method
      await controller.patchAllergies(validToken, allergyId, req as Request, res as Response, next);

      // Assertions
      sinon.assert.calledOnce(allergyService.patchAllergies);
      sinon.assert.calledWith(res.json as sinon.SinonSpy, allergyDTO);
    });

    it('should return 400 if allergy does not exist', async () => {
      const allergyDTO: IAllergyDTO = {
        id: '123',
        name: 'Peanut',
        description: 'Updated description of peanut allergy',
      };

      req.body = allergyDTO;
      const invalidAllergyId = 'non-existent-allergy-id';
      const validToken = 'valid-token';

      // Stub RSA decryption to simulate valid token decryption
      sinon.stub(RSADecryptionService.prototype, 'decrypt').returns(JSON.stringify({ TokenValue: 'ADMIN_AUTH_TOKEN' }));

      // Stub the service method to simulate failure
      allergyService.patchAllergies.resolves(Result.fail('Allergy not found'));

      // Call the controller method
      await controller.patchAllergies(validToken, invalidAllergyId, req as Request, res as Response, next);

      // Assertions
      sinon.assert.calledOnce(allergyService.patchAllergies);
      sinon.assert.calledWith(res.status as sinon.SinonSinSpy, 400);
      sinon.assert.calledWith(res.send as sinon.SinonSpy, { message: 'Allergy not found' });
    });

    it('should return 500 if there is an error in the service', async () => {
      const allergyDTO: IAllergyDTO = {
        id: '123',
        name: 'Peanut',
        description: 'Updated description of peanut allergy',
      };

      req.body = allergyDTO;
      const validToken = 'valid-token';
      const allergyId = 'existing-allergy-id';

      // Stub RSA decryption to simulate valid token decryption
      sinon.stub(RSADecryptionService.prototype, 'decrypt').returns(JSON.stringify({ TokenValue: 'ADMIN_AUTH_TOKEN' }));

      // Stub the service method to simulate failure
      allergyService.patchAllergies.rejects(new Error('Database error'));

      // Call the controller method
      await controller.patchAllergies(validToken, allergyId, req as Request, res as Response, next);

      // Assertions
      sinon.assert.calledOnce(allergyService.patchAllergies);
      sinon.assert.calledWith(res.status as sinon.SinonSinSpy, 500);
      sinon.assert.calledWith(res.send as sinon.SinonSinSpy, { message: 'Internal server error' });
    });
  });
});
