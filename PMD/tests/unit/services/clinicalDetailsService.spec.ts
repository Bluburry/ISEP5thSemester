/* eslint-disable @typescript-eslint/no-unused-vars */
/* eslint-disable prettier/prettier */
import 'reflect-metadata';
import { expect } from 'chai';
import { Container } from 'typedi';
import ClinicalDetailsService from '../../../src/services/ClinicalDetailsService';
import IClinicalDetailsRepo from '../../../src/services/IRepos/IClinicalDetailsRepo';
import { ClinicalDetailsMap } from '../../../src/mappers/ClinicalDetailsMap';
import { ClinicalDetails } from '../../../src/domain/ClinicalDetails';
import { Allergy } from '../../../src/domain/Allergy';
import { MedicalCondition } from '../../../src/domain/MedicalCondition';
import { Result } from '../../../src/core/logic/Result';
import { cli } from 'winston/lib/winston/config';

describe('ClinicalDetailsService', () => {
  let clinicalDetailsRepo: IClinicalDetailsRepo;
  let clinicalDetailsService: ClinicalDetailsService;

  beforeEach(() => {
    // Mock the repo
    clinicalDetailsRepo = {
      save: async () => undefined,
      findByDomainId: async () => undefined,
      getAll: async () => [],
    } as unknown as IClinicalDetailsRepo;
    
    // Mock the service with the repo
    Container.set('clinicalDetailsRepo', clinicalDetailsRepo);
    clinicalDetailsService = new ClinicalDetailsService(clinicalDetailsRepo);

    // Stub ClinicalDetailsMap and ClinicalDetails static methods
    ClinicalDetailsMap.toDomain = async (dto: any) => dto as unknown as ClinicalDetails;
    ClinicalDetailsMap.toDTO = (domain: ClinicalDetails) => domain as unknown as any;
  });

  afterEach(() => {
    Container.reset();
  });

  describe('save', () => {
    it('should save valid clinical details', async () => {

    const allergy1 = Allergy.create({ id: '4A8Z.1', name: 'Peanut', description: 'Peanut allergy' }).getValue();
    const allergy2 = Allergy.create({ id: '4A8Z.1', name: 'Dust', description: 'Dust allergy' }).getValue();

    const medicalCondition1 = MedicalCondition.create({ code: 1234567, designation: 'Hypertension', symptoms: 'High blood pressure' }, 1234567).getValue();
    const medicalCondition2 = MedicalCondition.create({ code: 4564567, designation: 'Asthma', symptoms: 'Breathing difficulties' }, 4564567).getValue();

    // Patient MRN (Medical Record Number)
    const patientMRN = '123456789';

    // Create a ClinicalDetails object with details
    const clinicalDetails: Result<ClinicalDetails> = ClinicalDetails.createWithDetails(
    [allergy1, allergy2],                // Allergies
    [medicalCondition1, medicalCondition2], // Medical Conditions
    patientMRN                           // Patient MRN
    );


      const clinicalDetailsDto = { allergies: [], medicalConditions: [], id: '123456789' };

      clinicalDetailsRepo.save = async () => clinicalDetails.getValue();

      const result = await clinicalDetailsService.save(clinicalDetailsDto);

      expect(result).to.deep.equal(clinicalDetails.getValue());
    });
   
  });

  describe('createBlank', () => {
    it('should create blank clinical details', async () => {

    const allergy1 = Allergy.create({ id: '4A8Z.1', name: 'Peanut', description: 'Peanut allergy' }).getValue();
    const allergy2 = Allergy.create({ id: '4A8Z.1', name: 'Dust', description: 'Dust allergy' }).getValue();

    const medicalCondition1 = MedicalCondition.create({ code: 1234567, designation: 'Hypertension', symptoms: 'High blood pressure' }, 1234567).getValue();
    const medicalCondition2 = MedicalCondition.create({ code: 4564567, designation: 'Asthma', symptoms: 'Breathing difficulties' }, 4564567).getValue();

    // Patient MRN (Medical Record Number)
    const patientMRN = '123456789';

    // Create a ClinicalDetails object with details
    const clinicalDetails: Result<ClinicalDetails> = ClinicalDetails.createWithDetails(
    [allergy1, allergy2],                // Allergies
    [medicalCondition1, medicalCondition2], // Medical Conditions
    patientMRN                           // Patient MRN
    );
      const mrn = '123456789';
      const savedClinicalDetails = { id: '123456789' };

      clinicalDetailsRepo.save = async () => clinicalDetails.getValue();

      const result = await clinicalDetailsService.createBlank(mrn);

      expect(result).to.deep.equal(ClinicalDetailsMap.toDTO(clinicalDetails.getValue()));

    });

  });

  describe('findByDomainId', () => {
    it('should find clinical details by domain ID', async () => {

        const allergy1 = Allergy.create({ id: '4A8Z.1', name: 'Peanut', description: 'Peanut allergy' }).getValue();
        const allergy2 = Allergy.create({ id: '4A8Z.1', name: 'Dust', description: 'Dust allergy' }).getValue();

    const medicalCondition1 = MedicalCondition.create({ code: 12367890, designation: 'Hypertension', symptoms: 'High blood pressure' }, 12367890).getValue();
    const medicalCondition2 = MedicalCondition.create({ code: 45609876, designation: 'Asthma', symptoms: 'Breathing difficulties' }, 45609876).getValue();

    // Patient MRN (Medical Record Number)
    const patientMRN = '123456789';

    // Create a ClinicalDetails object with details
    const clinicalDetails: Result<ClinicalDetails> = ClinicalDetails.createWithDetails(
    [allergy1, allergy2],                // Allergies
    [medicalCondition1, medicalCondition2], // Medical Conditions
    patientMRN                           // Patient MRN
    );
      clinicalDetailsRepo.findByDomainId = async () => clinicalDetails.getValue();

      const result = await clinicalDetailsService.findByDomainId('123456789');


      console.log(result);
      expect(result).to.deep.equal(ClinicalDetailsMap.toDTO(clinicalDetails.getValue()));
    });


  });
});
