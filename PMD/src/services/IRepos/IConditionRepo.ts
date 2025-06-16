/* eslint-disable prettier/prettier */
import { MedicalCondition } from "../../domain/MedicalCondition";
import { MedicalConditionCode } from "../../domain/MedicalConditionCode";
import IMedicalConditionQueryDto from '../../dto/IMedicalConditionQueryDto';
import IEditMedicalConditionDto from '../../dto/IEditMedicalConditionDto';


export default interface IConditionRepo {
  exists(condition: MedicalCondition): Promise<boolean>;
  save(condition: MedicalCondition): Promise<MedicalCondition>;
  findByDomainId(conditionCode: MedicalConditionCode | number): Promise<MedicalCondition | null>;
  getHighestId(): Promise<number | null>;
  find(queryDto: IMedicalConditionQueryDto): Promise<MedicalCondition[]>;
  //update(conditionId: number, editMedicalConditionDto:IEditMedicalConditionDto): Promise<MedicalCondition>;
}
