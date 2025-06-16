/* eslint-disable prettier/prettier */
/* eslint-disable @typescript-eslint/no-explicit-any */
/* eslint-disable prettier/prettier */
import { Container } from 'typedi';
import { Mapper } from '../core/infra/Mapper';
import { IClinicalDetailsDTO } from '../dto/IClinicalDetailsDTO';
import { ClinicalDetails } from '../domain/ClinicalDetails';
import { UniqueEntityID } from '../core/domain/UniqueEntityID';
import { AllergyMap } from './AllergyMap';
import AllergyRepo from '../repos/allergyRepo';
import ConditionRepo from '../repos/conditionRepo';
import { MedicalConditionCode } from '../domain/MedicalConditionCode';
import { IClinicalDetailsPersistence } from '../dataschema/IClinicalDetailsPersistence';
import { cli } from 'winston/lib/winston/config';
import { Allergy } from '../domain/Allergy';
import { MedicalCondition } from '../domain/MedicalCondition';

export class ClinicalDetailsMap extends Mapper<ClinicalDetails> {
  public static toDTO(clinicalDetails: ClinicalDetails): IClinicalDetailsDTO {
    return {
      patientMRN: clinicalDetails.id.toString(),
      allergies: clinicalDetails.allergies.map(allergy => AllergyMap.toDTO(allergy)),
      medicalConditions: clinicalDetails.medicalConditions.map(condition => condition.toDTO()),
    } as IClinicalDetailsDTO;
  }

  public static async toDomain(raw: any): Promise<ClinicalDetails> {
    console.log('============ ToDomain ============');
    console.log('Mapping Clinical Details to Domain');
    const allergyRepo = Container.get(AllergyRepo);
    const medicalConditionRepo = Container.get(ConditionRepo);
  
    const allergies = raw.allergies ? await this.mapAllergies(allergyRepo, raw.allergies) : [];
    const medicalConditions = raw.medicalConditions ? await this.mapMedicalConditions(medicalConditionRepo, raw.medicalConditions) : [];
  
    console.log(allergies)

    console.log('Allergies and Medical Conditions mapped');
    const clinicalDetailsOrError = ClinicalDetails.createWithDetails(allergies, medicalConditions, raw.patientMRN);
    
    clinicalDetailsOrError.isFailure ? console.log(clinicalDetailsOrError.error) : '';
  
    return clinicalDetailsOrError.isSuccess ? clinicalDetailsOrError.getValue() : null;
  }
  
  private static async mapAllergies(allergyRepo, allergies: any[]): Promise<Allergy[]> {
    try {
      return await Promise.all(allergies.map(async (allergy: any) => {
        console.log("Allergy/Allergy ID to add: " + JSON.stringify(allergy));
        if (allergy.id != undefined){
          return await allergyRepo.findByDomainId(allergy.id); 
        } else{
          return await allergyRepo.findByDomainId(allergy); 
        }
      }));
    } catch (error) {
      console.error('Error mapping allergies:', error);
      return [];  
    }
  }
  
  private static async mapMedicalConditions(medicalConditionRepo, conditions: any[]): Promise<MedicalCondition[]> {
    try {
      return await Promise.all(conditions.map(async (condition: any) => {
        console.log('MedicalCondition to Add:' + JSON.stringify(condition));
        if (condition.code != undefined){
          return await medicalConditionRepo.findByDomainId(condition.code);
        } else {
          return await medicalConditionRepo.findByDomainId(condition);
        }
      }));
    } catch (error) {
      console.error('Error mapping medical conditions:', error);
      return [];  
    }
  }
  

  public static toPersistence(clinicalDetails: ClinicalDetails): IClinicalDetailsPersistence {
    console.log('============ ToPersistence ============');
    console.log('MRN: ' + clinicalDetails.id.toString());
    return {
      patientMRN: clinicalDetails.id.toString(),
      allergies: clinicalDetails.allergies?.map(allergy => allergy.id.toString()) ?? [],
      medicalConditions: clinicalDetails.medicalConditions?.map(condition => condition.id.toString()) ?? [],
    };
  }
  
}
