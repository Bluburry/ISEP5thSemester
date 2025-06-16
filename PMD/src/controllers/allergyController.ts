/* eslint-disable prettier/prettier */
/* eslint-disable @typescript-eslint/no-unused-vars */
/* eslint-disable prettier/prettier */
import { Request, Response, NextFunction } from 'express';
import { Inject, Service } from 'typedi';
import IAllergyService from '../services/IServices/IAllergyService';
import IAllergyDTO from '../dto/IAllergyDTO';
import IAllergyQueryDto from '../dto/IAllergyQueryDto';
import { Result } from "../core/logic/Result";
import config from "../../config";
import IAllergyController from './IControllers/IAllergyController';
import { RSADecryptionService } from '../services/RSADecryptionService';
import IEditAllergyDto from '../dto/IEditAllergyDto';

@Service()
export default class AllergyController implements IAllergyController {
  constructor(
    @Inject(config.services.allergy.name) private allergyServiceInstance: IAllergyService
  ) {}


  public async patchAllergies(token: string, id: string, req: Request, res: Response, next: NextFunction): Promise<Response> {
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

      const allergyOrError = await this.allergyServiceInstance.patchAllergies(id, req.body as IEditAllergyDto) as Result<IAllergyDTO>;

      if (allergyOrError.isFailure) {
        return res.status(400).send({ message: allergyOrError.errorValue() });
      }

      const allergyDTO = allergyOrError.getValue();
      return res.status(201).json(allergyDTO);
    } catch (e) {
      console.error('Error in createAllergy:', e);
      return res.status(500).send({ message: 'Internal server error' });
    }
  }

  public async createAllergy(token: string, req: Request, res: Response, next: NextFunction): Promise<Response> {
    console.log('Creating allergy');
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

      const allergyOrError = await this.allergyServiceInstance.createAllergy(req.body as IAllergyDTO) as Result<IAllergyDTO>;
      
      if (allergyOrError.isFailure) {
        return res.status(400).send({ message: allergyOrError.errorValue() });
      }

      const allergyDTO = allergyOrError.getValue();
      console.log(allergyDTO);
      
      return res.status(201).json(allergyDTO);
    } catch (e) {
      console.error('Error in createAllergy:', e);
      return res.status(500).send({ message: 'Internal server error' });
    }
  }

  public async queryAllergies(token: string, req: Request, res: Response, next: NextFunction): Promise<Response> {
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
      
      const queryDto: IAllergyQueryDto = req.query as IAllergyQueryDto;
      const allergiesOrError = await this.allergyServiceInstance.queryAllergies(queryDto) as Result<IAllergyDTO[]>;

      if (allergiesOrError.isFailure) {
        return res.status(400).send({ message: allergiesOrError.errorValue() });
      }

      const allergies = allergiesOrError.getValue();
      return res.status(200).json(allergies);
    } catch (e) {
      console.error('Error in queryAllergies:', e);
      return res.status(500).send({ message: 'Internal server error' });
    }
  }
}
