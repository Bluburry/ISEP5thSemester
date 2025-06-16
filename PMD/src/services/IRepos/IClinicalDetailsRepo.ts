/* eslint-disable prettier/prettier */

import { ClinicalDetails } from "../../domain/ClinicalDetails";
import { IClinicalDetailsDTO } from "../../dto/IClinicalDetailsDTO";



export default interface IClinicalDetailsRepo {
  save(clinicalDetails: ClinicalDetails): Promise<ClinicalDetails>;
  findByDomainId(clinicalDetailsCode: string): Promise<ClinicalDetails | null>;
  getAll(): Promise<ClinicalDetails[]>;
  //update(editClinicalDetailsDto: IClinicalDetailsDTO): Promise<ClinicalDetails>;
}
