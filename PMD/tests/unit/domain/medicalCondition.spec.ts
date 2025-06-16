/* eslint-disable prettier/prettier */
import { expect } from 'chai';
import { MedicalCondition } from '../../../src/domain/MedicalCondition';
import IMedicalConditionDTO from '../../../src/dto/IMedicalConditionDTO';

describe('MedicalCondition', () => {
  it('should create a valid MedicalCondition', () => {
    const conditionDTO: IMedicalConditionDTO = {
      code: 123456,
      designation: 'Hypertension',
      description: 'High blood pressure',
      symptoms: 'Headache, dizziness',
    };

    const result = MedicalCondition.create(conditionDTO, conditionDTO.code);

    expect(result.isSuccess).to.be.true;
    expect(result.getValue()).to.be.instanceOf(MedicalCondition);
    expect(result.getValue().designation).to.equal(conditionDTO.designation);
    expect(result.getValue().description).to.equal(conditionDTO.description);
    expect(result.getValue().symptoms).to.equal(conditionDTO.symptoms);
  });

  it('should fail to create a MedicalCondition with an empty designation', () => {
    const conditionDTO: IMedicalConditionDTO = {
      code: 123456,
      designation: '',
      description: 'High blood pressure',
      symptoms: 'Headache, dizziness',
    };

    const result = MedicalCondition.create(conditionDTO, conditionDTO.code);

    expect(result.isFailure).to.be.true;
    expect(result.errorValue()).to.equal('Must provide a medical condition name');
  });

  it('should fail to create a MedicalCondition with an invalid code', () => {
    const conditionDTO: IMedicalConditionDTO = {
      code: 123,
      designation: 'Hypertension',
      description: 'High blood pressure',
      symptoms: 'Headache, dizziness',
    };

    const result = MedicalCondition.create(conditionDTO, conditionDTO.code);

    expect(result.isFailure).to.be.true;
    expect(result.errorValue()).to.equal('Code must be a number between 6 and 18 digits to be eligible');
  });

  it('should update the designation', () => {
    const conditionDTO: IMedicalConditionDTO = {
      code: 123456,
      designation: 'Hypertension',
      description: 'High blood pressure',
      symptoms: 'Headache, dizziness',
    };

    const result = MedicalCondition.create(conditionDTO, conditionDTO.code);
    const medicalCondition = result.getValue();

    medicalCondition.designation = 'Hypertensive Disorder';

    expect(medicalCondition.designation).to.equal('Hypertensive Disorder');
  });

  it('should update the description', () => {
    const conditionDTO: IMedicalConditionDTO = {
      code: 123456,
      designation: 'Hypertension',
      description: 'High blood pressure',
      symptoms: 'Headache, dizziness',
    };

    const result = MedicalCondition.create(conditionDTO, conditionDTO.code);
    const medicalCondition = result.getValue();

    medicalCondition.description = 'Chronic high blood pressure';

    expect(medicalCondition.description).to.equal('Chronic high blood pressure');
  });

  it('should update the symptoms', () => {
    const conditionDTO: IMedicalConditionDTO = {
      code: 123456,
      designation: 'Hypertension',
      description: 'High blood pressure',
      symptoms: 'Headache, dizziness',
    };

    const result = MedicalCondition.create(conditionDTO, conditionDTO.code);
    const medicalCondition = result.getValue();

    medicalCondition.symptoms = 'Severe headache, dizziness';

    expect(medicalCondition.symptoms).to.equal('Severe headache, dizziness');
  });
});