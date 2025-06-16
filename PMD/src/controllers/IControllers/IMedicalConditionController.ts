/* eslint-disable prettier/prettier */
import { Request, Response, NextFunction } from 'express';

export default interface IMedicalConditionController {
  createCondition(token: string, req: Request, res: Response, next: NextFunction): Promise<Response>; // Return type updated to Response
  searchCondition(token: string, req: Request, res: Response, next: NextFunction): Promise<Response>; // Return type updated to Response
  //updateCondition(token: string, id: string, req: Request, res: Response, next: NextFunction): Promise<Response>; // Return type updated to Response
}
