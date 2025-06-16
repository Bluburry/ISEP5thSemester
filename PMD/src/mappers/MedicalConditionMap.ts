/* eslint-disable prettier/prettier */
import { IMedicalConditionPersistence } from "../dataschema/IMedicalConditionPersistence";
import { MedicalCondition } from "../domain/MedicalCondition";

export class MedicalConditionMap {
  public static toPersistence(condition: MedicalCondition): IMedicalConditionPersistence {
    const idValue = condition.id.toValue();
  
    if (typeof idValue === 'string') {
      throw new Error('ID must be a number for persistence'); // Tecnically, this should never trigger
    }
  
    return {
      code: idValue, // Guaranteed to be a number
      designation: condition.designation,
      description: condition.description,
      symptoms: condition.symptoms,
    };
  }
  

  public static toDomain(raw: IMedicalConditionPersistence): MedicalCondition {
    return MedicalCondition.create(
      {
        designation: raw.designation,
        description: raw.description,
        symptoms: raw.symptoms,
        code: raw.code
      },
      raw.code
    ).getValue(); // Assumes Result<T> is used for Allergy creation.
  }
}
