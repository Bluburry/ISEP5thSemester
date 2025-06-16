/* eslint-disable prettier/prettier */
export interface IAllergyPersistence {
  domainId: string;
  name: string;
  description?: string | null;
  createdAt?: Date;
  updatedAt?: Date;
}
