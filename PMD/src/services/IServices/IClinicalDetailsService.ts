/* eslint-disable prettier/prettier */
import { IClinicalDetailsDTO } from "../../dto/IClinicalDetailsDTO";

export default interface IClinicalDetailsService {
  save(clinicalDetails: IClinicalDetailsDTO): Promise<IClinicalDetailsDTO>;
  findByDomainId(clinicalDetailsId: string): Promise<IClinicalDetailsDTO | null>;
  createBlank(mrn: string): Promise<IClinicalDetailsDTO>;
  filterByValues(allergy: string, condition: string): Promise<IClinicalDetailsDTO[] | null>;
	sendClinicalDetails(clinicalDetailsId: string, password: string): Promise<IClinicalDetailsDTO | null>;
	sendClinicalDetailsDemonstration(password: string): Promise<string | null>;

}
