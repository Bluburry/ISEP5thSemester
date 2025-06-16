/* eslint-disable prettier/prettier */
/* eslint-disable @typescript-eslint/no-unused-vars */
/* eslint-disable prettier/prettier */
import { Request, Response, NextFunction } from 'express';
import { Inject, Service } from 'typedi';
import IMedicalConditionService from '../services/IServices/IMedicalConditionService';
import IMedicalConditionDTO from '../dto/IMedicalConditionDTO';
import IMedicalConditionQueryDto from '../dto/IMedicalConditionQueryDto';
import { Result } from "../core/logic/Result";
import config from "../../config";
import IMedicalConditionController from './IControllers/IMedicalConditionController';
import { RSADecryptionService } from '../services/RSADecryptionService';
import IEditMedicalConditionDto from '../dto/IEditMedicalConditionDto';

@Service()
export default class MedicalConditionController implements IMedicalConditionController {
  constructor(
    @Inject(config.services.condition.name) private conditionServiceInstance: IMedicalConditionService
  ) {}

  public async createCondition(token: string, req: Request, res: Response, next: NextFunction): Promise<Response> {
    console.log('Creating Medical Condition');
    //console.log(token);
    try {
      const rsa = new RSADecryptionService();
      const decryptedToken = rsa.decrypt(token);

      if(token == null){
        console.error('No token in header');
        return res.status(400).send({ message: 'No token in header' });
      }
      try {
        const tokenObject = JSON.parse(decryptedToken);
        const authZ = tokenObject.TokenValue;
        console.log(authZ);
        if (authZ != 'ADMIN_AUTH_TOKEN') {
          return res.status(401).send({ message: 'Unauthorized' });
        }
      } catch (error) {
        console.error('Error parsing token JSON:', error.message);
        throw error;
      }

      const conditionOrError = await this.conditionServiceInstance.createCondition(req.body as IMedicalConditionDTO) as Result<IMedicalConditionDTO>;
      
      if (conditionOrError.isFailure) {
        console.log(conditionOrError.errorValue());
        return res.status(400).send({ message: conditionOrError.errorValue() });
      }

      const medicalConditionDTO = conditionOrError.getValue();
      console.log(medicalConditionDTO);
      
      return res.status(201).json(medicalConditionDTO);
    } catch (e) {
      console.error('Error in createCondition:', e);
      console.log('DSFJKBHBGDFKJJKDSGFBKDSFBKDSJFGH');
      return res.status(500).send({ message: 'Internal server error' });
    }
  }

  public async searchCondition(token: string, req: Request, res: Response, next: NextFunction): Promise<Response> {
    console.log(token)
    if(token == null){
      console.error('No token in header');
      return res.status(400).send({ message: 'No token in header' });
    }
    try {
      const rsa = new RSADecryptionService();
      const decryptedToken = rsa.decrypt(token);

      try {
        const tokenObject = JSON.parse(decryptedToken);
        const authZ = tokenObject.TokenValue;
        console.log(authZ);
        if (authZ != 'ADMIN_AUTH_TOKEN' && authZ != 'STAFF_AUTH_TOKEN') {
          return res.status(401).send({ message: 'Unauthorized' });
        }
      } catch (error) {
        console.error('Error parsing token JSON:', error.message);
        throw error;
      }
      
      const queryDto: IMedicalConditionQueryDto = req.query as IMedicalConditionQueryDto;
      const conditionOrError = await this.conditionServiceInstance.searchCondition(queryDto) as Result<IMedicalConditionDTO[]>;

      if (conditionOrError.isFailure) {
        return res.status(400).send({ message: conditionOrError.errorValue() });
      }

      const conditions = conditionOrError.getValue();
      return res.status(200).json(conditions);
    } catch (e) {
      console.error('Error in searchConditions:', e);
      return res.status(500).send({ message: 'Internal server error' });
    }
  }
  
  //Tecnicamente, não nos foi pedido para fazer, mas está feito com o mesmo raciocinio que o Allergy
  /* public async updateCondition(token: string, id: string, req: Request, res: Response, next: NextFunction): Promise<Response> {
    try {
      const rsa = new RSADecryptionService();
      const decryptedToken = rsa.decrypt(token);

      if(token == null){
        console.error('No token in header');
        return res.status(400).send({ message: 'No token in header' });
      }
      try {
        const tokenObject = JSON.parse(decryptedToken);
        const authZ = tokenObject.TokenValue;
        console.log(authZ);
        if (authZ != 'ADMIN_AUTH_TOKEN') {
          return res.status(401).send({ message: 'Unauthorized' });
        }
      } catch (error) {
        console.error('Error parsing token JSON:', error.message);
        throw error;
      }

      const allergyOrError = await this.conditionServiceInstance.updateCondition(id, req.body as IEditMedicalConditionDto) as Result<IMedicalConditionDTO>;

      if (allergyOrError.isFailure) {
        return res.status(400).send({ message: allergyOrError.errorValue() });
      }

      const allergyDTO = allergyOrError.getValue();
      return res.status(201).json(allergyDTO);
    } catch (e) {
      console.error('Error in createAllergy:', e);
      return res.status(500).send({ message: 'Internal server error' });
    }
  } */
}
