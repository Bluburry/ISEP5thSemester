/* eslint-disable prettier/prettier */
/* eslint-disable @typescript-eslint/no-explicit-any */
/* eslint-disable prettier/prettier */
import { UniqueEntityID } from '../core/domain/UniqueEntityID';
import { Entity } from '../core/domain/Entity';

export class AllergyId extends Entity<any> {
  get id(): UniqueEntityID {
    return this._id;
  }

  public constructor(id: UniqueEntityID) {
    super(null, id);
  }

  public static create(id?: UniqueEntityID): AllergyId {
    return new AllergyId(id || new UniqueEntityID());
  }
}
