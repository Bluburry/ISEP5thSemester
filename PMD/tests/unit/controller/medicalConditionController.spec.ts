/* eslint-disable @typescript-eslint/no-explicit-any */
/* eslint-disable @typescript-eslint/no-unused-vars */
/* eslint-disable prettier/prettier */
import 'reflect-metadata';
import { expect } from 'chai';
import { Request, Response, NextFunction } from 'express';
import { Container } from 'typedi';
import * as sinon from 'sinon';
import { describe, it } from 'mocha';
import { Result } from '../../../src/core/logic/Result';
import IMedicalConditionDTO from '../../../src/dto/IMedicalConditionDTO';
import IMedicalConditionQueryDto from '../../../src/dto/IMedicalConditionQueryDto';
import IMedicalConditionService from '../../../src/services/IServices/IMedicalConditionService';
import { RSADecryptionService } from '../../../src/services/RSADecryptionService';
import MedicalConditionController from '../../../src/controllers/medicalConditionController';

describe('MedicalConditionController', () => {
  let medicalConditionService: IMedicalConditionService;
  let medicalConditionController: MedicalConditionController;
  let req: Partial<Request>;
  let res: Partial<Response>;
  let next: NextFunction;

  beforeEach(() => {
    medicalConditionService = {
      createCondition: async () => Result.ok<IMedicalConditionDTO>({} as IMedicalConditionDTO),
      searchCondition: async () => Result.ok<IMedicalConditionDTO[]>([]),
    } as unknown as IMedicalConditionService;

    Container.set('conditionServiceInstance', medicalConditionService);
    medicalConditionController = new MedicalConditionController(medicalConditionService);

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

    next = () => {};
  });

  afterEach(() => {
    Container.reset();
  });

  describe('createCondition', () => {
    it('should create a valid MedicalCondition', async () => {
      const conditionDTO: IMedicalConditionDTO = {
        code: 123456,
        designation: 'Hypertension',
        description: 'High blood pressure',
        symptoms: 'Headache, dizziness',
      };

      req.body = conditionDTO;
      const token = 'valid-token';

      const rsaDecryptStub = sinon.stub(RSADecryptionService.prototype, 'decrypt').returns(JSON.stringify({ TokenValue: 'ADMIN_AUTH_TOKEN' }));

      const result = await medicalConditionController.createCondition(token, req as Request, res as Response, next);

      expect(result.statusCode).to.equal(201);

      rsaDecryptStub.restore();
    });

    it('should return 201 if token is provided', async () => {
      const token = 'prA58za/H4wbmm0VjWvWR2qDpDnm+VoNm2DHF1+Gav70HqPtJz9HSqp/0ugOjub60fU77+x9S2bLMNBKS71GCAMNpkD0vr+bzToIaRqATP0toIz6jNov45Nnb4zMJcqTdrT3FSKytTe2H9aNQLCxtiRBLBu5ymPMT34bnIZRwFEqhgy/0ScmE7LD+SADWk1F1naDZLOgDNOSrlIbUb8ae5PyKPnNiJAabgepQp52YxTj3NzIZ3BMzb/zSdGmKM2YlesZl5QuYyLK5INqQ38NsZEBLqXBo6OxkQeixvQ2hqz14GQHnu397D0DC5Bxc8x6z2Dj3Vr9UK/ujvoWT46Aew==';

      const result = await medicalConditionController.createCondition(token, req as Request, res as Response, next);

      expect(result.statusCode).to.equal(201);
    });

    it('should return 401 if unauthorized', async () => {
      const token = 'invalid-token';

      const rsaDecryptStub = sinon.stub(RSADecryptionService.prototype, 'decrypt').returns(JSON.stringify({ TokenValue: 'INVALID_AUTH_TOKEN' }));

      const result = await medicalConditionController.createCondition(token, req as Request, res as Response, next);

      expect(result.statusCode).to.equal(401);

      rsaDecryptStub.restore();
    });

    it('should return 400 if MedicalCondition creation fails', async () => {
      const conditionDTO: IMedicalConditionDTO = {
        code: 123,
        designation: '',
        description: 'High blood pressure',
        symptoms: 'Headache, dizziness',
      };

      req.body = conditionDTO;
      const token = 'valid-token';

      const rsaDecryptStub = sinon.stub(RSADecryptionService.prototype, 'decrypt').returns(JSON.stringify({ TokenValue: 'ADMIN_AUTH_TOKEN' }));
      sinon.stub(medicalConditionService, 'createCondition').resolves(Result.fail<IMedicalConditionDTO>('Must provide a medical condition name'));

      const result = await medicalConditionController.createCondition(token, req as Request, res as Response, next);

      expect(result.statusCode).to.equal(400);

      rsaDecryptStub.restore();
    });
  });

  describe('searchCondition', () => {
    it('should return a list of MedicalConditionDTOs', async () => {
      const queryDto: IMedicalConditionQueryDto = {
        designation: 'Hypertension',
        code: '123456',
        symptoms: 'High blood pressure',
      };

      req.query = queryDto as any;
      const token = 'valid-token';

      const conditionDTO: IMedicalConditionDTO = {
        code: 123456,
        designation: 'Hypertension',
        description: 'High blood pressure',
        symptoms: 'Headache, dizziness',
      };

      const rsaDecryptStub = sinon.stub(RSADecryptionService.prototype, 'decrypt').returns(JSON.stringify({ TokenValue: 'ADMIN_AUTH_TOKEN' }));
      sinon.stub(medicalConditionService, 'searchCondition').resolves(Result.ok<IMedicalConditionDTO[]>([conditionDTO]));

      const result = await medicalConditionController.searchCondition(token, req as Request, res as Response, next);

      expect(result.statusCode).to.equal(200);

      rsaDecryptStub.restore();
    });



    it('should return 401 if unauthorized', async () => {
      const token = 'invalid-token';

      const rsaDecryptStub = sinon.stub(RSADecryptionService.prototype, 'decrypt').returns(JSON.stringify({ TokenValue: 'INVALID_AUTH_TOKEN' }));

      const result = await medicalConditionController.searchCondition(token, req as Request, res as Response, next);

      expect(result.statusCode).to.equal(401);
        
      rsaDecryptStub.restore();
    });

    it('should return 500 if an error occurs', async () => {
      const queryDto: IMedicalConditionQueryDto = {
        designation: 'Hypertension',
        code: '123456',
        symptoms: 'High blood pressure',
      };

      req.query = queryDto as any;
      const token = 'valid-token';

      const rsaDecryptStub = sinon.stub(RSADecryptionService.prototype, 'decrypt').returns(JSON.stringify({ TokenValue: 'ADMIN_AUTH_TOKEN' }));
      sinon.stub(medicalConditionService, 'searchCondition').rejects(new Error('Database error'));

      const result = await medicalConditionController.searchCondition(token, req as Request, res as Response, next);

      expect(result.statusCode).to.equal(500);

      rsaDecryptStub.restore();
    });
  });
});