/* eslint-disable prettier/prettier */
/* eslint-disable @typescript-eslint/no-unused-vars */
/* eslint-disable prettier/prettier */
import { Request, Response, NextFunction } from 'express';
import { Service, Inject } from 'typedi';
import IClinicalDetailsController from './IControllers/IClinicalDetailsController';
import IClinicalDetailsService from '../services/IServices/IClinicalDetailsService';
import { ClinicalDetailsMap } from '../mappers/ClinicalDetailsMap';
import { RSADecryptionService } from '../services/RSADecryptionService';
import config from '../../config';
import { cli } from 'winston/lib/winston/config';
import { IClinicalDetailsDTO } from '../dto/IClinicalDetailsDTO';
import path from 'path';

@Service()
export default class ClinicalDetailsController implements IClinicalDetailsController {
  constructor(
    @Inject(config.services.clinicalDetails.name) private clinicalDetailsService: IClinicalDetailsService
  ) {}
    public async getClinicalDetailsByMRN(token: string, mrn: string, res: Response, next: NextFunction): Promise<Response> {
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
          if (authZ != 'ADMIN_AUTH_TOKEN' && authZ != 'STAFF_AUTH_TOKEN' && authZ != 'PATIENT_AUTH_TOKEN') {
            return res.status(401).send({ message: 'Unauthorized' });
          }
        } catch (error) {
          
          console.error('Error parsing token JSON:', error.message);
          throw error;
        }
      try {
        const result = await this.clinicalDetailsService.findByDomainId(mrn);

        return res.status(201).json(result);
      } catch (e) {
        
        next(e);
      }
      } catch (error) {
        next(error);
      }
  }
    
  public async filterClinicalDetails(token: string, allergyID: string, medicalConditionID: string, res: Response, next: NextFunction): Promise<Response> {
      console.log('Fetching Clinical Details' + allergyID + medicalConditionID);
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
          if (authZ != 'ADMIN_AUTH_TOKEN' && authZ != 'STAFF_AUTH_TOKEN') {
            return res.status(401).send({ message: 'Unauthorized' });
          }
        } catch (error) {
          console.error('Error parsing token JSON:', error.message);
          throw error;
        }
      try {
        const result = await this.clinicalDetailsService.filterByValues(allergyID, medicalConditionID);

        return res.status(201).json(result);
      } catch (e) {
        next(e);
      }
      } catch (error) {
        next(error);
      }
  }

  public async saveClinicalDetails(token: string, req: Request, res: Response, next: NextFunction): Promise<Response> {
    console.log('Creating Clinical Details');
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
        if (authZ != 'ADMIN_AUTH_TOKEN' && authZ != 'STAFF_AUTH_TOKEN') {
          return res.status(401).send({ message: 'Unauthorized' });
        }
      } catch (error) {
        console.error('Error parsing token JSON:', error.message);
        throw error;
    }
    try {
      const result = await this.clinicalDetailsService.save(req.body);

      return res.status(201).json(result);
    } catch (e) {
      next(e);
    }
    } catch (error) {
      next(error);
    }
  }

  public async createBlankClinicalDetails(token: string, mrn: string, res: Response, next: NextFunction): Promise<Response> {
    console.log('Creating blank Clinical Details');
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
          try {
            const result = await this.clinicalDetailsService.createBlank(mrn);
            return res.status(201).json(result);
          } catch (e) {
            next(e);
          }
    } catch (error) {
      next(error);
    }
  }

	public async sendClinicalDetails(token: string, mrn: string, password: string, res: Response, next: NextFunction): Promise<Response> {
		try {
			const rsa = new RSADecryptionService();
			const decryptedToken = rsa.decrypt(token);

			if (token == null) {
				console.error('No token in header');
				return res.status(400).send({ message: 'No token in header' });
			}
			try {
				const tokenObject = JSON.parse(decryptedToken);
				const authZ = tokenObject.TokenValue;
				console.log(authZ);
				if (authZ != 'ADMIN_AUTH_TOKEN' && authZ != 'STAFF_AUTH_TOKEN' && authZ != 'PATIENT_AUTH_TOKEN') {
					return res.status(401).send({ message: 'Unauthorized' });
				}
			} catch (error) {

				console.error('Error parsing token JSON:', error.message);
				throw error;
			}
			try {
				const result = await this.clinicalDetailsService.sendClinicalDetails(mrn, password);

				return res.status(201).json(result);
			} catch (e) {

				next(e);
			}
		} catch (error) {
			next(error);
		}
	}

	public async sendClinicalDetailsPresentation(token: string, password: string, res: Response, next: NextFunction): Promise<Response> {
		//console.log("received request sendClinicalDetailsPresentation");
		const outputDir = "./exports/"
		try {
			const rsa = new RSADecryptionService();
			const decryptedToken = rsa.decrypt(token);

			if (token == null) {
				console.error('No token in header');
				return res.status(400).send({ message: 'No token in header' });
			}
			try {
				const tokenObject = JSON.parse(decryptedToken);
				const authZ = tokenObject.TokenValue;
				console.log(authZ);
				if (authZ != 'ADMIN_AUTH_TOKEN' && authZ != 'STAFF_AUTH_TOKEN' && authZ != 'PATIENT_AUTH_TOKEN') {
					return res.status(401).send({ message: 'Unauthorized' });
				}
			} catch (error) {

				console.error('Error parsing token JSON:', error.message);
				throw error;
			}
			try {
				const result = await this.clinicalDetailsService.sendClinicalDetailsDemonstration(password);

				const filePath = path.join(outputDir, result);
				
				res.setHeader('Content-Type', 'application/zip');
				res.setHeader('Content-Disposition', `attachment; filename="${result}"`);

				console.log("File path: ", filePath);

				res.download(filePath, (err) => {
					if (err) {
						console.error('Error sending file:', err);
						res.status(500).send('File could not be sent.');
					}
				});

				//return res.status(201).json(result);
			} catch (e) {

				next(e);
			}
		} catch (error) {
			next(error);
		}
	}
}
