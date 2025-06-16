/* eslint-disable prettier/prettier */
import { AggregateRoot } from '../core/domain/AggregateRoot';
import { Result } from '../core/logic/Result';
import { Allergy } from './Allergy';
import { MedicalCondition } from './MedicalCondition';
import IAllergyDTO from '../dto/IAllergyDTO';
import IMedicalConditionDTO from '../dto/IMedicalConditionDTO';
import { UniqueEntityID } from '../core/domain/UniqueEntityID';

interface ClinicalDetailsProps {
  allergy: Allergy[];
  medicalCondition: MedicalCondition[];
}

export class ClinicalDetails extends AggregateRoot<ClinicalDetailsProps> {
  private readonly _patientMRN: UniqueEntityID;    //For naming purposes, the list ID will be called patientMRN (since they both are unique and will be same)

  get id(): UniqueEntityID {
    return this._patientMRN;
  }

  get allergies(): Allergy[] {
    return this.props.allergy;
  }

  set allergies(value: Allergy[]) {
    this.props.allergy = value;
  }

  get medicalConditions(): MedicalCondition[] {
    return this.props.medicalCondition;
  }

  set medicalConditions(value: MedicalCondition[]) {
    this.props.medicalCondition = value;
  }

  private constructor(props: ClinicalDetailsProps, mrn: string) {
    super(props, new UniqueEntityID(mrn));
    this._patientMRN = new UniqueEntityID(mrn);
  }

  public static createWithDetails(
    allergies: Allergy[],
    medicalConditions: MedicalCondition[],
    mrn: string
  ): Result<ClinicalDetails> {
    if (!mrn) {
      return Result.fail<ClinicalDetails>('MRN is required [CreateWithDetails]');
    }

    const clinicalDetails = new ClinicalDetails(
      { allergy: allergies, medicalCondition: medicalConditions },
      mrn
    );
    
    return Result.ok<ClinicalDetails>(clinicalDetails);
  }

  public static createBlank(mrn: string): Result<ClinicalDetails> {
    if (!mrn) {
      return Result.fail<ClinicalDetails>('MRN is required [CreateBlank]');
    }

    const clinicalDetails = new ClinicalDetails(
      { allergy: [], medicalCondition: [] },
      mrn
    );

    return Result.ok<ClinicalDetails>(clinicalDetails);
  }
}