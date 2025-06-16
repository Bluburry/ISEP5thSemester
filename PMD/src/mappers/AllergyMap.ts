/* eslint-disable prettier/prettier */
import { IAllergyPersistence } from "../dataschema/IAllergyPestistence";
import { Allergy } from "../domain/Allergy";
import IAllergyDTO from "../dto/IAllergyDTO";

export class AllergyMap {
  public static toPersistence(allergy: Allergy): IAllergyPersistence {
    return {
      domainId: allergy.id.toString(),
      name: allergy.name,
      description: allergy.description,
    };
  }

  public static toDomain(raw: IAllergyPersistence): Allergy {
    return Allergy.create(
      {
        name: raw.name,
        description: raw.description,
        id: raw.domainId
      },
      raw.domainId
    ).getValue(); // Assumes Result<T> is used for Allergy creation.
  }

  public static toDTO(allergy: Allergy): IAllergyDTO {
    return {
      id: allergy.id.toString(),
      name: allergy.name,
      description: allergy.description,
    };
  }
}