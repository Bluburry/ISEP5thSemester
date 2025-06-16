/* eslint-disable @typescript-eslint/no-explicit-any */
// eslint-disable-next-line prettier/prettier
/* eslint-disable @typescript-eslint/no-unused-vars */
/* eslint-disable prettier/prettier */
import 'reflect-metadata';
import { expect } from 'chai';
import { Request, Response, NextFunction } from 'express';
import { Container } from 'typedi';
import * as sinon from 'sinon';
import { describe, it, beforeEach, afterEach } from 'mocha';
import ClinicalDetailsController from '../../../src/controllers/ClinicalDetailsController';
import IClinicalDetailsService from '../../../src/services/IServices/IClinicalDetailsService';
import { RSADecryptionService } from '../../../src/services/RSADecryptionService';

describe('ClinicalDetailsController', () => {
  let clinicalDetailsService: IClinicalDetailsService;
  let clinicalDetailsController: ClinicalDetailsController;
  let req: Partial<Request>;
  let res: Partial<Response>;
  let next: NextFunction;

  beforeEach(() => {
    clinicalDetailsService = {
      findByDomainId: async () => ({}),
      filterByValues: async () => ({}),
      save: async () => ({}),
      createBlank: async () => ({}),
    } as unknown as IClinicalDetailsService;

    Container.set('clinicalDetailsServiceInstance', clinicalDetailsService);
    clinicalDetailsController = new ClinicalDetailsController(clinicalDetailsService);

    req = {
      body: {},
      query: {},
    };

    res = {
      status: function (code: number) {
        this.statusCode = code;
        return this;
      },
      json: function (data: any) {
        this.data = data;
        return this;
      },
      send: function (data: any) {
        this.data = data;
        return this;
      },
    } as Partial<Response> & { statusCode?: number; data?: any };

    next = sinon.stub();
  });

  afterEach(() => {
    sinon.restore();
    Container.reset();
  });

  describe('getClinicalDetailsByMRN', () => {
    it('should return clinical details when authorized', async () => {
      const token = 'valid-token';
      const mrn = '12345';

      const rsaDecryptStub = sinon.stub(RSADecryptionService.prototype, 'decrypt').returns(JSON.stringify({ TokenValue: 'ADMIN_AUTH_TOKEN' }));
      const findByDomainIdStub = sinon.stub(clinicalDetailsService, 'findByDomainId').resolves({});

      const result = await clinicalDetailsController.getClinicalDetailsByMRN(token, mrn, res as Response, next);

      expect(result.statusCode).to.equal(201);
      sinon.assert.calledOnce(findByDomainIdStub);
      rsaDecryptStub.restore();
    });

    it('should return 401 if unauthorized', async () => {
      const token = 'invalid-token';
      const mrn = '12345';

      const rsaDecryptStub = sinon.stub(RSADecryptionService.prototype, 'decrypt').returns(JSON.stringify({ TokenValue: 'INVALID_AUTH_TOKEN' }));

      const result = await clinicalDetailsController.getClinicalDetailsByMRN(token, mrn, res as Response, next);

      expect(result.statusCode).to.equal(401);
      rsaDecryptStub.restore();
    });
  });

  describe('filterClinicalDetails', () => {
    it('should return filtered clinical details when authorized', async () => {
      const token = 'valid-token';
      const allergyID = 'A123';
      const medicalConditionID = 'M456';

      const rsaDecryptStub = sinon.stub(RSADecryptionService.prototype, 'decrypt').returns(JSON.stringify({ TokenValue: 'ADMIN_AUTH_TOKEN' }));
      const filterByValuesStub = sinon.stub(clinicalDetailsService, 'filterByValues').resolves({});

      const result = await clinicalDetailsController.filterClinicalDetails(token, allergyID, medicalConditionID, res as Response, next);

      expect(result.statusCode).to.equal(201);
      sinon.assert.calledOnce(filterByValuesStub);
      rsaDecryptStub.restore();
    });

    it('should return 401 if unauthorized', async () => {
      const token = 'invalid-token';
      const allergyID = 'A123';
      const medicalConditionID = 'M456';

      const rsaDecryptStub = sinon.stub(RSADecryptionService.prototype, 'decrypt').returns(JSON.stringify({ TokenValue: 'INVALID_AUTH_TOKEN' }));

      const result = await clinicalDetailsController.filterClinicalDetails(token, allergyID, medicalConditionID, res as Response, next);

      expect(result.statusCode).to.equal(401);
      rsaDecryptStub.restore();
    });
  });

  describe('saveClinicalDetails', () => {
    it('should save clinical details when authorized', async () => {
      const token = 'valid-token';
      req.body = { some: 'data' };

      const rsaDecryptStub = sinon.stub(RSADecryptionService.prototype, 'decrypt').returns(JSON.stringify({ TokenValue: 'ADMIN_AUTH_TOKEN' }));
      const saveStub = sinon.stub(clinicalDetailsService, 'save').resolves({});

      const result = await clinicalDetailsController.saveClinicalDetails(token, req as Request, res as Response, next);

      expect(result.statusCode).to.equal(201);
      sinon.assert.calledOnce(saveStub);
      rsaDecryptStub.restore();
    });

    it('should return 401 if unauthorized', async () => {
      const token = 'invalid-token';

      const rsaDecryptStub = sinon.stub(RSADecryptionService.prototype, 'decrypt').returns(JSON.stringify({ TokenValue: 'INVALID_AUTH_TOKEN' }));

      const result = await clinicalDetailsController.saveClinicalDetails(token, req as Request, res as Response, next);

      expect(result.statusCode).to.equal(401);
      rsaDecryptStub.restore();
    });
  });

  describe('createBlankClinicalDetails', () => {
    it('should create blank clinical details when authorized', async () => {
      const token = 'valid-token';
      const mrn = '12345';

      const rsaDecryptStub = sinon.stub(RSADecryptionService.prototype, 'decrypt').returns(JSON.stringify({ TokenValue: 'ADMIN_AUTH_TOKEN' }));
      const createBlankStub = sinon.stub(clinicalDetailsService, 'createBlank').resolves({});

      const result = await clinicalDetailsController.createBlankClinicalDetails(token, mrn, res as Response, next);

      expect(result.statusCode).to.equal(201);
      sinon.assert.calledOnce(createBlankStub);
      rsaDecryptStub.restore();
    });

    it('should return 401 if unauthorized', async () => {
      const token = 'invalid-token';
      const mrn = '12345';

      const rsaDecryptStub = sinon.stub(RSADecryptionService.prototype, 'decrypt').returns(JSON.stringify({ TokenValue: 'INVALID_AUTH_TOKEN' }));

      const result = await clinicalDetailsController.createBlankClinicalDetails(token, mrn, res as Response, next);

      expect(result.statusCode).to.equal(401);
      rsaDecryptStub.restore();
    });
  });
});
