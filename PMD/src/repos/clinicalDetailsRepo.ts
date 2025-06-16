/* eslint-disable prettier/prettier */
/* eslint-disable @typescript-eslint/no-unused-vars */
/* eslint-disable prettier/prettier */
/* eslint-disable @typescript-eslint/no-explicit-any */
/* eslint-disable prettier/prettier */

import { Inject, Service } from "typedi";
import { Document, Model, FilterQuery, UpdateQuery } from 'mongoose';
import { ClinicalDetails } from '../domain/ClinicalDetails';
import { IClinicalDetailsPersistence } from '../dataschema/IClinicalDetailsPersistence';
import { ClinicalDetailsMap } from '../mappers/ClinicalDetailsMap';
import IClinicalDetailsRepo from "../services/IRepos/IClinicalDetailsRepo";

@Service()
export default class ClinicalDetailsRepo implements IClinicalDetailsRepo {
  constructor(
    @Inject('clinicalDetailsSchema') private clinicalDetailsSchema: Model<IClinicalDetailsPersistence & Document>,
  ) {}

  public async save(clinicalDetails: ClinicalDetails): Promise<ClinicalDetails> {
    console.log('============ Save - Repo ============');
    const query = { patientMRN: clinicalDetails.id.toString() };

    const clinicalDetailsDocument = await this.clinicalDetailsSchema.findOne(query);
    try {
      console.log('Patient MRN: ' + clinicalDetails.id.toString());
      if (clinicalDetailsDocument === null) {
        const rawClinicalDetails = ClinicalDetailsMap.toPersistence(clinicalDetails);
        console.log(rawClinicalDetails);
        console.log('Patient MRN: ' + rawClinicalDetails.patientMRN);
        console.log('Allergies: ' + rawClinicalDetails.allergies);
        console.log('Medical Conditions: ' + rawClinicalDetails.medicalConditions);
        const clinicalDetailsCreated = await this.clinicalDetailsSchema.create(rawClinicalDetails);

        return ClinicalDetailsMap.toDomain(clinicalDetailsCreated);
      } else {
        clinicalDetailsDocument.allergies = clinicalDetails.allergies.map(allergy => allergy.id.toString());
        clinicalDetailsDocument.medicalConditions = clinicalDetails.medicalConditions.map(condition => condition.id.toString());
        await clinicalDetailsDocument.save();

        return clinicalDetails;
      }
    } catch (err) {
      console.error("Error while saving clinical details in the Repo: ", err);
      throw new Error("Failed to save clinical details in the Repo.");
    }    
  }

  public async findByDomainId(id: string): Promise<ClinicalDetails | null> {
    console.log('============ FindByDomain - Repo ============');
    const query = { patientMRN: id };
    const clinicalDetailsRecord = await this.clinicalDetailsSchema.findOne(query);

    console.log(clinicalDetailsRecord);
    if (!clinicalDetailsRecord) {return null;}

    return ClinicalDetailsMap.toDomain(clinicalDetailsRecord);
  }
  
  public async getAll(): Promise<ClinicalDetails[]> {
  
    const clinicalDetailsRecords = await this.clinicalDetailsSchema.find();
    
    if (!clinicalDetailsRecords || clinicalDetailsRecords.length === 0) {
      return null;
    }
  
    return Promise.all(clinicalDetailsRecords.map(record => ClinicalDetailsMap.toDomain(record)));
  }
}