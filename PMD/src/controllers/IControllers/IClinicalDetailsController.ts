/* eslint-disable prettier/prettier */
import { Request, Response, NextFunction } from 'express';
import { IClinicalDetailsDTO } from '../../dto/IClinicalDetailsDTO';

export default interface IClinicalDetailsController {
	getClinicalDetailsByMRN(token: string, mrn: string, res: Response, next: NextFunction): Promise<Response>;
	saveClinicalDetails(token: string, req: Request, res: Response, next: NextFunction): Promise<Response>;
	createBlankClinicalDetails(token: string, mrn: string, res: Response, next: NextFunction): Promise<Response>;
	sendClinicalDetails(token: string, mrn: string, password: string, res: Response, next: NextFunction): Promise<Response>;
	sendClinicalDetailsPresentation(token: string, password: string, res: Response, next: NextFunction): Promise<Response>;
}
