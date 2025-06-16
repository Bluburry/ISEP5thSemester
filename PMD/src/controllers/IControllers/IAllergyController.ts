/* eslint-disable prettier/prettier */
import { Request, Response, NextFunction } from 'express';

export default interface IAllergyController {
  createAllergy(token: string, req: Request, res: Response, next: NextFunction): Promise<Response>; // Return type updated to Response
  queryAllergies(token: string,req: Request, res: Response, next: NextFunction): Promise<Response>; // Return type updated to Response
  patchAllergies(token: string, id: string, req: Request, res: Response, next: NextFunction): Promise<Response>; // Return type updated to Response

}
