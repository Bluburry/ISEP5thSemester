/* eslint-disable prettier/prettier */
/* eslint-disable @typescript-eslint/no-unused-vars */
/* eslint-disable prettier/prettier */
/* eslint-disable @typescript-eslint/no-explicit-any */
/* eslint-disable prettier/prettier */

import { Document, FilterQuery, Model, UpdateQuery } from 'mongoose';
import { Allergy } from '../domain/Allergy';
import IAllergyRepo from '../services/IRepos/IAllergyRepo';
import { AllergyId } from '../domain/AllergyId';
import { IAllergyPersistence } from '../dataschema/IAllergyPestistence';
import { Inject, Service } from 'typedi';
import { AllergyMap } from '../mappers/AllergyMap';
import IAllergyQueryDto from '../dto/IAllergyQueryDto';
import IEditAllergyDto from '../dto/IEditAllergyDto';

@Service()
export default class AllergyRepo implements IAllergyRepo {
  constructor(
    @Inject('allergySchema') private allergySchema: Model<IAllergyPersistence & Document>,
  ) {}


  public async patch(allergyId: string, editAllergyDto: IEditAllergyDto): Promise<Allergy> {
    const query: FilterQuery<IAllergyPersistence & Document> = { domainId: allergyId };
    const update: UpdateQuery<IAllergyPersistence & Document> = {};

    if (editAllergyDto.name) {
      update.name = editAllergyDto.name;
    }
    if (editAllergyDto.description) {
      update.description = editAllergyDto.description;
    }

    const updatedAllergyDocument = await this.allergySchema.findOneAndUpdate(query, update, { new: true });

    if (!updatedAllergyDocument) {
      throw new Error(`Allergy with ID ${allergyId} not found`);
    }

    return AllergyMap.toDomain(updatedAllergyDocument);
  }
  
  public async find(queryDto: IAllergyQueryDto = { name: '', code: '', description: '' }): Promise<Allergy[]> {
    const query: FilterQuery<IAllergyPersistence & Document> = {};
    console.log(queryDto);
  
    if (queryDto.name && queryDto.name !== '') {
      query.name = { $regex: queryDto.name, $options: 'i' }; // Case-insensitive partial match
    }
    if (queryDto.code && queryDto.code !== '') {
      query.domainId = { $regex: queryDto.code, $options: 'i' }; // Case-insensitive partial match
    }
    if (queryDto.description && queryDto.description !== '') {
      query.description = { $regex: queryDto.description, $options: 'i' }; // Case-insensitive partial match
    }
  
    const allergyDocuments = await this.allergySchema.find(query);
    return allergyDocuments.map(doc => AllergyMap.toDomain(doc));
  }

  public async exists(allergy: Allergy): Promise<boolean> {
    const idX = allergy.id instanceof AllergyId ? (<AllergyId>allergy.id) : allergy.id;

    const query = { domainId: idX }; 
    const allergyDocument = await this.allergySchema.findOne(query as FilterQuery<IAllergyPersistence & Document>);

    return !!allergyDocument === true;
  }

  public async save(allergy: Allergy): Promise<Allergy> {
    const query = { domainId: allergy.id.toString() };

    const allergyDocument = await this.allergySchema.findOne(query);

    try {
      if (allergyDocument === null) {
        const rawAllergy: any = AllergyMap.toPersistence(allergy);

        const allergyCreated = await this.allergySchema.create(rawAllergy);

        return AllergyMap.toDomain(allergyCreated);
      } else {
        allergyDocument.name = allergy.name;
        allergyDocument.description = allergy.description;
        await allergyDocument.save();

        return allergy;
      }
    } catch (err) {
      throw err;
    }
  }

  public async findByDomainId(allergyId: AllergyId | string): Promise<Allergy | null> {
    const query = { domainId: allergyId };
    const allergyRecord = await this.allergySchema.findOne(query as FilterQuery<IAllergyPersistence & Document>);

    if (allergyRecord != null) {
      return AllergyMap.toDomain(allergyRecord);
    } else {
      return null;
    }
  }

  public async findByRawID(allergyId: AllergyId | string): Promise<Allergy | null> {
    const query = { _id: allergyId };
    const allergyRecord = await this.allergySchema.findOne(query as FilterQuery<IAllergyPersistence & Document>);

    if (allergyRecord != null) {
      return AllergyMap.toDomain(allergyRecord);
    } else {
      return null;
    }
  }

  public async getHighestId(): Promise<string | null> {
    const allergyRecord = await this.allergySchema
      .aggregate([
        {
          $addFields: {
            numericPart: {
              $toDouble: { $arrayElemAt: [{ $split: ["$domainId", "."] }, 1] }
            }
          }
        },
        { $sort: { numericPart: -1 } },
        { $limit: 1 },
        { $project: { domainId: 1 } }
      ])
      .exec();
  
    console.log('Highest Id:', allergyRecord[0]?.domainId);
    return allergyRecord[0] ? allergyRecord[0].domainId : null;
  }
  
}
