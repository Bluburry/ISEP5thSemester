/* eslint-disable prettier/prettier */
import IAllergyDTO from './IAllergyDTO';
import IMedicalConditionDTO from './IMedicalConditionDTO';

export interface IClinicalDetailsDTO {
  patientMRN: string;
  allergies: IAllergyDTO[];
  medicalConditions: IMedicalConditionDTO[];
}