/* eslint-disable prettier/prettier */

import { Result } from "../../core/logic/Result";
import IMedicalConditionDTO from "../../dto/IMedicalConditionDTO";
import IMedicalConditionQueryDto from "../../dto/IMedicalConditionQueryDto";
import IEditMedicalConditionDto from "../../dto/IEditMedicalConditionDto";


export default interface IMedicalConditionService {
  createCondition(allergyDTO: IMedicalConditionDTO): Promise<Result<IMedicalConditionDTO>>;
  searchCondition(queryDto: IMedicalConditionQueryDto): Promise<Result<IMedicalConditionDTO[]>>;
  //updateCondition(id: number, queryDto: IEditMedicalConditionDto): Promise<Result<IMedicalConditionDTO>>;
}
