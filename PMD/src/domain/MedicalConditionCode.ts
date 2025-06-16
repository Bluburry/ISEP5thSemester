/* eslint-disable prettier/prettier */
/* eslint-disable @typescript-eslint/no-explicit-any */
/* eslint-disable prettier/prettier */
import { UniqueEntityID } from '../core/domain/UniqueEntityID';
import { Entity } from '../core/domain/Entity';

export class MedicalConditionCode extends Entity<any> {
  get id(): UniqueEntityID {
    return this._id;
  }

  public constructor(id: UniqueEntityID) {
    super(null, id);
  }

  public static create(id?: UniqueEntityID | number): MedicalConditionCode {
    // Handle `id` as a number or `UniqueEntityID`
    if (typeof id === 'number') {
      // Validate number is between 6 and 18 digits
      const isValidId = id >= 100000 && id <= 999999999999999999;
      if (!isValidId) {
        throw new Error('ID must be a number between 6 and 18 digits');
      }
      id = new UniqueEntityID(id);
    }

    return new MedicalConditionCode(id || new UniqueEntityID());
  }
}
