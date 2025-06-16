/* eslint-disable @typescript-eslint/no-explicit-any */
// eslint-disable-next-line prettier/prettier
/* eslint-disable @typescript-eslint/no-unused-vars */
/* eslint-disable prettier/prettier */
import 'reflect-metadata';
import { expect } from 'chai';
import { Container } from 'typedi';
import * as sinon from 'sinon';
import { describe, it, beforeEach, afterEach } from 'mocha';
import { Request, Response, NextFunction } from 'express';
import ClinicalDetailsService from '../../src/services/ClinicalDetailsService';
import ClinicalDetailsController from '../../src/controllers/ClinicalDetailsController';
import { RSADecryptionService } from '../../src/services/RSADecryptionService';
import { Allergy } from '../../src/domain/Allergy';
import { MedicalCondition } from '../../src/domain/MedicalCondition';
import { ClinicalDetails } from '../../src/domain/ClinicalDetails';
import { Result } from '../../src/core/logic/Result';
import { cli } from 'winston/lib/winston/config';


describe('ClinicalDetailsController and ClinicalDetailsService Integration', () => {
  let clinicalDetailsController: ClinicalDetailsController;
  let clinicalDetailsService: ClinicalDetailsService;
  let req: Partial<Request>;
  let res: Partial<Response>;
  let next: NextFunction;

  beforeEach(() => {
    // Mock repository and services
    const clinicalDetailsRepo = {
      save: async () => undefined,
      findByDomainId: async () => undefined,
      getAll: async () => [],
    };

    // Register mocked repo and service in the container
    Container.set('clinicalDetailsRepo', clinicalDetailsRepo);
    clinicalDetailsService = new ClinicalDetailsService(clinicalDetailsRepo as any);
    Container.set('clinicalDetailsServiceInstance', clinicalDetailsService);

    // Inject the service into the controller
    clinicalDetailsController = new ClinicalDetailsController(clinicalDetailsService);

    // Mock request and response
    req = {
      body: {},
      query: {},
    };

    res = {
        status: sinon.stub().returnsThis(),
        json: sinon.stub().returnsThis(),
        send: sinon.stub().returnsThis(),
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
        const mrn = '123456789';
      
        // Mock the RSADecryptionService
        const rsaDecryptStub = sinon.stub(RSADecryptionService.prototype, 'decrypt').returns(
          JSON.stringify({ TokenValue: 'ADMIN_AUTH_TOKEN' })
        );
      
        // Mock ClinicalDetailsService findByDomainId
        const allergy = Allergy.create({ id: '4A8Z.1', name: 'Peanut', description: 'Peanut allergy' }).getValue();
        const medicalCondition = MedicalCondition.create(
          { code: 1234567, designation: 'Hypertension', symptoms: 'High blood pressure' },
          1234567
        ).getValue();
        const clinicalDetails: Result<ClinicalDetails> = ClinicalDetails.createWithDetails(
          [allergy],
          [medicalCondition],
          mrn
        );
      
        sinon.stub(clinicalDetailsService, 'findByDomainId').resolves(clinicalDetails.getValue());
      
        // Stub createBlank to return a successful result
        const createBlankStub = sinon.stub(clinicalDetailsService, 'createBlank').resolves(
          ClinicalDetails.createBlank(mrn)
        );
      
        // Call the controller method
        const result = await clinicalDetailsController.getClinicalDetailsByMRN(token, mrn, res as Response, next);
      
        // Assertions
        sinon.assert.calledWith(res.json as sinon.SinonSpy, clinicalDetails.getValue());
      });
      
  });
});
