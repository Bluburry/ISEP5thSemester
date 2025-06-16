/* eslint-disable prettier/prettier */
/* eslint-disable @typescript-eslint/no-unused-vars */
/* eslint-disable prettier/prettier */
import config from "../../config";
import { Service, Inject } from 'typedi';
import { Result } from '../core/logic/Result';
import { Allergy } from '../domain/Allergy';
import IAllergyDTO from '../dto/IAllergyDTO';
import IAllergyQueryDto from '../dto/IAllergyQueryDto';
import IAllergyRepo from './IRepos/IAllergyRepo';
import IAllergyService from './IServices/IAllergyService';
import IEditAllergyDto from "../dto/IEditAllergyDto";
import { AllergyMap } from "../mappers/AllergyMap";


@Service()
class AllergyService implements IAllergyService {
  constructor(@Inject(config.repos.allergy.name) private allergyRepo: IAllergyRepo) {}
  public async patchAllergies(id: string, editAllergyDto: IEditAllergyDto): Promise<Result<IAllergyDTO>> {
    try {
      const updatedAllergy = await this.allergyRepo.patch(id, editAllergyDto);
      const allergyDTO = AllergyMap.toDTO(updatedAllergy);
      return Result.ok<IAllergyDTO>(allergyDTO);
    } catch (e) {
      return Result.fail<IAllergyDTO>(e.message);
    }
  }

  public async createAllergy(allergyDTO: IAllergyDTO): Promise<Result<IAllergyDTO>> {
    
    try {
      let allergyOrError;
      const id = await this.allergyRepo.getHighestId();
      
      if(id != null){
        const parts = id.split('.');
      
        const number = parseInt(parts[1], 10);
        console.log(number);
        console.log(parts[0] + String(number+1));
        allergyOrError = Allergy.create(allergyDTO, parts[0] + '.' + String(number+1));
      }else if((await this.allergyRepo.find({ name: '', code: '', description: '' })).length == 0){
        allergyOrError = Allergy.create(allergyDTO, '4A8Z.1');
      }
      else{
        allergyOrError = Allergy.create(allergyDTO);
        const checkAllergy = this.allergyRepo.findByDomainId(allergyDTO.id);
        if(checkAllergy != null){
          return Result.fail<IAllergyDTO>('This ICD already exists, please consult database');
        }
      }
      
      

      if (allergyOrError.isFailure) {
        return Result.fail<IAllergyDTO>(allergyOrError.errorValue());
      }

      const allergy = allergyOrError.getValue();
      await this.allergyRepo.save(allergy);
      const allergyDTOResult = AllergyMap.toDTO(allergy);

      return Result.ok<IAllergyDTO>(allergyDTOResult);
    } catch (e) {
      throw e;
    }
  }

  public async queryAllergies(queryDto: IAllergyQueryDto): Promise<Result<IAllergyDTO[]>> {
    console.log('Querying allergies');
    try {
      const allergies = await this.allergyRepo.find(queryDto);
      const allergyDTOs = allergies.map(allergy => AllergyMap.toDTO(allergy));
      return Result.ok<IAllergyDTO[]>(allergyDTOs);
    } catch (e) {
      return Result.fail<IAllergyDTO[]>(e.message);
    }
  }

}

export default AllergyService;
