/* eslint-disable prettier/prettier */
/* eslint-disable @typescript-eslint/no-loss-of-precision */
/* eslint-disable prettier/prettier */
import { AggregateRoot } from '../core/domain/AggregateRoot';
import { UniqueEntityID } from '../core/domain/UniqueEntityID';
import { Result } from '../core/logic/Result';
import IMedicalConditionDTO from '../dto/IMedicalConditionDTO';
import { MedicalConditionCode } from './MedicalConditionCode';

interface MedicalConditionProps {
  designation: string;
  description?: string | null;
  symptoms: string;
}

export class MedicalCondition extends AggregateRoot<MedicalConditionProps> {
  get id(): UniqueEntityID {
    return this._id;
  }

  get medicalConditionCode(): MedicalConditionCode {
    return new MedicalConditionCode(this.id);
  }

  get designation(): string {
    return this.props.designation;
  }

  set designation(value: string) {
    this.props.designation = value;
  }

  get description(): string | null {
    return this.props.description ?? null;
  }

  set description(value: string | null) {
    this.props.description = value;
  }

  get symptoms(): string {
    return this.props.symptoms;
  }

  set symptoms(value: string) {
    this.props.symptoms = value;
  }

  private constructor(props: MedicalConditionProps, id?: UniqueEntityID) {
    super(props, id);
  }

  public static create(conditionDTO: IMedicalConditionDTO, id: number | UniqueEntityID): Result<MedicalCondition> {
    const { designation, description, symptoms } = conditionDTO;

    if (!designation || designation.trim().length === 0) {
      return Result.fail<MedicalCondition>('Must provide a medical condition name');
    }

    // Ensure `id` is a valid number or a UniqueEntityID
    if (typeof id === 'number') {
      const isValidId = id >= 100000 && id <= 999999999999999999;
      if (!isValidId) {
        return Result.fail<MedicalCondition>('Code must be a number between 6 and 18 digits to be eligible');
      }
      id = new UniqueEntityID(id);
    } else {
      return Result.fail<MedicalCondition>('Code must be a number.');
    }

    const condition = new MedicalCondition({ designation: designation.trim(), description: description || null, symptoms: symptoms }, new UniqueEntityID(id?.toValue()));

    return Result.ok<MedicalCondition>(condition);
  }

  public toDTO(): IMedicalConditionDTO {
    return {
      code: parseInt(this.id.toValue().toString(), 10),
      designation: this.designation,
      description: this.description,
      symptoms: this.symptoms,
    };
  }
}