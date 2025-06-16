/* eslint-disable prettier/prettier */
import { AggregateRoot } from '../core/domain/AggregateRoot';
import { UniqueEntityID } from '../core/domain/UniqueEntityID';

import { Result } from '../core/logic/Result';


import IAllergyDTO from '../dto/IAllergyDTO';
import { AllergyId } from './AllergyId';

interface AllergyProps {
    name: string;
    description?: string | null;
  }
  
  export class Allergy extends AggregateRoot<AllergyProps> {
    get id(): UniqueEntityID {
      return this._id;
    }
  
    get allergyId(): AllergyId {
      return new AllergyId(this.id);
    }
  
    get name(): string {
      return this.props.name;
    }
  
    set name(value: string) {
      this.props.name = value;
    }
  
    get description(): string | null {
      return this.props.description ?? null;
    }
  
    set description(value: string | null) {
      this.props.description = value;
    }
  
    private constructor(props: AllergyProps, id?: UniqueEntityID) {
      super(props, id);
    }
  
    public static create(allergyDTO: IAllergyDTO, id?: string | UniqueEntityID): Result<Allergy> {
      const { name, description } = allergyDTO;
      

      if (!name || name.trim().length === 0) {
        return Result.fail<Allergy>('Must provide an allergy name');
      }

      // Ensure `id` is converted to a `UniqueEntityID` if it's a string
      if(id == null){
        id = new UniqueEntityID('4A8Z.1');
      }

      const allergy = new Allergy({ name: name.trim(), description: description || null }, new UniqueEntityID(id?.toString()));

      return Result.ok<Allergy>(allergy);
    }
  }