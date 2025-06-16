/* eslint-disable prettier/prettier */
/* eslint-disable @typescript-eslint/no-unused-vars */
/* eslint-disable prettier/prettier */
import config from "../../config";
import { Service, Inject } from 'typedi';
import { Result } from '../core/logic/Result';
import { MedicalCondition } from '../domain/MedicalCondition';
import IMedicalConditionDTO from '../dto/IMedicalConditionDTO';
import IMedicalConditionQueryDto from '../dto/IMedicalConditionQueryDto';
import IConditionRepo from './IRepos/IConditionRepo';
import IMedicalConditionService from './IServices/IMedicalConditionService';
import IEditMedicalConditionDto from "../dto/IEditMedicalConditionDto";

@Service()
class ConditionService implements IMedicalConditionService {
  constructor(@Inject(config.repos.conditions.name) private conditionRepo: IConditionRepo) {}

  public async createCondition(conditionDTO: IMedicalConditionDTO): Promise<Result<IMedicalConditionDTO>> {
    console.log('Creating Medical Condition');
    try {
      let ConditionOrError;

      if(conditionDTO.code != null){
        ConditionOrError = MedicalCondition.create(conditionDTO, conditionDTO.code);
      }else{
        return Result.fail<IMedicalConditionDTO>('Code must be provided.');
      }
      
      if (ConditionOrError.isFailure) {
        return Result.fail<IMedicalConditionDTO>(ConditionOrError.errorValue());
      }

      const condition = ConditionOrError.getValue();
      await this.conditionRepo.save(condition);
      const conditionDTOResult = this.toDTO(condition);

      return Result.ok<IMedicalConditionDTO>(conditionDTOResult);
    } catch (e) {
      throw e;
    }
  }

  public async searchCondition(queryDto: IMedicalConditionQueryDto): Promise<Result<IMedicalConditionDTO[]>> {
    console.log('Querying Conditions');
    try {
      const conditions = await this.conditionRepo.find(queryDto);
      const conditionDTOs = conditions.map(condition => this.toDTO(condition));
      return Result.ok<IMedicalConditionDTO[]>(conditionDTOs);
    } catch (e) {
      return Result.fail<IMedicalConditionDTO[]>(e.message);
    }
  }

  //Tecnicamente, não nos foi pedido para fazer, mas está feito com o mesmo raciocinio que o Allergy
  /* public async updateCondition(id: number, editMedicalConditionDTO: IEditMedicalConditionDto): Promise<Result<IMedicalConditionDTO>> {
    try {
      const updatedCondition = await this.conditionRepo.update(id, editMedicalConditionDTO);
      const allergyDTO = this.toDTO(updatedCondition);
      return Result.ok<IMedicalConditionDTO>(allergyDTO);
    } catch (e) {
      return Result.fail<IMedicalConditionDTO>(e.message);
    }
  } */

  private toDTO(condition: MedicalCondition): IMedicalConditionDTO {
    const idValue = condition.id.toValue();

    if (typeof idValue === 'string') {
    throw new Error('ID must be a number for persistence');
    }

    return {
      code: idValue, // Guaranteed to be a number
      designation: condition.designation,
      description: condition.description,
      symptoms: condition.symptoms,
    };
  }
}

export default ConditionService;
