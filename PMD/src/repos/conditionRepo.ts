/* eslint-disable prettier/prettier */
/* eslint-disable @typescript-eslint/no-unused-vars */
/* eslint-disable prettier/prettier */
/* eslint-disable @typescript-eslint/no-explicit-any */
/* eslint-disable prettier/prettier */

import { Document, FilterQuery, Model, UpdateQuery } from 'mongoose';
import { MedicalCondition } from '../domain/MedicalCondition';
import IConditionRepo from '../services/IRepos/IConditionRepo';
import { MedicalConditionCode } from '../domain/MedicalConditionCode';
import { IMedicalConditionPersistence } from '../dataschema/IMedicalConditionPersistence';
import { Inject, Service } from 'typedi';
import { MedicalConditionMap } from '../mappers/MedicalConditionMap';
import IMedicalConditionQueryDto from '../dto/IMedicalConditionQueryDto';
import IEditMedicalConditionDto from '../dto/IEditMedicalConditionDto';

@Service()
export default class ConditionRepo implements IConditionRepo {
  constructor(
    @Inject('conditionSchema') private conditionSchema: Model<IMedicalConditionPersistence & Document>,
  ) {}

  public async find(queryDto: IMedicalConditionQueryDto): Promise<MedicalCondition[]> {
    const query: FilterQuery<IMedicalConditionPersistence & Document> = {};
    console.log(queryDto);
  
    if (queryDto.designation?.trim()) {
      query.designation = { $regex: queryDto.designation, $options: 'i' };
    }
    if (queryDto.symptoms?.trim()) {
      query.symptoms = { $regex: queryDto.symptoms, $options: 'i' };
    }
    if (queryDto.code?.trim()) {
        query.code = queryDto.code;
    }
   
    console.log(query);
    const conditionDocuments = await this.conditionSchema.find(query);
    return conditionDocuments.map(doc => MedicalConditionMap.toDomain(doc));
  }

  public async exists(condition: MedicalCondition): Promise<boolean> {
    const idX = condition.id instanceof MedicalConditionCode ? (<MedicalConditionCode>condition.id) : condition.id;

    const query = { domainId: idX }; 
    const conditionDocument = await this.conditionSchema.findOne(query as FilterQuery<IMedicalConditionPersistence & Document>);

    return !!conditionDocument === true;
  }

  public async save(condition: MedicalCondition): Promise<MedicalCondition> {
    const query = { code: condition.id.toValue() };

    const conditionDocument = await this.conditionSchema.findOne(query);

    try {
      if (conditionDocument === null) {
        const rawCondition: any = MedicalConditionMap.toPersistence(condition);

        const conditionCreated = await this.conditionSchema.create(rawCondition);

        return MedicalConditionMap.toDomain(conditionCreated);
      } else {
        conditionDocument.designation = condition.designation;
        conditionDocument.description = condition.description;
        conditionDocument.symptoms = condition.symptoms;
        await conditionDocument.save();

        return condition;
      }
    } catch (err) {
      throw err;
    }
  }

  public async findByDomainId(conditionId: MedicalConditionCode | number): Promise<MedicalCondition | null> {
    const query = { code: conditionId };
    const conditionRecord = await this.conditionSchema.findOne(query as FilterQuery<IMedicalConditionPersistence & Document>);

    if (conditionRecord != null) {
      return MedicalConditionMap.toDomain(conditionRecord);
    } else {
      return null;
    }
  }

  public async getHighestId(): Promise<number | null> {
    const conditionRecord = await this.conditionSchema
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
  
    console.log('Highest Id:', conditionRecord[0]?.domainId);
    return conditionRecord[0] ? conditionRecord[0].domainId : null;
  }

  /* public async update(conditionId: number, editConditionDto: IEditMedicalConditionDto): Promise<MedicalCondition> {
    const query: FilterQuery<IMedicalConditionPersistence & Document> = { domainId: conditionId };
    const update: UpdateQuery<IMedicalConditionPersistence & Document> = {};

    if (editConditionDto.designation) {
      update.designation = editConditionDto.designation;
    }
    if (editConditionDto.description) {
      update.description = editConditionDto.description;
    }
    if (editConditionDto.symptoms) {
      update.symptoms = editConditionDto.symptoms;
    }

    const updatedMedicalDocument = await this.conditionSchema.findOneAndUpdate(query, update, { new: true });

    if (!updatedMedicalDocument) {
      throw new Error(`MedicalCondition with ID ${conditionId} not found`);
    }

    return MedicalConditionMap.toDomain(updatedMedicalDocument);
  } */

}
