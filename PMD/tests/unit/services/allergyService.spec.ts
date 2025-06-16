import { expect } from 'chai';
import sinon, { SinonStubbedInstance } from 'sinon';
import { describe, it } from 'mocha';
import AllergyService from '../../../src/services/allergyService';
import IAllergyRepo from '../../../src/services/IRepos/IAllergyRepo';
import { Result } from '../../../src/core/logic/Result';
import IAllergyDTO from '../../../src/dto/IAllergyDTO';
import { Allergy } from '../../../src/domain/Allergy';
import { AllergyMap } from '../../../src/mappers/AllergyMap';
import IAllergyQueryDto from '../../../src/dto/IAllergyQueryDto';

describe('AllergyService', () => {
  let service: AllergyService;
  let mockRepo: SinonStubbedInstance<IAllergyRepo>;

  beforeEach(() => {
    // Explicitly define the methods that will be stubbed
    mockRepo = {
        findByDomainId: sinon.stub(),
        save: sinon.stub(),
        getHighestId: sinon.stub(),
        patch: sinon.stub(),
        find: sinon.stub(),
      };      
    service = new AllergyService(mockRepo as any);
  });

  afterEach(() => {
    sinon.restore();
  });

  describe('createAllergy', () => {
    it('should return 201 if allergy is created successfully', async () => {
      const allergyDTO: IAllergyDTO = { id: '4A8Z.1', name: 'Peanut', description: 'Peanut allergy' };

      // Mock the repo's getHighestId and save methods
      mockRepo.getHighestId.resolves('4A8Z.1');
      mockRepo.save.resolves(); // Simply resolve the save method

      // Mock the creation and return a successful result
      const allergyInstance = Allergy.create(allergyDTO, '4A8Z.2');
      mockRepo.save.resolves(allergyInstance.getValue());

      const result = await service.createAllergy(allergyDTO);

      expect(result.isSuccess).to.be.true;
      expect(result.getValue()).to.deep.equal(AllergyMap.toDTO(allergyInstance.getValue()));
    });
  });

  describe('patchAllergies', () => {
    it('should return 400 if patch fails', async () => {
      const allergyDTO: IAllergyQueryDto = { name: 'Peanut Updated', code: 'A124', description: 'Updated description' };

      // Simulate a failed patch by rejecting with an error message
      mockRepo.patch.rejects(new Error('Patch failed'));

      const result = await service.patchAllergies('4A8Z.1', allergyDTO);

      expect(result.isFailure).to.be.true;
      expect(result.errorValue()).to.equal('Patch failed');
    });

    it('should return 200 if allergy is patched successfully', async () => {
      const allergyDTO: IAllergyQueryDto = { name: 'Peanut Updated', code: 'A124', description: 'Updated description' };

      // Simulate a successful patch by resolving with a successful allergy instance
      const updatedAllergy = Allergy.create({ id: '4A8Z.1', name: 'Peanut Updated', description: 'Updated description' }, '4A8Z.1');
      mockRepo.patch.resolves(updatedAllergy.getValue()); // Ensure it's a valid allergy instance


      const result = await service.patchAllergies('4A8Z.1', allergyDTO);

      expect(result.isSuccess).to.be.true;
      expect(result.getValue().name).to.equal('Peanut Updated');
    });
  });

  describe('queryAllergies', () => {
    it('should return 400 if query fails', async () => {
      const queryDto = { name: 'Peanut' };

      // Simulate a failed query by rejecting with an error message
      mockRepo.find.rejects(new Error('Query failed'));

      const result = await service.queryAllergies(queryDto);

      expect(result.isFailure).to.be.true;
      expect(result.errorValue()).to.equal('Query failed');
    });

    it('should return 200 if allergies are queried successfully', async () => {
      const queryDto = { name: 'Peanut' };

      // Simulate a successful query with an array of allergies
      const allergyInstance = Allergy.create({ id: '4A8Z.1', name: 'Peanut', description: 'Peanut allergy' });
      mockRepo.find.resolves([allergyInstance.getValue()]);

      const result = await service.queryAllergies(queryDto);

      expect(result.isSuccess).to.be.true;
      expect(result.getValue()).to.deep.equal([AllergyMap.toDTO(allergyInstance.getValue())]);
    });
  });
});
