/* eslint-disable prettier/prettier */
import { Allergy } from "../../domain/Allergy";
import { AllergyId } from "../../domain/AllergyId";
import IAllergyQueryDto from "../../dto/IAllergyQueryDto";
import IEditAllergyDto from "../../dto/IEditAllergyDto";


export default interface IAllergyRepo {
  exists(allergy: Allergy): Promise<boolean>;
  save(allergy: Allergy): Promise<Allergy>;
  findByDomainId(allergyId: AllergyId | string): Promise<Allergy | null>;
  getHighestId(): Promise<string | null>;
  find(queryDto: IAllergyQueryDto ): Promise<Allergy[]>;
  patch(allergyId: string, editAllergyDto:IEditAllergyDto): Promise<Allergy>;
  findByRawID(allergyId: AllergyId | string): Promise<Allergy | null>;

}
