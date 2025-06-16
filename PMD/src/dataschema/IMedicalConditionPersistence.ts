/* eslint-disable prettier/prettier */
export interface IMedicalConditionPersistence {
  code: number;
  designation: string;
  description?: string | null;
  symptoms: string;
  createdAt?: Date;
  updatedAt?: Date;
}
