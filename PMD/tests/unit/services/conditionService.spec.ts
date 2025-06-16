/* eslint-disable prettier/prettier */
import 'reflect-metadata';
import { expect } from 'chai';
import { Container } from 'typedi';
import { MedicalCondition } from '../../../src/domain/MedicalCondition';
import IMedicalConditionDTO from '../../../src/dto/IMedicalConditionDTO';
import IMedicalConditionQueryDto from '../../../src/dto/IMedicalConditionQueryDto';
import IConditionRepo from '../../../src/services/IRepos/IConditionRepo';
import ConditionService from '../../../src/services/conditionService';

describe('ConditionService', () => {
  let conditionRepo: IConditionRepo;
  let conditionService: ConditionService;

  beforeEach(() => {
    conditionRepo = {
      save: async () => undefined,
      find: async () => [],
      exists: async () => false,
      findByDomainId: async () => undefined,
      getHighestId: async () => 0,
    } as unknown as IConditionRepo;

    Container.set('conditionRepo', conditionRepo);
    conditionService = new ConditionService(conditionRepo);
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

      const condition = MedicalCondition.create(conditionDTO, conditionDTO.code).getValue();
      conditionRepo.save = async () => condition;

      const result = await conditionService.createCondition(conditionDTO);

      expect(result.isSuccess).to.be.true;
      expect(result.getValue()).to.deep.equal(conditionDTO);
    });

    it('should fail if code is not provided', async () => {
      const conditionDTO: IMedicalConditionDTO = {
        code: null,
        designation: 'Hypertension',
        description: 'High blood pressure',
        symptoms: 'Headache, dizziness',
      };

      const result = await conditionService.createCondition(conditionDTO);

      expect(result.isFailure).to.be.true;
      expect(result.errorValue()).to.equal('Code must be provided.');
    });

    it('should fail if MedicalCondition creation fails', async () => {
      const conditionDTO: IMedicalConditionDTO = {
        code: 123,
        designation: '',
        description: 'High blood pressure',
        symptoms: 'Headache, dizziness',
      };

      const result = await conditionService.createCondition(conditionDTO);

      expect(result.isFailure).to.be.true;
      expect(result.errorValue()).to.equal('Must provide a medical condition name');
    });
  });

  describe('searchCondition', () => {
    it('should return a list of MedicalConditionDTOs', async () => {
      const queryDto: IMedicalConditionQueryDto = {
        designation: 'Hypertension',
        code: '123456',
        symptoms: 'High blood pressure',
      };

      const conditionDTO: IMedicalConditionDTO = {
        code: 123456,
        designation: 'Hypertension',
        description: 'High blood pressure',
        symptoms: 'Headache, dizziness',
      };

      const condition = MedicalCondition.create(conditionDTO, conditionDTO.code).getValue();
      conditionRepo.find = async () => [condition];

      const result = await conditionService.searchCondition(queryDto);

      expect(result.isSuccess).to.be.true;
      expect(result.getValue()).to.deep.equal([conditionDTO]);
    });

    it('should fail if an error occurs', async () => {
      const queryDto: IMedicalConditionQueryDto = {
        designation: 'Hypertension',
        code: '123456',
        symptoms: 'High blood pressure',
      };

      conditionRepo.find = async () => { throw new Error('Database error'); };

      const result = await conditionService.searchCondition(queryDto);

      expect(result.isFailure).to.be.true;
      expect(result.errorValue()).to.equal('Database error');
    });
  });
});