/* eslint-disable prettier/prettier */

import { Result } from "../../core/logic/Result";
import IAllergyDTO from "../../dto/IAllergyDTO";
import IAllergyQueryDto from "../../dto/IAllergyQueryDto";
import IEditAllergyDto from "../../dto/IEditAllergyDto";


export default interface IAllergyService {
  createAllergy(allergyDTO: IAllergyDTO): Promise<Result<IAllergyDTO>>;
  queryAllergies(queryDto: IAllergyQueryDto): Promise<Result<IAllergyDTO[]>>;
  patchAllergies(id: string, queryDto: IEditAllergyDto): Promise<Result<IAllergyDTO>>;
}
