import { expect } from 'chai';
import { describe, it } from 'mocha';
import { Allergy } from '../../../src/domain/Allergy';
import IAllergyDTO from '../../../src/dto/IAllergyDTO';

describe('Allergy Domain', () => {
  
  describe('create', () => {
    it('should return failure if allergy name is empty', () => {
      const allergyDTO: IAllergyDTO = { id: '4A8Z.1', name: '', description: 'Peanut allergy' };
      
      const result = Allergy.create(allergyDTO);
      
      expect(result.isFailure).to.be.true;
      expect(result.errorValue()).to.equal('Must provide an allergy name');
    });

    it('should return success if allergy is created with valid properties', () => {
      const allergyDTO: IAllergyDTO = { id: '4A8Z.1', name: 'Peanut', description: 'Peanut allergy' };
      
      const result = Allergy.create(allergyDTO);
      
      expect(result.isSuccess).to.be.true;
      expect(result.getValue().name).to.equal('Peanut');
      expect(result.getValue().description).to.equal('Peanut allergy');
      expect(result.getValue().id).to.not.be.null;
    });

    it('should set default id if not provided', () => {
      const allergyDTO: IAllergyDTO = { id: '', name: 'Peanut', description: 'Peanut allergy' };
      
      const result = Allergy.create(allergyDTO);
      
      expect(result.isSuccess).to.be.true;
      expect(result.getValue().id).to.not.be.null;
    });

    it('should return failure if description is provided but name is missing', () => {
      const allergyDTO: IAllergyDTO = { id: '4A8Z.1', name: '', description: 'Peanut allergy' };
      
      const result = Allergy.create(allergyDTO);
      
      expect(result.isFailure).to.be.true;
      expect(result.errorValue()).to.equal('Must provide an allergy name');
    });
  });

  describe('name and description properties', () => {
    it('should allow getting and setting name', () => {
      const allergyDTO: IAllergyDTO = { id: '4A8Z.1', name: 'Peanut', description: 'Peanut allergy' };
      
      const result = Allergy.create(allergyDTO);
      const allergy = result.getValue();
      
      allergy.name = 'Updated Peanut';
      expect(allergy.name).to.equal('Updated Peanut');
    });

    it('should allow getting and setting description', () => {
      const allergyDTO: IAllergyDTO = { id: '4A8Z.1', name: 'Peanut', description: 'Peanut allergy' };
      
      const result = Allergy.create(allergyDTO);
      const allergy = result.getValue();
      
      allergy.description = 'Updated description';
      expect(allergy.description).to.equal('Updated description');
    });

    it('should default description to null if not provided', () => {
      const allergyDTO: IAllergyDTO = { id: '4A8Z.1', name: 'Peanut', description: null };
      
      const result = Allergy.create(allergyDTO);
      const allergy = result.getValue();
      
      expect(allergy.description).to.equal(null);
    });
  });
});
