/* eslint-disable prettier/prettier */
import { expect } from 'chai';
import { ClinicalDetails } from '../../../src/domain/ClinicalDetails';
import { MedicalCondition } from '../../../src/domain/MedicalCondition';
import IAllergyDTO from '../../../src/dto/IAllergyDTO';
import { Allergy } from '../../../src/domain/Allergy';
import IMedicalConditionDTO from '../../../src/dto/IMedicalConditionDTO';

describe('ClinicalDetails', () => {
  it('should create ClinicalDetails with details', () => {
    
    const dtoAllergy = {id: 'Shellfish', name:'Severe allergic reaction'} as IAllergyDTO;
    const allergyResult = Allergy.create(dtoAllergy);
    const allergy = allergyResult.getValue();
    const allergies: Allergy[] = [
      allergy
    ];
    const dtoMedicalCondition = {code: 12345678, designation:'Diabetes', description:'Increased thirst', symptoms:'frequent urination'} as IMedicalConditionDTO;
    const medicalConditionResult = MedicalCondition.create(dtoMedicalCondition, 12345678);  
    const medicalCondition = medicalConditionResult.getValue();
    const medicalConditions: MedicalCondition[] = [
      medicalCondition
    ];

    const result = ClinicalDetails.createWithDetails(allergies, medicalConditions, 'MRN12345');

    expect(result.isSuccess).to.be.true;
    expect(result.getValue()).to.be.instanceOf(ClinicalDetails);
    expect(result.getValue().allergies).to.deep.equal(allergies);
    expect(result.getValue().medicalConditions).to.deep.equal(medicalConditions);
  });

  it('should fail to create ClinicalDetails with details if MRN is missing', () => {
    const allergies: Allergy[] = [];
    const medicalConditions: MedicalCondition[] = [];

    const result = ClinicalDetails.createWithDetails(allergies, medicalConditions, '');

    expect(result.isFailure).to.be.true;
    expect(result.errorValue()).to.equal('MRN is required [CreateWithDetails]');
  });

  it('should create blank ClinicalDetails', () => {
    const result = ClinicalDetails.createBlank('MRN12345');

    expect(result.isSuccess).to.be.true;
    expect(result.getValue()).to.be.instanceOf(ClinicalDetails);
    expect(result.getValue().allergies).to.deep.equal([]);
    expect(result.getValue().medicalConditions).to.deep.equal([]);
  });

  it('should fail to create blank ClinicalDetails if MRN is missing', () => {
    const result = ClinicalDetails.createBlank('');

    expect(result.isFailure).to.be.true;
    expect(result.errorValue()).to.equal('MRN is required [CreateBlank]');
  });

  it('should update the list of allergies', () => {
    const result = ClinicalDetails.createBlank('MRN12345');
    const clinicalDetails = result.getValue();
    const dtoAllergy = {id: 'Shellfish', name:'Severe allergic reaction'} as IAllergyDTO;
    const allergyResult = Allergy.create(dtoAllergy);
    const allergy = allergyResult.getValue();
    const newAllergies: Allergy[] = [
      allergy
    ];
    clinicalDetails.allergies = newAllergies;

    expect(clinicalDetails.allergies).to.deep.equal(newAllergies);
  });

  it('should update the list of medical conditions', () => {
    const result = ClinicalDetails.createBlank('MRN12345');
    const clinicalDetails = result.getValue();
    const dtoMedicalCondition = {code: 12345678, designation:'Diabetes', description:'Increased thirst', symptoms:'frequent urination'} as IMedicalConditionDTO;
    const medicalConditionResult = MedicalCondition.create(dtoMedicalCondition, 12345678);  
    const medicalCondition = medicalConditionResult.getValue();
    const newMedicalConditions: MedicalCondition[] = [
      medicalCondition
    ];
    clinicalDetails.medicalConditions = newMedicalConditions;

    expect(clinicalDetails.medicalConditions).to.deep.equal(newMedicalConditions);
  });

  it('should return the correct MRN as id', () => {
    const result = ClinicalDetails.createBlank('MRN12345');
    const clinicalDetails = result.getValue();

    expect(clinicalDetails.id.toValue()).to.equal('MRN12345');
  });
});
